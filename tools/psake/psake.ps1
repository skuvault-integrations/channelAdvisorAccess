# Helper script for those who want to run psake without importing the module.
# Example:
# .\psake.ps1 "default.ps1" "BuildHelloWord" "4.0" 

# Must match parameter definitions for psake.psm1/invoke-psake 
# otherwise named parameter binding fails
param(
  [Parameter(Position=0,Mandatory=0)]
  [string]$buildFile = 'default.ps1',
  [Parameter(Position=1,Mandatory=0)]
  [string[]]$taskList = @(),
  [Parameter(Position=2,Mandatory=0)]
  [string]$configuration = "Release",
  [Parameter(Position=3,Mandatory=0)]
  [string]$framework = '3.5',
  [Parameter(Position=4,Mandatory=0)]
  [switch]$docs = $false,
  [Parameter(Position=5,Mandatory=0)]
  [string]$logfile=$null,
  [Parameter(Position=6,Mandatory=0)]
  [System.Collections.Hashtable]$parameters = @{},
  [Parameter(Position=7, Mandatory=0)]
  [System.Collections.Hashtable]$properties = @{}
)

begin {
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

process {
	remove-module psake -ea 'SilentlyContinue'
	$scriptPath = Split-Path -parent $MyInvocation.MyCommand.path
	import-module (join-path $scriptPath psake.psm1)
	if (-not(test-path $buildFile))
	{
	    $buildFile = (join-path $scriptPath $buildFile)
	}
	
	if( -not( $parameters.ContainsKey( "configuration" )))
	{
		$parameters.Add( "configuration", $configuration )
	}

    Write-Host ("="*25) "[ Tasks to run: " $taskList " ]" ("="*25) -ForegroundColor Cyan

	invoke-psake $buildFile $taskList $framework $docs $parameters $properties
	exit $lastexitcode
}

end {
	if( $oldBufferSize -ne $null ) {
		$host.UI.RawUI.BufferSize = $oldBufferSize
	}

	if( $transcribing ) {
		Stop-Transcript }
}