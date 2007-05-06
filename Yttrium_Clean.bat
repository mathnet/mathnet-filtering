@echo off
cls

msbuild Yttrium.sln /p:Configuration=Release /t:Clean
msbuild Yttrium.sln /p:Configuration=Debug /t:Clean

pause