<#
.Synopsis
	Build script (https://github.com/nightroman/Invoke-Build)

.Description
	How to use this script and build the module:

	Get the utility script Invoke-Build.ps1:
	https://github.com/nightroman/Invoke-Build

	Copy it to the path. Set location to this directory. Build:
	PS> Invoke-Build Build

	This command builds the module and installs it to the $ModuleRoot which is
	the working location of the module. The build fails if the module is
	currently in use. Ensure it is not and then repeat.

	The build task Help fails if the help builder Helps is not installed.
	Ignore this or better get and use the script (it is really easy):
	https://github.com/nightroman/Helps
#>

param
(
	$Configuration = 'Release',
	$logfile = $null
)

$project_name = "ChannelAdvisorAccess"

# Folder structure:
# \build - Contains all code during the build process
# \build\artifacts - Contains all files during intermidiate bulid process
# \build\output - Contains the final result of the build process
# \release - Contains final release files for upload
# \release\archive - Contains files archived from the previous builds
# \src - Contains all source code
$build_dir = "$BuildRoot\build"
$build_artifacts_dir = "$build_dir\artifacts"
$build_output_dir = "$build_dir\output"
$release_dir = "$BuildRoot\release"
$archive_dir = "$release_dir\archive"

$src_dir = "$BuildRoot\src"
$solution_file = "$src_dir\ChannelAdvisorAccess.sln"
	
# Use MSBuild.
use Framework\v4.0.30319 MSBuild

task Clean { 
	exec { MSBuild "$solution_file" /t:Clean /p:Configuration=$configuration /v:quiet } 
	Remove-Item -force -recurse $build_dir -ErrorAction SilentlyContinue | Out-Null
}

task Init Clean, { 
    New-Item $build_dir -itemType directory | Out-Null
    New-Item $build_artifacts_dir -itemType directory | Out-Null
    New-Item $build_output_dir -itemType directory | Out-Null
}

task Build {
	exec { MSBuild "$solution_file" /t:Build /p:Configuration=$configuration /v:minimal /p:OutDir="$build_artifacts_dir\" }
}

task Package  {
	New-Item $build_output_dir\ChannelAdvisorAccess\lib\net40 -itemType directory -force | Out-Null
	Copy-Item $build_artifacts_dir\ChannelAdvisorAccess.??? $build_output_dir\ChannelAdvisorAccess\lib\net40 -PassThru |% { Write-Host "Copied " $_.FullName }
	Copy-Item $build_artifacts_dir\Zayko.Finance.CurrencyConverter.??? $build_output_dir\ChannelAdvisorAccess\lib\net40 -PassThru |% { Write-Host "Copied " $_.FullName }
}

# Set $script:Version = assembly version
task Version {
	assert (( Get-Item $build_artifacts_dir\ChannelAdvisorAccess.dll ).VersionInfo.FileVersion -match '^(\d+\.\d+\.\d+)')
	$script:Version = $matches[1]
}

task Archive {
	New-Item $release_dir -ItemType directory -Force | Out-Null
	New-Item $archive_dir -ItemType directory -Force | Out-Null
	Move-Item -Path $release_dir\*.* -Destination $archive_dir
}

task Zip Version, {
	$release_zip_file = "$release_dir\$project_name.$Version.zip"
	
	Write-Host "Zipping release to: " $release_zip_file
	
	exec { & 7za.exe a $release_zip_file $build_output_dir\ChannelAdvisorAccess\lib\net40\* -mx9 }
}

task NuGet Package, Version, {

	Write-Host ================= Preparing ChannelAdvisorAccess Nuget package =================
	$text = "ChannelAdvisor webservices API wrapper."
	# nuspec
	Set-Content $build_output_dir\ChannelAdvisorAccess\ChannelAdvisorAccess.nuspec @"
<?xml version="1.0"?>
<package>
	<metadata>
		<id>ChannelAdvisorAccess</id>
		<version>$Version-alpha7</version>
		<authors>Slav Ivanyuk</authors>
		<owners>Slav Ivanyuk</owners>
		<projectUrl>https://github.com/slav/ChannelAdvisorAccess</projectUrl>
		<licenseUrl>https://raw.github.com/slav/ChannelAdvisorAccess/master/License.txt</licenseUrl>
		<requireLicenseAcceptance>false</requireLicenseAcceptance>
		<copyright>Copyright (C) Agile Harbor, LLC 2012</copyright>
		<summary>$text</summary>
		<description>$text</description>
		<tags>ChannelAdvisor</tags>
		<dependencies> 
			<dependency id="Netco" version="1.1.0" />
		</dependencies>
	</metadata>
</package>
"@
	# pack
	exec { NuGet pack $build_output_dir\ChannelAdvisorAccess\ChannelAdvisorAccess.nuspec -Output $build_dir }
	
	$pushChannelAdvisorAccess = Read-Host 'Push ChannelAdvisorAccess ' $Version ' to NuGet? (Y/N)'
	Write-Host $pushChannelAdvisorAccess
	if( $pushChannelAdvisorAccess -eq "y" -or $pushChannelAdvisorAccess -eq "Y" )	{
		Get-ChildItem $build_dir\*.nupkg |% { exec { NuGet push  $_.FullName }}
	}
}

task . Init, Build, Package, Zip, NuGet


#///////////////////////////////////////////////////////////////////////////////////////////

function Enter-BuildScript {
	if($logfile) {
		if( $Host -and $Host.UI -and $Host.UI.RawUI ) {
			$rawUI = $Host.UI.RawUI
			$oldBufferSize = $rawUI.BufferSize
			$typeName = $oldBufferSize.GetType().FullName
			$newSize = New-Object $typeName (128, $oldBufferSize.Height)
			$rawUI.BufferSize = $newSize
		}
		
		$logfolder = Split-Path $logfile -Parent
		New-Item $logfolder -Type directory -Force  | Out-Null
		
		$transcribing = $true
		Start-Transcript $logfile
	}
}

function Exit-BuildScript {
	if( $transcribing ) {
		Write-Host @'

---------- Transcript Build Summary ----------

'@
		
		foreach($_ in $Result.AllTasks) {
			Write-Host ('{0,-16} {1} {2}:{3}' -f $_.Elapsed, $_.Name, $_.InvocationInfo.ScriptName, $_.InvocationInfo.ScriptLineNumber)
			if ($_.Error) {
				Write-Host -ForegroundColor Red (*Err* $_.Error $_.Error.InvocationInfo)
			}
		}
	
		if( $oldBufferSize -ne $null ) {
			$host.UI.RawUI.BufferSize = $oldBufferSize
		}

		Stop-Transcript
		
		Write-Host @'
		
***********************************************************
	
'@
	}
}