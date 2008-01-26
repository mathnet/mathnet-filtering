@echo off
cls

msbuild config/MathNet.Iridium.msbuild /t:CustomRelease

pause