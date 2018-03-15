#!/bin/bash

export EXENAME=${1:-Launch}
OUTPUTDIR="$(realpath ${2:-build})"

dir=$(cd -P -- "$(dirname -- "$0")" && pwd -P)
cd $dir

if [ -d bin ]; then
    rm -r bin
fi

dotnet publish -c Release -r win10-x64 -o "$OUTPUTDIR"
