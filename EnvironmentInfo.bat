@echo off
cls

msbuild Environment.msbuild /t:PrintEnvironmentInfo

pause