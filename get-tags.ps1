$dirs = @("$PSScriptRoot\_tools", "$PSScriptRoot\_learning")
$tags = $dirs | ForEach-Object {
    if (Test-Path $_) {
        Get-ChildItem -Path $_ -Filter *.md | ForEach-Object {
            if ((Get-Content $_.FullName -Raw) -match 'tags:\s*\[([^\]]+)\]') {
                $matches[1] -split ',\s*'
            }
        }
    }
} | Select-Object -Unique

$tags -join ', '
