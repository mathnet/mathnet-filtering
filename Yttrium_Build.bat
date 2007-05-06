@echo off
cls

msbuild Yttrium.sln /p:Configuration=Release /t:Build
msbuild Yttrium.sln /p:Configuration=Debug /t:Build

pause