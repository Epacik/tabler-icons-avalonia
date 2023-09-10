﻿param (
    [Parameter()] [string] $ObjPath
)

$intermediateOutput = Join-Path $ObjPath "TablerIcons";
$assetsPath = Join-Path $PSScriptRoot "Assets" "TablerIcons";
$iconsPath = Join-Path $PSScriptRoot ".." ".." ".." ".." "icons";

$generatedFilePath = Join-Path $intermediateOutput "TablerIcons.g.cs";

# exit if file exists

if ((Test-Path -Path $generatedFilePath -PathType Leaf)) {
    exit 0;
}

# make sure that directories exist
if ((Test-Path -Path $intermediateOutput -PathType Container) -ne $true) {
    New-Item -Path $intermediateOutput -ItemType Directory -Force;
}

if ((Test-Path -Path $assetsPath -PathType Container) -ne $true) {
    New-Item -Path $assetsPath -ItemType Directory -Force;
}

$maxThreads = [System.Math]::Max([System.Environment]::ProcessorCount - 1, 1);

# copy all icons to use as assets

$copyMt = {
    param($info)

    $files = Get-ChildItem $info.SourcePath;
    $targetPath = $info.TargetPath;
    $threadNumber = $info.ThreadNumber;
    $totalThreads = $info.TotalThreads;

    for ($i = $threadNumber; $i -lt $files.Length; $i = $i + $totalThreads){
        $file = $files[$i];

        $src = $file.FullName;
        $tgt = Join-Path $targetPath ($file.Name);

        try {
            if ((Test-Path -Path $targetPath -PathType Leaf) -ne $true) {
                continue;
            }
        }
        catch {
            continue;
        }

        Write-Output "Copying '$src' to '$tgt'";

        Copy-Item $src -Destination $tgt -Force
    }
}

$jobs = [System.Collections.Generic.List[object]]::new();
for ($i = 0; $i -lt $maxThreads; $i++) {
    $info = [PSCustomObject]@{
        SourcePath = $iconsPath;
        TargetPath = $assetsPath;
        ThreadNumber = $i;
        TotalThreads = $maxThreads;
    };
    $job = Start-Job -ScriptBlock $copyMt -ArgumentList $info;
    Write-Output $job;
    $jobs.Add($job);
}

Wait-Job $jobs

foreach ($job in $jobs)
{
    foreach ($err in $job.ChildJobs[0].Error)
    {
        Write-Error $err;
    }
}

function ConvertTo-PascalCase
{
    [OutputType('System.String')]
    param(
        [Parameter(Position=0)]
        [string] $Value
    )

    # https://devblogs.microsoft.com/oldnewthing/20190909-00/?p=102844
    return [regex]::replace($Value.ToLower(), '(^|[_-])(.)', { $args[0].Groups[2].Value.ToUpper()})
}

$lines = "";

foreach ($file in (Get-ChildItem $assetsPath))
{
    $key = ConvertTo-PascalCase $file.BaseName;
    $value = $file.BaseName;

    $lines += "            [global::TablerIcons.Avalonia.Value(`"$value`")]Icon$key,`n";
}

$out = @"
// <autogenerated />
// THIS FILE IS AUTOGENERATED
// CHANGES WITHIN WILL BE LOST

namespace TablerIcons.Avalonia {

    [global::System.CodeDom.Compiler.GeneratedCode("custom", "1.0.0")]
    public enum Icons {
$lines
    }
}
"@

Out-File -FilePath $generatedFilePath -InputObject $out
