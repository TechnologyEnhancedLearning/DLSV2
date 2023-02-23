module.exports = {
    env: {
        test: {
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
};
