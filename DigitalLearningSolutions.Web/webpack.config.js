const glob = require('glob');
const path = require('path');
const ForkTsCheckerWebpackPlugin = require('fork-ts-checker-webpack-plugin');

const entry = {};
glob.sync(
  './Scripts/**/*.ts',
  {
    ignore: [
      './Scripts/**/*.d.ts',
      './Scripts/helpers/**/*.ts',
      './Scripts/spec/**/*.*',
    ],
  },
).forEach((file) => {
  const name = file.replace('./Scripts/', '').replace('.ts', '');
  entry[name] = file;
});

const config = {
  entry,
  mode: 'production',
  target: ['web', 'es5'],
  output: {
    path: path.join(__dirname, './wwwroot/js/'),
    filename: '[name].js',
    devtoolModuleFilenameTemplate: (info) => path.join('../../', info.resourcePath),
  },
  module: {
    rules: [
      {
        test: /\.[j,t]s$/,
        exclude: [
          /\bcore-js\b/,
        ],
        loader: 'babel-loader',
        options: {
          presets: [
            ['@babel/preset-env',
              {
                targets: {
                  ie: '11',
                },
                corejs: '3',
                useBuiltIns: 'entry',
              },
            ],
            ['@babel/preset-typescript',
              {
                onlyRemoveTypeImports: true,
              },
            ],
          ],
        },
      },
    ],
  },
  plugins: [
    new ForkTsCheckerWebpackPlugin({
      typescript: {
        diagnosticOptions: {
          semantic: true,
          syntactic: true,
        },
        mode: 'write-references',
      },
      issue: {
        exclude: [
          { file: 'node_modules/**' },
          { file: '**/*.spec.ts' },
        ],
      },
    }),
  ],
  resolve: {
    extensions: ['.ts', '.js'],
  },
};

module.exports = config;
