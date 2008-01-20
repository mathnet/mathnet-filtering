@echo off
cls

msbuild config\MathNet.Common.msbuild /t:ForceCreateNewPersonalTestKey

pause