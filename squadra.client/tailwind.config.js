module.exports = {
    content: [
        "./src/**/*.{js,jsx,ts,tsx,html}"
    ],
    // Safelist dla niestandardowych klas generowanych dynamicznie lub z bracket notation
    safelist: [
        { pattern: /^text-\[.*]$/ },
        { pattern: /^p-\[.*]$/ },
        { pattern: /^w-\[.*]$/ },
        { pattern: /^h-\[.*]$/ },
        { pattern: /^rounded-\[.*]$/ },
        { pattern: /^border-\[.*]$/ },
        { pattern: /^bg-\[.*]$/ },
        // konkretne utility których używasz
        'object-cover',
        'w-fit',
        'my-10',
        'mx-auto',
        'rounded-full',
    ],
    theme: {
        extend: {
            fontFamily: {
                sans: ['Epilogue', 'sans-serif'],
                orbitron: ['Orbitron', 'sans-serif'],
                'orbitron-bold': ['Orbitron-Bold', 'sans-serif'],
                epilogue: ['Epilogue', 'sans-serif'],
                'epilogue-semi-bold': ['Epilogue-SemiBold', 'sans-serif'],
            },
        },
    },
    plugins: [],
}