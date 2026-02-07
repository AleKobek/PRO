module.exports = {
    plugins: [
        require('postcss-import'),
        // Use tailwindcss/nesting wrapper with postcss-nesting for nesting support
        require('tailwindcss/nesting')(require('postcss-nesting')),
        require('tailwindcss'),
        require('autoprefixer'),
    ],
}