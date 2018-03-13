# CondaLauncher
Launcher Exe for Python3 apps using Conda as backbone.

# Description

CondaLauncher provides a C# .NET Core program that can be build with a custom filename. It will read a file `launcher.json` which contains anaconda and pip dependencies. The first time it runs it will set up a conda environment with those dependencies and then launch the python module specified.

# Usage

Example build script for your python project:

```bash
#!/bin/bash

dir=$(cd -P -- "$(dirname -- "$0")" && pwd -P)
cd $dir

APP_NAME=myapp
CL_VERSION=1.2

if [ ! -d CondaLauncher-$CL_VERSION ]; then
    curl -L https://github.com/eblade/CondaLauncher/archive/v$CL_VERSION.tar.gz | tar -xz
fi

if [ -d build ]; then
    rm -r build
fi

./CondaLauncher-$CL_VERSION/build.sh $APP_NAME build

cp launcher.json build/
```

Example json file can be found [here](launcher.json).

# Flags

The launcher can be run with the following flags:

* `--upgrade`: Run the pip install command even if the environment exists, and with the `--upgrade` flag.
* `--debug`: Do not hide the command prompt (which shows `stdout` and `stderr`).

# Requirements

Running the build script requires .NET Core 2.0 SDK (or later), and running the app requires the same runtime. You can fiddle with the build script to get it to publish a stand-alone instead, but that takes up much more disk space.
