module.exports = {
    env: {
        browser: true,
        es2020: true,
    },
    extends: [
        'airbnb-base',
    ],
    parser: '@typescript-eslint/parser',
    parserOptions: {
        ecmaVersion: 11,
        sourceType: 'module',
    },
    settings: {
      'import/resolver': 'webpack'
    },
    plugins: [
        '@typescript-eslint',
    ],

    rules: {
      "import/extensions": [
        'error',
        'ignorePackages',
        {
          "js": 'never',
          "jsx": 'never',
          "ts": 'never',
          "tsx": 'never'
        }
      ]
    }
};
