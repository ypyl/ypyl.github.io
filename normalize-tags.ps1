param(
    [switch]$WhatIf = $false
)

$ErrorActionPreference = "Stop"
$postsDir = Join-Path $PSScriptRoot "_posts"

if (-not (Test-Path $postsDir)) {
    Write-Error "Posts directory not found: $postsDir"
    exit 1
}

$files = Get-ChildItem -Path $postsDir -Recurse -Filter "*.md" | Sort-Object FullName

$stats = [PSCustomObject]@{
    Total              = 0
    Skipped_NoTags     = 0
    Skipped_AlreadyArray = 0
    Converted_Comma    = 0
    Converted_Single   = 0
    Converted_Space    = 0
}

foreach ($file in $files) {
    $stats.Total++
    $lines = Get-Content -Path $file.FullName -Encoding UTF8

    # Find frontmatter boundaries (between first --- and second ---)
    $inFM = $false
    $tagIdx = -1

    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        if ($line.Trim() -eq '---') {
            if (-not $inFM) {
                $inFM = $true
            } else {
                break  # End of frontmatter
            }
        }
        elseif ($inFM -and $line -match '^tags:\s*') {
            $tagIdx = $i
        }
    }

    if ($tagIdx -lt 0) {
        $stats.Skipped_NoTags++
        continue
    }

    $tagLine = $lines[$tagIdx]
    $tagValue = ($tagLine -replace '^tags:\s*', '').Trim()

    if (-not $tagValue) {
        $stats.Skipped_NoTags++
        continue
    }

    # Already YAML flow sequence? e.g. tags: [a, b] or tags: ["a", "b"]
    if ($tagValue -match '^\s*\[.*\]\s*$') {
        $stats.Skipped_AlreadyArray++
        continue
    }

    # Determine format and transform
    $newValue = $null
    $format = ""

    if ($tagValue -match ',') {
        # Comma-separated: split, trim, wrap
        $tags = $tagValue -split ',\s*' | Where-Object { $_.Trim() -ne '' } | ForEach-Object { $_.Trim() }
        $newValue = "[" + ($tags -join ", ") + "]"
        $format = "comma"
        $stats.Converted_Comma++
    }
    elseif ($tagValue -match '\s') {
        # Space-separated: split on whitespace, wrap
        $tags = $tagValue -split '\s+' | Where-Object { $_ -ne '' }
        $newValue = "[" + ($tags -join ", ") + "]"
        $format = "space"
        $stats.Converted_Space++
    }
    else {
        # Single word
        $newValue = "[$tagValue]"
        $format = "single"
        $stats.Converted_Single++
    }

    if ($newValue) {
        $lines[$tagIdx] = "tags: $newValue"

        if (-not $WhatIf) {
            $utf8NoBom = New-Object System.Text.UTF8Encoding $false
            [System.IO.File]::WriteAllLines($file.FullName, $lines, $utf8NoBom)
        }

        Write-Host ("[{0}] {1}" -f $format.PadRight(7), $file.FullName)
    }
}

Write-Host ""
Write-Host "=== Summary ==="
Write-Host ("Total:                 {0}" -f $stats.Total)
Write-Host ("Skipped (no tags):     {0}" -f $stats.Skipped_NoTags)
Write-Host ("Skipped (YAML array):  {0}" -f $stats.Skipped_AlreadyArray)
Write-Host ("Converted (comma):     {0}" -f $stats.Converted_Comma)
Write-Host ("Converted (single):    {0}" -f $stats.Converted_Single)
Write-Host ("Converted (space):     {0}" -f $stats.Converted_Space)

$converted = $stats.Converted_Comma + $stats.Converted_Single + $stats.Converted_Space
Write-Host ("---")
Write-Host ("Total converted:       {0}" -f $converted)

if ($WhatIf) {
    Write-Host ""
    Write-Host "DRY RUN - no files were modified. Remove -WhatIf to apply changes."
}
