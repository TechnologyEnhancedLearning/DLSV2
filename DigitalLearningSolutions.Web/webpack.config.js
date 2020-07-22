const glob = require("glob");
const path = require("path");
const CopyPlugin = require("copy-webpack-plugin");

const entry = {};
glob.sync("./Scripts/**/*.ts",
  {
    ignore: [
      "./Scripts/**/*.d.ts",
      "./Scripts/helpers/**/*.ts"
    ]
  }
).forEach(file => {
  const name = file.replace("./Scripts/", "").replace(".ts", "");
  entry[name] = file;
});

const config = {
  entry: entry,
  mode: "production",
  output: {
    path: path.join(__dirname, "./wwwroot/js/"),
    filename: "[name].js",
    devtoolModuleFilenameTemplate: info => path.join("../../", info.resourcePath)
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        loader: "ts-loader",
        options: {
          configFile: "tsconfig.json"
        }
      }
    ]
  },
  resolve: {
    extensions: [".ts", ".js"]
  },
    plugins: [
      new CopyPlugin({
        patterns: [{
              from: "./node_modules/@fortawesome/fontawesome-free/webfonts",
              to: "../webfonts"
        }]
      })
  ]
};

module.exports = config;
