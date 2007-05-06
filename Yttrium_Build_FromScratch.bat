@echo off
cls

msbuild Iridium.sln /p:Configuration=Release /t:Clean
msbuild Iridium.sln /p:Configuration=Debug /t:Clean

msbuild Yttrium.sln /p:Configuration=Release /t:Clean
msbuild Yttrium.sln /p:Configuration=Debug /t:Clean

msbuild Iridium.sln /p:Configuration=Release /t:Build
msbuild Iridium.sln /p:Configuration=Debug /t:Build

msbuild Yttrium.sln /p:Configuration=Release /t:Build
msbuild Yttrium.sln /p:Configuration=Debug /t:Build

pause