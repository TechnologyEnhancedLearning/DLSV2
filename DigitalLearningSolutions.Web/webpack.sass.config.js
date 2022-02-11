const glob = require('glob');
const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

const entry = {};
glob.sync('./Styles/**/*.scss', {}).forEach((file) => {
  const name = file.replace('./Styles/', '').replace('.scss', '');
  entry[name] = file;
});

const config = {
  entry,
  mode: 'production',
  output: {
    path: path.join(__dirname, './wwwroot/css/'),
    filename: '[name].css',
    devtoolModuleFilenameTemplate: (info) => path.join('../../', info.resourcePath),
  },
  module: {
    rules: [
      {
        test: /\.s[ac]ss$/i,
        use: [
          MiniCssExtractPlugin.loader,
          'css-loader',
          {
            loader: 'sass-loader',
            options: {
              // Prefer `dart-sass`
              implementation: require('sass'),
              sassOptions: {
                quietDeps: true,
              },
            },
          },
        ],
      },
    ],
  },
  plugins: [
    new MiniCssExtractPlugin(),
  ],
  resolve: {
    extensions: ['.scss'],
  },
};

module.exports = config;
