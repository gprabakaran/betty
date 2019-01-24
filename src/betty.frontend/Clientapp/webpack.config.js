const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = {
    mode: 'development',
    entry: './src/index.js',
    devtool: 'eval-source-map',
    module: {
        rules: [
            {
                test: /\.js$/,
                use: 'babel-loader',
                exclude: [
                    /node_modules/
                ]
            },
            {
                test: /\.css$/,
                use: ['style-loader', 'css-loader']
            },
            {
                test: /phaser\-input.js$/,
                use: ['exports-loader?PhaserInput=PhaserInput']
            }
        ]
    },
    resolve: {
        extensions: ['.js'],
        alias: {
            'phaser-input': path.join(__dirname, 'node_modules/@orange-games/phaser-input/build/phaser-input.js')
        }
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