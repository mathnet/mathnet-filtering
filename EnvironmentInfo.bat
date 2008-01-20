@echo off
cls

msbuild config\Environment.msbuild /t:PrintEnvironmentInfo

pause