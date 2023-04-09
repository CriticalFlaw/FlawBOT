// @ts-check

/** @type {import("@docusaurus/types").Config} */
const config = {
    title: "FlawBOT",
    tagline: "Multipurpose Discord bot written in C# using DSharpPlus.",
    url: "https://criticalflaw.ca/",
    baseUrl: "/FlawBOT/",
    favicon: "https://raw.githubusercontent.com/CriticalFlaw/FlawBOT/master/docs/resources/favicon.ico",

    // GitHub pages deployment config.
    // If you aren"t using GitHub pages, you don"t need these.
    organizationName: "CriticalFlaw", // Usually your GitHub org/user name.
    projectName: "FlawBOT", // Usually your repo name.
    deploymentBranch: "gh-pages",

    // Even if you don"t use internalization, you can use this field to set useful
    // metadata like html lang. For example, if your site is Chinese, you may want
    // to replace "en" with "zh-Hans".
    i18n: {
        defaultLocale: "en",
        locales: ["en"],
    },

    presets: [
        [
            "classic",
            /** @type {import("@docusaurus/preset-classic").Options} */
            ({
                docs: {
                    routeBasePath: "/",
                    sidebarPath: require.resolve("./docs/sidebars.js"),

                    // Please change this to your repo.
                    // Remove this to remove the "edit this page" links.
                    editUrl: "https://github.com/CriticalFlaw/FlawBOT/tree/master",

                    showLastUpdateTime: true
                },
                pages: false,
                blog: false,
                theme: {
                    customCss: require.resolve("./docs/resources/custom.css"),
                }
            })
        ]
    ],

    themes: [
        [
            require.resolve("@easyops-cn/docusaurus-search-local"),
            {
                docsRouteBasePath: "/"
            }
        ]
    ],

    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    themeConfig: {
        navbar: {
            style: "dark",
            title: "FlawBOT",
            logo: {
                alt: "FlawBOT Logo",
                src: "https://raw.githubusercontent.com/CriticalFlaw/FlawBOT/master/docs/resources/favicon.ico",
            },
            items: [
                {
                    label: "Modules",
                    type: "doc",
                    docId: "modules/bot",
                    position: "left"
                },
                {
                    label: "Deployment",
                    type: "doc",
                    docId: "deploy",
                    position: "left"
                },
                {
                    label: "Lavalink",
                    type: "doc",
                    docId: "lavalink",
                    position: "left"
                },
                {
                    label: "Tokens",
                    type: "doc",
                    docId: "tokens",
                    position: "left"
                },
                {
                    label: "Blog",
                    href: "https://criticalflaw.ca/"
                },
                {
                    href: "https://github.com/CriticalFlaw/FlawBOT/",
                    position: "right",
                    className: "header-github-link"
                }
            ]
        },
        footer: {
            links: [
                {
                    title: "Community",
                    items: [
                        {
                            label: "Github",
                            href: "https://github.com/CriticalFlaw/FlawBOT",
                        },
                        {
                            label: "Twitter",
                            href: "https://twitter.com/CriticalFlaw_",
                        },
                        {
                            label: "Discord",
                            href: "https://discord.gg/hTdtK9vBhE",
                        }
                    ]
                }
            ],
            copyright: `Copyright © ${new Date().getFullYear()} My Project, Inc. Built with Docusaurus.`
        }
    }
}

module.exports = config
