@echo off
cls

msbuild Iridium.sln /p:Configuration=Release /t:Clean
msbuild Iridium.sln /p:Configuration=Debug /t:Clean

pause