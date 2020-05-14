const config = require("./webpack.config");

config.mode = "development";
config.module.rules.find(rule => rule.loader === "ts-loader").options.configFile = "tsconfig.dev.json";
config.devtool = "source-map";
config.watch = true;

module.exports = config;
