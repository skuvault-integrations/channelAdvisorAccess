#Requires -Version 2.0

function Write-7Zip
{
<#
.SYNOPSIS
Archives files at the specified path using 7zip

.DESCRIPTION
This function can archive all specified files using 7zip.

.PARAMETER OutputPath
Resulting archive file name
Required

.PARAMETER Path
Path to the files to archive. Wildcard syntax is allowed.
Required

.PARAMETER Switches
Additional switches to customize archive. 

.EXAMPLE
Write-7Zip -Path $build_output_dir -OutputPath $release_dile -Method Zip

This example archives a file.
#>
  [CmdletBinding()]
  param(
		[Parameter(Position=0,Mandatory=1)][string]$OutputPath,
		[Parameter(Position=1,Mandatory=1)][string]$Path,
		[Parameter(Position=2,Mandatory=0)][string[]]$Switches = @()
	)
	
	$7zipPath = Join-Path $PSScriptRoot 7zip\7za.exe
	
	& $7zipPath a $OutputPath $Path $Switches $Switches | Out-Host
}

function CommitEx-Git
{
<#
.SYNOPSIS
Archives files at the specified path using 7zip

.DESCRIPTION
This function can archive all specified files using 7zip.
#>
	$gitExCommand = Join-Path $PSScriptRoot gitex.cmd
	Start-Process -FilePath GitExtensions.exe -ArgumentList commit -Wait -PassThru
}