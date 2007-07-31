@echo off
cls

msbuild Neodym.sln /p:Configuration=Release /t:Build
msbuild Neodym.sln /p:Configuration=Debug /t:Build

pause