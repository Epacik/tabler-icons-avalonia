$iconsDir = "$PSScriptRoot/icons";
$sourceDir = "$PSScriptRoot/../../../../icons"

Write-Host "Copying icons"

if (-not (Test-Path -Path $iconsDir -PathType Container))
{
  new-item $iconsDir -ItemType Directory;
}

$filledIcons = Get-ChildItem "$sourceDir/filled"
$outlineIcons = Get-ChildItem "$sourceDir/outline"

$filledIcons | Foreach-Object -ThrottleLimit 5 -Parallel {
  #Action that will run in Parallel. Reference the current object via $PSItem and bring in outside variables with $USING:varname

  $dest = $USING:iconsDir;
  $name = $PSItem.Name.Replace($PSItem.Extension, "-filled" + $PSItem.Extension);

  try {
    Copy-Item -Path $PsItem.FullName -Destination "$dest/$name"
  }
  catch {
    # ignore
  }
}

$outlineIcons | Foreach-Object -ThrottleLimit 5 -Parallel {
  #Action that will run in Parallel. Reference the current object via $PSItem and bring in outside variables with $USING:varname

  $dest = $USING:iconsDir;
  $name = $PSItem.Name;

  try {
    Copy-Item -Path $PsItem.FullName -Destination "$dest/$name"
  }
  catch {
    # ignore
  }
}

exit 0;
