# CondaLauncher
Launcher Exe for Python3 apps using Conda as backbone

Example build script:

```bash
#!/bin/bash

dir=$(cd -P -- "$(dirname -- "$0")" && pwd -P)
cd $dir

APPNAME=myapp
CL_VERSION=1.2

if [ ! -d CondaLauncher-$CL_VERSION ]; then
    curl -L https://github.com/eblade/CondaLauncher/archive/v$CL_VERSION.tar.gz | tar -xz
fi

if [ -d build ]; then
    rm -r build
fi

./CondaLauncher-$CL_VERSION/build.sh $APPNAME build

cp launcher.json build/
cp setup.py build/
cp -r mymodule build/
```
