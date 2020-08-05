const glob = require('glob');
const path = require('path');
const ForkTsCheckerWebpackPlugin = require('fork-ts-checker-webpack-plugin');

const entry = {};
glob.sync('./Scripts/**/*.ts',
  {
    ignore: [
      './Scripts/**/*.d.ts',
      './Scripts/helpers/**/*.ts',
      './Scripts/spec/**/*.*',
    ],
  }).forEach((file) => {
  const name = file.replace('./Scripts/', '').replace('.ts', '');
  entry[name] = file;
});

const config = {
  entry,
  mode: 'production',
  output: {
    path: path.join(__dirname, './wwwroot/js/'),
    filename: '[name].js',
    devtoolModuleFilenameTemplate: (info) => path.join('../../', info.resourcePath),
  },
  module: {
    rules: [
      {
        test: /\.[j,t]s$/,
        loader: 'babel-loader',
        options: {
          presets: [
            ['@babel/preset-env',
              {
                targets: {
                  ie: '11',
                },
              }],
            ['@babel/preset-typescript',
              {
                onlyRemoveTypeImports: true,
              },
            ],
          ],
          plugins: [
            'transform-es2015-arrow-functions',
          ],
        },
      },
    ],
  },
  plugins: [
    new ForkTsCheckerWebpackPlugin(),
  ],
  resolve: {
    extensions: ['.ts', '.js'],
  },
};

module.exports = config;
