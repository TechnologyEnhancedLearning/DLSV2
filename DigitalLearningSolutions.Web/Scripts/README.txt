All *.ts files in the Scripts/ directory are compiled to *.js files in wwwroot/js/ except for:

    *.d.ts typings files
    Any files in the helpers/ directory

Compiled files preserve their path, e.g. Scripts/home/index.ts compiles to wwwroot/js/home/index.js

The build:webpack and dev:webpack NPM tasks are responsible for the compilation.

In order for the NPM tasks to be run automatically when the project is opened, you need to have the NPMTaskRunner extension installed in Visual Studio:
https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner
