const glob = require("glob");
const config = require("./webpack.sass.config");

config.mode = "development";
config.watch = true;

module.exports = config;
