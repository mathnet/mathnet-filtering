@echo off
cls

msbuild Iridium.sln /p:Configuration=Release /t:Build
msbuild Iridium.sln /p:Configuration=Debug /t:Build

pause