$categories = Get-ChildItem -Path "$PSScriptRoot\_tools" -Filter *.md | ForEach-Object {
    if ((Get-Content $_.FullName -Raw) -match 'category:\s*(.+)') {
        $matches[1].Trim()
    }
} | Select-Object -Unique

$categories -join ', '
