#!/bin/bash

echo "Building Slave Machine"
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=false

echo "Compressing Slave Machine Build"
zip ./builds/SlaveBuild.zip ./SlaveMachine/bin/Release/net8.0/win-x64/publish/*