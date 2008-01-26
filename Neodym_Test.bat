@echo off
cls

msbuild config/MathNet.Neodym.msbuild /t:BuildVerificationUnitTests

pause