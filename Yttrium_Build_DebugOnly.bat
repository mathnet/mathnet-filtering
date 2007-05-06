@echo off
cls

msbuild Yttrium.sln /p:Configuration=Debug /t:Build

pause