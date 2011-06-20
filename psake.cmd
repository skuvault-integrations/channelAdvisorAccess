@echo off
IF %1.==. GOTO No1

set LOGDIR=%~dp0log\
set LOGFILE=%LOGDIR%%1.log

powershell -NoProfile -ExecutionPolicy unrestricted -Command "& '%~dp0\tools\psake\psake.ps1' -taskList %1 -configuration %1 -logfile ""%LOGFILE%""" -framework 4.0

PAUSE
GOTO End1

:No1
	echo psake.cmd cannot be run directly.
	echo Execute other cmd file instead (for example release.cmd).
	pause
GOTO End1

:End1