$tags = Get-ChildItem -Path "$PSScriptRoot\_tools" -Filter *.md | ForEach-Object {
    if ((Get-Content $_.FullName -Raw) -match 'tags:\s*\[([^\]]+)\]') {
        $matches[1] -split ',\s*'
    }
} | Select-Object -Unique

$tags -join ', '
