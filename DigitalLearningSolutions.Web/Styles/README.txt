All *.scss files in the Styles/ directory are compiled to *.css files in wwwroot/css/ except for:

    Files with names that start with an underscore (partials)

Compiled files preserve their path, e.g. Styles/home/index.scss compiles to wwwroot/css/home/index.css

The build:sass and dev:sass NPM tasks are responsible for the compilation.
