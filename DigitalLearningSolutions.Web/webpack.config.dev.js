const glob = require("glob");
const config = require("./webpack.config");

config.mode = "development";
config.module.rules.find(rule => rule.loader === "ts-loader").options.configFile = "tsconfig.dev.json";
config.devtool = "source-map";
config.watch = true;

glob.sync("./Scripts/spec/**/*.ts", {})
    .forEach(file => {
        const name = file.replace("./Scripts/", "").replace(".ts", "");
        config.entry[name] = file;
    });

module.exports = config;
