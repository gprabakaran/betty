const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = {
    mode: 'development',
    entry: './src/index.ts',
    module: {
        rules: [
            {
                test: /\.ts$/,
                use: 'ts-loader',
                exclude: [
                    /node_modules/,
                    /webpack.*/
                ]
            }
        ]
    },
    resolve: {
        extensions: [ '.ts', '.js']
    },
    output: {
        filename: 'frontend.js',
        path: path.resolve(__dirname, '../wwwroot')
    },
    plugins: [
        new HtmlWebpackPlugin({
            title: 'Improbable spaceflight inc.'
        })
    ]
};