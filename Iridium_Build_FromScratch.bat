@echo off
cls

msbuild config/MathNet.Iridium.msbuild /t:CustomRebuild

pause