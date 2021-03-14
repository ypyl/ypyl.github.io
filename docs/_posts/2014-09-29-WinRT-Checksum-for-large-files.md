---
layout: post
title: "WinRT Checksum for large files"
date: 2014-09-29

tags: dotnet winrt
categories: programming
---
There is a good [question](http://stackoverflow.com/questions/13534334/how-to-compute-hash-md5-or-sha-of-a-large-file-with-c-sharp-in-windows-store-a) about this.

Also there is a code. I have used MD5 algorithm for my purposes.
```cs
public async Task<string> GetFileChecksumAsync(string fileName)
{
  HashAlgorithmProvider alg = Windows.Security.Cryptography.Core.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
  IStorageFile stream = await openFile(fileName);
  using (var inputStream = await stream.OpenReadAsync())
  {
    Windows.Storage.Streams.Buffer buffer = new Windows.Storage.Streams.Buffer(BUFFER_SIZE);
    var hash = alg.CreateHash();

    while (true)
    {
      await inputStream.ReadAsync(buffer, BUFFER_SIZE, InputStreamOptions.None);
      if (buffer.Length > 0)
        hash.Append(buffer);
      else
        break;
    }

    return CryptographicBuffer.EncodeToHexString(hash.GetValueAndReset()).ToUpper();
  }
}
```