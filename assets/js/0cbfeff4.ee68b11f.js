"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[836],{3905:(e,t,r)=>{r.d(t,{Zo:()=>u,kt:()=>d});var n=r(7294);function a(e,t,r){return t in e?Object.defineProperty(e,t,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[t]=r,e}function o(e,t){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),r.push.apply(r,n)}return r}function l(e){for(var t=1;t<arguments.length;t++){var r=null!=arguments[t]?arguments[t]:{};t%2?o(Object(r),!0).forEach((function(t){a(e,t,r[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):o(Object(r)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(r,t))}))}return e}function i(e,t){if(null==e)return{};var r,n,a=function(e,t){if(null==e)return{};var r,n,a={},o=Object.keys(e);for(n=0;n<o.length;n++)r=o[n],t.indexOf(r)>=0||(a[r]=e[r]);return a}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(n=0;n<o.length;n++)r=o[n],t.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(a[r]=e[r])}return a}var c=n.createContext({}),s=function(e){var t=n.useContext(c),r=t;return e&&(r="function"==typeof e?e(t):l(l({},t),e)),r},u=function(e){var t=s(e.components);return n.createElement(c.Provider,{value:t},e.children)},m={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},p=n.forwardRef((function(e,t){var r=e.components,a=e.mdxType,o=e.originalType,c=e.parentName,u=i(e,["components","mdxType","originalType","parentName"]),p=s(r),d=a,f=p["".concat(c,".").concat(d)]||p[d]||m[d]||o;return r?n.createElement(f,l(l({ref:t},u),{},{components:r})):n.createElement(f,l({ref:t},u))}));function d(e,t){var r=arguments,a=t&&t.mdxType;if("string"==typeof e||a){var o=r.length,l=new Array(o);l[0]=p;var i={};for(var c in t)hasOwnProperty.call(t,c)&&(i[c]=t[c]);i.originalType=e,i.mdxType="string"==typeof e?e:a,l[1]=i;for(var s=2;s<o;s++)l[s]=r[s];return n.createElement.apply(null,l)}return n.createElement.apply(null,r)}p.displayName="MDXCreateElement"},3769:(e,t,r)=>{r.r(t),r.d(t,{assets:()=>u,contentTitle:()=>c,default:()=>d,frontMatter:()=>i,metadata:()=>s,toc:()=>m});var n=r(7462),a=r(3366),o=(r(7294),r(3905)),l=["components"],i={},c=void 0,s={unversionedId:"modules/steam",id:"modules/steam",title:"steam",description:"Commands for interacting with the Steam API.",source:"@site/docs/modules/steam.md",sourceDirName:"modules",slug:"/modules/steam",permalink:"/FlawBOT/modules/steam",draft:!1,editUrl:"https://github.com/CriticalFlaw/FlawBOT/tree/master/docs/modules/steam.md",tags:[],version:"current",lastUpdatedAt:1689389933,formattedLastUpdatedAt:"Jul 15, 2023",frontMatter:{},sidebar:"jsonSideBar",previous:{title:"speedrun",permalink:"/FlawBOT/modules/speedrun"},next:{title:"tf2",permalink:"/FlawBOT/modules/tf2"}},u={},m=[{value:"connect",id:"connect",level:3},{value:"game",id:"game",level:3},{value:"user",id:"user",level:3}],p={toc:m};function d(e){var t=e.components,r=(0,a.Z)(e,l);return(0,o.kt)("wrapper",(0,n.Z)({},p,r,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("p",null,"Commands for interacting with the Steam API."),(0,o.kt)("h3",{id:"connect"},"connect"),(0,o.kt)("p",null,"Formats a server connection string into a link."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/steam connect 192.168.0.200\n")),(0,o.kt)("h3",{id:"game"},"game"),(0,o.kt)("p",null,"Returns information on a Steam game."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/steam game Team Fortress 2\n")),(0,o.kt)("h3",{id:"user"},"user"),(0,o.kt)("p",null,"Returns information on a Steam user."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/steam user criticalflaw\n")))}d.isMDXComponent=!0}}]);