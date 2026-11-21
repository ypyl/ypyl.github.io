#!/usr/bin/env python3
"""Validate frontmatter of every content file in the repo.

Jekyll logs a broken frontmatter block as "YAML Exception reading ..." and still exits 0, so a
colon inside an unquoted description, a stray tab or a bad tag list reaches production unless
something checks it. This does, with a real YAML parser.

Usage: python scripts/check-frontmatter.py [path ...]
"""

import datetime
import re
import sys
from pathlib import Path

try:
    import yaml
except ImportError:  # pragma: no cover
    sys.exit("PyYAML is required: python -m pip install pyyaml")

ROOT = Path(__file__).resolve().parent.parent
DATE_TAG = re.compile(r"^\d{4}-\d{2}-\d{2}$")
TAG = re.compile(r"^[a-z0-9]+(-[a-z0-9]+)*$")

# directory -> (required keys, keys that must be lists of tags)
COLLECTIONS = {
    "_news": (["layout", "title", "date"], []),
    "_posts": (["layout", "title", "date"], []),
    "_tools": (["name", "link", "category", "tags", "description"], ["tags"]),
    "_learning": (["name", "link", "category", "tags", "description"], ["tags"]),
    "_apps": (["name", "link", "order", "tags", "description"], ["tags"]),
    "_presentations": (["layout"], []),
    "_drafts": ([], []),
}

# Only the collections this repo writes today. The older posts predate these rules, so checking
# them by default would bury a new problem under legacy noise. Pass a path to check one anyway.
DEFAULT_TARGETS = ["_news", "_tools", "_learning", "_apps"]

# Cards carry the add date as first tag; collection-specific rules live in AGENTS.md.
DATED_TAGS = {"_tools", "_learning", "_apps"}

LEARNING_CATEGORIES = {
    "Book",
    "Course",
    "Curated List",
    "Guide",
    "Research Paper",
    "Tutorial",
}


def fail(errors, path, message):
    try:
        shown = path.relative_to(ROOT)
    except ValueError:
        shown = path
    errors.append(f"{shown}: {message}")


def split_frontmatter(text, path, errors):
    if not text.startswith("---"):
        fail(errors, path, "no frontmatter block")
        return None
    lines = text.splitlines()
    for index in range(1, len(lines)):
        if lines[index].strip() == "---":
            return "\n".join(lines[1:index])
    fail(errors, path, "frontmatter block is never closed with ---")
    return None


def check_file(path, collection, errors):
    required, list_keys = COLLECTIONS[collection]
    raw = split_frontmatter(path.read_text(encoding="utf-8"), path, errors)
    if raw is None:
        return

    try:
        data = yaml.safe_load(raw)
    except yaml.YAMLError as exc:
        detail = re.sub(r"\s+", " ", str(exc)).strip()
        hint = ""
        if ": " in raw:
            hint = " (an unquoted value containing ': ' is the usual cause, quote the whole value)"
        fail(errors, path, f"frontmatter is not valid YAML: {detail}{hint}")
        return

    if not isinstance(data, dict):
        fail(errors, path, "frontmatter is not a mapping")
        return

    for key in required:
        if key not in data:
            fail(errors, path, f"missing required key '{key}'")

    for key in list_keys:
        if key not in data:
            continue
        tags = data[key]
        if not isinstance(tags, list):
            fail(errors, path, f"'{key}' must be a YAML array, got {type(tags).__name__}")
            continue
        for tag in tags:
            tag = str(tag)
            if not DATE_TAG.match(tag) and not TAG.match(tag):
                fail(errors, path, f"tag '{tag}' must be lowercase and hyphenated")

        if collection in DATED_TAGS and tags and not DATE_TAG.match(str(tags[0])):
            fail(errors, path, f"first '{key}' entry must be the add date, got '{tags[0]}'")

    if collection == "_learning":
        category = data.get("category")
        if category is not None and category not in LEARNING_CATEGORIES:
            allowed = ", ".join(sorted(LEARNING_CATEGORIES))
            fail(errors, path, f"category '{category}' is not one of: {allowed}")
        if category is not None and isinstance(data.get("tags"), list):
            slug = str(category).lower().replace(" ", "-")
            if slug in [str(tag) for tag in data["tags"]]:
                fail(errors, path, f"category '{category}' must not repeat as a tag")

    date = data.get("date")
    if date is not None and not isinstance(date, (datetime.date, str)):
        fail(errors, path, f"'date' must be a date, got {type(date).__name__}")


def main(argv):
    targets = [Path(arg) for arg in argv[1:]] or [ROOT / name for name in DEFAULT_TARGETS]
    errors = []
    checked = 0

    for target in targets:
        if not target.exists():
            continue
        if target.is_file():
            candidates = [(target, target.parent.name)]
        else:
            candidates = [
                (path, target.name) for path in sorted(target.rglob("*.md"))
            ]
        for path, collection in candidates:
            if collection not in COLLECTIONS:
                continue
            checked += 1
            check_file(path, collection, errors)

    if errors:
        print(f"{len(errors)} frontmatter problem(s):\n")
        for error in errors:
            print(f"  {error}")
        return 1

    print(f"frontmatter ok: {checked} files")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
