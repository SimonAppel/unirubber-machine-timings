#!/bin/bash

echo "Building Master Machine"
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=false

echo "Compressing Master Machine Build"
zip ./builds/MasterBuild.zip ./MasterMachine/bin/Release/net8.0/win-x64/publish/*