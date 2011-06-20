Properties { 
	$solution_name = "ChannelAdvisorAccess"
  
    $base_dir  = resolve-path .
	
	# Folder structure:
	# \build - Contains all code during the build process
	# \build\artifacts - Contains all files during intermidiate bulid process
	# \build\output - Contains the final result of the build process
	# \release - Contains final release files for upload
	# \release\archive - Contains files archived from the previous builds
	# \src - Contains all source code
	# \tools - Contains all 3rd party tools (including psake)
	
    $build_dir = "$base_dir\build"
	$build_artifacts_dir = "$build_dir\artifacts"
	$build_output_dir = "$build_dir\output"
    $release_dir = "$base_dir\release"
	$archive_dir = "$release_dir\archive"
	
	$src_dir = "$base_dir\src"
    $solution_file = "$src_dir\$solution_name.sln"
	
	$tools_dir = "$base_dir\tools"
} 

FormatTaskName { Write-Host (("-"*25) + "[ $taskName ]" + ("-"*25)) -ForegroundColor Green }

Task Default -depends Release

Task Clean -depends Archive { 
	remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue | Out-Null
	Exec { msbuild "$solution_file" /t:Clean /p:Configuration=$configuration /v:quiet } 
}

Task Archive {
	New-Item $release_dir -ItemType directory -Force | Out-Null
	New-Item $archive_dir -ItemType directory -Force | Out-Null
	Move-Item -Path $release_dir\*.* -Destination $archive_dir -Force
}

Task Init -depends Clean { 
    New-Item $build_dir -itemType directory | Out-Null
    New-Item $build_artifacts_dir -itemType directory | Out-Null
    New-Item $build_output_dir -itemType directory | Out-Null
}

Task Compile -depends Init {
	Exec { msbuild "$solution_file" /t:Build /p:Configuration=$configuration /v:minimal /p:OutDir="$build_artifacts_dir\" }
}

Task Build -depends Compile {
   Copy-Item -Path $build_artifacts_dir\* -Destination $build_output_dir -Recurse
}

Task Check-Commit {
	$repo_status = & git status $base_dir | Out-String
	
	if( $repo_status.IndexOf( "nothing to commit (working directory clean)", [StringComparison]::InvariantCultureIgnoreCase ) -eq -1 )
	{
		Write-Host "Repository Status:" -ForegroundColor Yellow
		Write-Host $repo_status -ForegroundColor Yellow
		throw "Solution is not committed. Aborting..."
	}
}

Task Zip-Release {
	$release_date = [DateTime]::UtcNow.ToString( "yyyyMMddTHHmm" );
	$release_zip_file = "$release_dir\$solution_name.$release_date.zip"
	
	Write-Host "Zipping release to: " $release_zip_file
	Write-7Zip -OutputPath $release_zip_file -Path $build_output_dir\* -Switches "-mx9"
}

Task Release -depends Check-Commit, Build, Zip-Release