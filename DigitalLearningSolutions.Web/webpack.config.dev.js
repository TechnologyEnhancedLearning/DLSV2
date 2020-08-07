const glob = require("glob");
const config = require("./webpack.config");

config.mode = "development";
config.devtool = "source-map";
config.watch = true;

module.exports = config;
