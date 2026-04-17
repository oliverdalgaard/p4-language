@echo off
cd .\Matilda
.\Coco.exe Matilda.cs.ATG -namespace Matilda
dotnet build matilda.csproj -c Release