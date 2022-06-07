const rules = {
  'import/extensions': [
    'error',
    'ignorePackages',
    {
      js: 'never',
      jsx: 'never',
      ts: 'never',
      tsx: 'never',
    },
  ],
  'linebreak-style': 'off',
  'import/no-extraneous-dependencies': ['error', { devDependencies: ['Scripts/spec/**/*.ts', './*.js'] }],
  'no-use-before-define': ['error', { functions: false }],
  'max-len': 'off',
};

module.exports = {
  env: {
    browser: true,
    es2020: true,
    jasmine: true,
  },
  settings: {
    'import/resolver': {
      node: {},
      webpack: {},
    },
  },
  extends: [
    'airbnb-base',
    'eslint:recommended',
  ],
  plugins: [
    'jasmine',
  ],
  rules,
  overrides: [
    {
      files: ['*.ts'],
      parser: '@typescript-eslint/parser',
      parserOptions: {
        ecmaVersion: 11,
        sourceType: 'module',
      },
      extends: [
        'airbnb-base',
        'plugin:@typescript-eslint/eslint-recommended',
        'plugin:@typescript-eslint/recommended',
      ],
      plugins: [
        '@typescript-eslint',
        'jasmine',
      ],
      rules,
    },
  ],
};
