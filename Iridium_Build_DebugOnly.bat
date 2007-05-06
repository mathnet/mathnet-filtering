@echo off
cls

msbuild Iridium.sln /p:Configuration=Debug /t:Build

pause