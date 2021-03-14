---
layout: post
title: "Use WebSQL and IndexedDB in Typescript"
date: 2016-10-25

tags: javascript
categories: programming
---
More information about [IndexedDb](https://developer.mozilla.org/en-US/docs/Web/API/IndexedDB_API) or [WebSQL](https://www.w3.org/TR/webdatabase/).

Let's define base interfaces for our task:

```ts
export interface IItem {
    id: string;
    value: string;
}

export interface IStorage<T extends IItem> {
    // Initial method to create storage
    init(name: string): Observable<IStorage<T>>;

    // Get the value by unique key
    get(key: string): Observable<T>;

    // Clear/remove all data in the storage
    clear(): Observable<T>;

    // Put specific value into the storage
    put(value: T): Observable<T>;

    // Get all values using the set of keys
    getDenseBatch(keys: string[]): Observable<T>;

    // Get all values from the storage
    all(): Observable<T>;
}
```

Here I am using [rxjs](http://reactivex.io/) to handle results. IItem is an interface for items which we are saving, IStorage is an interface for a specific storage.

### In Memory implementation

A short example how to implement mentioned interface using in-memory array:

```ts
export class MemoryStorage<T extends IItem> implements IStorage<T> {
    private storage: { [key: string]: T } = {};

    init(name: string): Observable<MemoryStorage<T>> {
        return Observable.of(this);
    }

    get(key: string): Observable<T> {
        return Observable.of(this.storage[key]);
    }

    clear(): Observable<T> {
        this.storage = {};
        return Observable.empty<T>();
    }

    put(value: T): Observable<T> {
        if (!value.id) {
            value.id = Math.random().toString(36).substring(7);
        }
        this.storage[value.id] = value;
        return Observable.of(value);
    }

    getDenseBatch(keys: string[]): Observable<T> {
        return Observable.from(keys.map(x => this.storage[x]));
    }

    all(): Observable<T> {
        return Observable.from(Object.keys(this.storage).map(x => this.storage[x]));
    }
}
```

Simple implementation of IItem:

```ts
class TestKeyValue implements IItem {
  public id: string;
  public value: string;
}
```

Unit tests for MemoryStorage:

```ts
describe('MemoryStorage: Class', () => {
  let key1 = 'key1', key2 = 'key2';
  let value1 = 'value1', value2 = 'value2';

  function init(): MemoryStorage<TestKeyValue> {
    let storage = new MemoryStorage<TestKeyValue>();
    storage.init('test');
    return storage;
  }

  it('should create empty storage', async(() => {
    let storage = init();
    storage.all().isEmpty().subscribe(isAny => expect(isAny).toBeTruthy());
  }));

  it('should save one item', async(() => {
    let storage = init();
    storage.put({ id: key1, value: value1 });
    storage.all().isEmpty().subscribe(isAny => expect(isAny).toBeFalsy());
  }));

  it('should save/get one item', async(() => {
    let storage = init();
    let item = { id: key1, value: value1 };
    storage.put(item);
    storage.get(key1).subscribe(value => expect(value).toEqual(item));
  }));

  it('should save/get two items', async(() => {
    let storage = init();
    let items = [{ id: key1, value: value1 }, { id: key2, value: value2 }];
    storage.put(items[0]);
    storage.put(items[1]);
    let i = 0;
    storage.getDenseBatch([key1, key2]).subscribe(value => expect(value).toEqual(items[i++]));
  }));

  it('should clear saved items', async(() => {
    let storage = init();
    let items = [{ id: key1, value: value1 }, { id: key2, value: value2 }];
    storage.put(items[0]);
    storage.put(items[1]);

    storage.clear();
    storage.all().isEmpty().subscribe(isAny => expect(isAny).toBeTruthy());
  }));
});
```

### WebSQL implementation

Current implementation just just for objects where key (string) is unique string value, value (string) is a payload.

```ts
export class WebSQLStorage<T extends IItem> implements IStorage<T> {
    private db: Database;
    private databaseName: string = 'TripNoteDB';
    private name: string;

    constructor() {
        this.db = window.openDatabase(this.databaseName, '1.0', `Store information`, 40 * 1024 * 1024);
    }

    init(name: string): Observable<WebSQLStorage<T>> {
        this.name = name;
        return Observable.create((observer: Observer<WebSQLStorage<T>>) => {
            this.db.transaction(
                (tx) => tx.executeSql(`CREATE TABLE IF NOT EXISTS ${name} (key unique, value string)`,
                    [],
                    (t, results) => {
                        observer.next(this);
                        observer.complete();
                    },
                    (t, message) => {
                        observer.error(message.message.toString());
                        return true;
                    })
            );
        });
    }

    get(key: string): Observable<T> {
        return Observable.create((observer: Observer<T>) => {
            this.db.transaction((tx) => {
                tx.executeSql(`SELECT * FROM ${this.name} WHERE key='${key}'`, [],
                    (t, results) => {
                        let len = results.rows.length;
                        if (len === 0) {
                            observer.next(undefined);
                        } else if (len === 1) {
                            observer.next(results.rows.item(0));
                        } else {
                            observer.error('There should be no more than one entry');
                        }
                        observer.complete();
                    },
                    (t, message) => {
                        observer.error(message.message.toString());
                        return true;
                    });
            });
        });
    }

    clear() {
        return Observable.create((observer: Observer<T>) => {
            this.db.transaction((tx) => {
                tx.executeSql(`DELETE FROM ${this.name}`, [], (t, r) => observer.complete(), (t, e) => {
                    observer.error(e.message.toString());
                    return true;
                });
            });
        });
    }

    all(): Observable<T> {
        return Observable.create((observer: Observer<T>) => {
            this.db.transaction((tx) => {
                tx.executeSql(`SELECT * FROM ${this.name}`,
                    [],
                    (t, results) => {
                        for (let i = 0; i < results.rows.length; i++) {
                            observer.next(results.rows.item(i));
                        }
                        observer.complete();
                    },
                    (t, message) => {
                        observer.error(message.message.toString());
                        return true;
                    });
            });
        });
    }

    put(value: T): Observable<T> {
        return Observable.create((observer: Observer<T>) => {
            this.db.transaction((tx) => {
                tx.executeSql(`INSERT OR REPLACE INTO ${this.name} VALUES (?, ?)`, [value.id, value.value],
                    () => {
                        observer.next(value);
                        observer.complete();
                    },
                    (t, e) => {
                        observer.error(e.message.toString());
                        return true;
                    });
            });
        });
    }

    getDenseBatch(keys: string[]): Observable<T> {
        if (keys.length === 0) {
            return Observable.empty<T>();
        };

        return Observable.create((observer: Observer<T[]>) => {
            this.db.transaction((tx) => {
                let key = keys.map(x => '\'' + x + '\'').join(',');
                tx.executeSql(`SELECT * FROM ${this.name} WHERE key IN (${key})`,
                    [],
                    (t, results) => {
                        for (let i = 0; i < results.rows.length; i++) {
                            observer.next(results.rows.item(i));
                        }
                        observer.complete();
                    },
                    (t, e) => {
                        observer.error(e.message.toString());
                        return true;
                    });
            });
        });
    }
}
```

```ts
describe('WebSQLStorage: Class', () => {
  let key1 = 'key1', key2 = 'key2';
  let value1 = 'value1', value2 = 'value2';

  it('should create empty storage', async(() => {
    let storage = new WebSQLStorage<TestKeyValue>();
    storage.init('test1').subscribe(() => {
      storage.all().isEmpty().subscribe(isAny => expect(isAny).toBeTruthy());
    });
  }));

  it('should save one item ', async(() => {
    let storage = new WebSQLStorage<TestKeyValue>();
    storage.init('test2').subscribe(() => {
      storage.put({ id: key1, value: value1 }).subscribe(() => {
        storage.all().isEmpty().subscribe(isAny => expect(isAny).toBeFalsy());
      });
    });
  }));

  it('should save/get one item', async(() => {
    let storage = new WebSQLStorage<TestKeyValue>();
    storage.init('test3').subscribe(() => {
      let item = { id: key1, value: value1 };
      storage.put(item).subscribe(() => {
        storage.get(key1).subscribe(value => {
          expect(value.value).toEqual(item.value);
        });

      });
    });
  }));

  it('should save/get two items', async(() => {
    let storage = new WebSQLStorage<TestKeyValue>();
    storage.init('test4').subscribe(() => {
      let items = [{ id: key1, value: value1 }, { id: key2, value: value2 }];
      storage.put(items[0])
      .subscribe(() => storage.put(items[1])
        .subscribe(() => {
          let i = 0;
          storage.getDenseBatch([key1, key2])
            .subscribe(value => expect(value.value).toEqual(items[i++].value));
        }));
    });
  }));

  it('should clear saved items', async(() => {
    let storage = new WebSQLStorage<TestKeyValue>();
    storage.init('test5').subscribe(() => {
      let items = [{ id: key1, value: value1 }, { id: key2, value: value2 }];
      storage.put(items[0])
        .zip(() => storage.put(items[1]))
        .subscribe(() => storage.clear()
        .subscribe(() => {
          storage.all().isEmpty().subscribe(isAny => expect(isAny).toBeTruthy());
        }));
    });
  }));
});
```

### IndexedDB implementation

How to use IndexedDB is [here](https://developer.mozilla.org/en-US/docs/Web/API/IndexedDB_API/Using_IndexedDB).
There are very useful [tricks](https://www.codeproject.com/articles/744986/how-to-do-some-magic-with-indexeddb).

```ts
export class IndexedDBStorage<T extends IItem> implements IStorage<T> {
    private databaseName: string = 'TripNoteDB';
    private name: string;

    private getDb(version?: number, storeName?: string): Observable<IDBDatabase> {
        return Observable.create((observer: Observer<number>) => {
            let req = version && version > 0 ? window.indexedDB.open(this.databaseName, version)
                : window.indexedDB.open(this.databaseName);
            req.onsuccess = (e) => {
                let db = (<any>event.target).result;
                observer.next(db);
                db.close();
                observer.complete();
            };
            req.onupgradeneeded = (e) => {
                let db = (<any>e.target).result;
                if (storeName && !db.objectStoreNames.contains(storeName)) {
                    db.createObjectStore(storeName, { keyPath: 'id' });
                    let transaction = (<any>e.target).transaction;
                    transaction.oncomplete = (event) => {
                        observer.next(db);
                        db.close();
                        observer.complete();
                    };
                };
            };
            req.onblocked = (event) => observer.error('IndexedDB is blocked');
            req.onerror = (e) => observer.error(e.error);
        });
    }

    private getVersionOfDb(name: string): Observable<number> {
        return this.getDb().map(db => {
            if (!db.objectStoreNames.contains(this.name)) {
                return db.version + 1;
            } else {
                return db.version;
            }
        });
    }

    init(name: string): Observable<IndexedDBStorage<T>> {
        this.name = name;
        return Observable.create((observer: Observer<IndexedDBStorage<T>>) => {
            this.getVersionOfDb(name).subscribe((version) => {
                this.getDb(version, name).subscribe(db => {
                    observer.next(this);
                    observer.complete();
                });
            });
        });
    }

    all(): Observable<T> {
        return Observable.create((observer: Observer<T>) => {
            this.getDb().subscribe(db => {
                let req = db.transaction(this.name, 'readwrite').objectStore(this.name)
                    .openCursor();
                req.onsuccess = (e) => {
                    let res = (<any>event.target).result;
                    if (res) {
                        observer.next(res.value);
                        res.continue();
                    }
                    observer.complete();
                };
                req.onerror = (e) => observer.error(e.error);
            });
        });
    }

    get(key: string): Observable<T> {
        return Observable.create((observer: Observer<T>) => {
            this.getDb().subscribe(db => {
                let req = db.transaction(this.name).objectStore(this.name).get(key);
                req.onerror = (e) => observer.error(e.error);
                req.onsuccess = (e) => {
                    observer.next(req.result);
                    observer.complete();
                };
            });
        });
    }

    clear(): Observable<IStorage<T>> {
        return Observable.create((observer: Observer<IStorage<T>>) => {
            this.getDb().subscribe(db => {
                let req = db.transaction(this.name, 'readwrite').objectStore(this.name).clear();
                req.onerror = (e) => observer.error(e.error);
                req.onsuccess = (e) => {
                    observer.next(this);
                    observer.complete();
                };
            });
        });
    }

    put(value: T): Observable<T> {
        return Observable.create((observer: Observer<T>) => {
            this.getDb().subscribe(db => {
                let req = db.transaction(this.name, 'readwrite').objectStore(this.name).put(value);
                req.onerror = (e) => {
                    observer.error(e.error);
                };
                req.onsuccess = (e) => {
                    observer.next(value);
                    observer.complete();
                };
            });
        });
    }

    getDenseBatch(keys: string[]): Observable<T> {
        return Observable.create((observer: Observer<T>) => {
            this.getDb().subscribe(db => {
                let set = keys.sort();
                let i = 0;
                let req = db.transaction(this.name).objectStore(this.name)
                    .openCursor();
                req.onsuccess = (e) => {
                    let cursor = (<any>event.target).result;
                    if (!cursor) { observer.complete(); return; }
                    let key = cursor.key;
                    while (key > set[i]) {
                        // The cursor has passed beyond this key. Check next.
                        ++i;
                        if (i === set.length) {
                            // There is no next. Stop searching.
                            observer.complete();
                            return;
                        }
                    }
                    if (key === set[i]) {
                        // The current cursor value should be included and we should continue
                        // a single step in case next item has the same key or possibly our
                        // next key in set.
                        observer.next(cursor.value);
                        cursor.continue();
                    } else {
                        // cursor.key not yet at set[i]. Forward cursor to the next key to hunt for.
                        cursor.continue(set[i]);
                    }
                };
                req.onerror = (e) => observer.error(e.error);
            });
        });
    }
}
```

Unit tests for indexedDB:

```ts
describe('IndexedDBStorage: Class', () => {
  let key1 = 'key1', key2 = 'key2';
  let value1 = 'value1', value2 = 'value2';

  it('should create empty storage', (done) => {
    let storage = new IndexedDBStorage<TestKeyValue>();
    storage.init('test1').subscribe(() => {
      storage.all().isEmpty().subscribe(isAny => {
        expect(isAny).toBeTruthy();
        done();
      });
    });
  });

  it('should save one item ', (done) => {
    let storage = new IndexedDBStorage<TestKeyValue>();
    storage.init('test2').subscribe(() => {
      storage.put({ id: key1, value: value1 }).subscribe(() => {
        storage.all().isEmpty().subscribe(isAny => {
          expect(isAny).toBeFalsy();
          done();
        });
      });
    });
  });

  it('should save/get one item', (done) => {
    let storage = new IndexedDBStorage<TestKeyValue>();
    storage.init('test3').subscribe(() => {
      let item = { id: key1, value: value1 };
      storage.put(item).subscribe(() => {
        storage.get(key1).subscribe(value => {
          expect(value).toEqual(item);
          done();
        });
      });
    });
  });

  it('should save/get two items', (done) => {
    let storage = new IndexedDBStorage<TestKeyValue>();
    storage.init('test4').subscribe(() => {
      let items = [{ id: key1, value: value1 }, { id: key2, value: value2 }];
      let item1 = storage.put(items[0])
        .merge(storage.put(items[1])).last()
        .subscribe(y => {
          storage.getDenseBatch([key1, key2]).toArray().subscribe(x => {
            expect(x[0]).toEqual(items[0]);
            expect(x[1]).toEqual(items[1]);
            done();
          });
        });
      });
  });

  it('should clear saved items', (done) => {
    let storage = new IndexedDBStorage<TestKeyValue>();
    storage.init('test5').subscribe(() => {
      let items = [{ id: key1, value: value1 }, { id: key2, value: value2 }];
      storage.put(items[0])
        .merge(storage.put(items[1])).last()
        .subscribe(x => storage.clear()
          .subscribe(y => storage.all().isEmpty().subscribe(isAny => {
              expect(isAny).toBeTruthy();
              done();
            })));
    });
  });
});
```