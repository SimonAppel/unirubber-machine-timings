#!/bin/bash

echo "Starting General build"

echo "Running Master Build File"
sh ./MasterMachine/build.sh

echo "Running Slave Build File"
sh ./SlaveMachine/build.sh

exit 0;