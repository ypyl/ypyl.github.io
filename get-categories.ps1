$dirs = @("$PSScriptRoot\_tools", "$PSScriptRoot\_learning")
$categories = $dirs | ForEach-Object {
    if (Test-Path $_) {
        Get-ChildItem -Path $_ -Filter *.md | ForEach-Object {
            if ((Get-Content $_.FullName -Raw) -match 'category:\s*(.+)') {
                $matches[1].Trim()
            }
        }
    }
} | Select-Object -Unique

$categories -join ', '
