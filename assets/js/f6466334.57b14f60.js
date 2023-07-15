"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[12],{3905:(e,t,r)=>{r.d(t,{Zo:()=>c,kt:()=>f});var n=r(7294);function a(e,t,r){return t in e?Object.defineProperty(e,t,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[t]=r,e}function o(e,t){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),r.push.apply(r,n)}return r}function l(e){for(var t=1;t<arguments.length;t++){var r=null!=arguments[t]?arguments[t]:{};t%2?o(Object(r),!0).forEach((function(t){a(e,t,r[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):o(Object(r)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(r,t))}))}return e}function i(e,t){if(null==e)return{};var r,n,a=function(e,t){if(null==e)return{};var r,n,a={},o=Object.keys(e);for(n=0;n<o.length;n++)r=o[n],t.indexOf(r)>=0||(a[r]=e[r]);return a}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(n=0;n<o.length;n++)r=o[n],t.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(a[r]=e[r])}return a}var s=n.createContext({}),p=function(e){var t=n.useContext(s),r=t;return e&&(r="function"==typeof e?e(t):l(l({},t),e)),r},c=function(e){var t=p(e.components);return n.createElement(s.Provider,{value:t},e.children)},u={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},m=n.forwardRef((function(e,t){var r=e.components,a=e.mdxType,o=e.originalType,s=e.parentName,c=i(e,["components","mdxType","originalType","parentName"]),m=p(r),f=a,d=m["".concat(s,".").concat(f)]||m[f]||u[f]||o;return r?n.createElement(d,l(l({ref:t},c),{},{components:r})):n.createElement(d,l({ref:t},c))}));function f(e,t){var r=arguments,a=t&&t.mdxType;if("string"==typeof e||a){var o=r.length,l=new Array(o);l[0]=m;var i={};for(var s in t)hasOwnProperty.call(t,s)&&(i[s]=t[s]);i.originalType=e,i.mdxType="string"==typeof e?e:a,l[1]=i;for(var p=2;p<o;p++)l[p]=r[p];return n.createElement.apply(null,l)}return n.createElement.apply(null,r)}m.displayName="MDXCreateElement"},8835:(e,t,r)=>{r.r(t),r.d(t,{assets:()=>c,contentTitle:()=>s,default:()=>f,frontMatter:()=>i,metadata:()=>p,toc:()=>u});var n=r(7462),a=r(3366),o=(r(7294),r(3905)),l=["components"],i={},s=void 0,p={unversionedId:"modules/tf2",id:"modules/tf2",title:"tf2",description:"Commands for interacting with the Teamwork.TF API for Team Fortress 2.",source:"@site/docs/modules/tf2.md",sourceDirName:"modules",slug:"/modules/tf2",permalink:"/FlawBOT/modules/tf2",draft:!1,editUrl:"https://github.com/CriticalFlaw/FlawBOT/tree/master/docs/modules/tf2.md",tags:[],version:"current",lastUpdatedAt:1689389933,formattedLastUpdatedAt:"Jul 15, 2023",frontMatter:{},sidebar:"jsonSideBar",previous:{title:"steam",permalink:"/FlawBOT/modules/steam"},next:{title:"user",permalink:"/FlawBOT/modules/user"}},c={},u=[{value:"creator",id:"creator",level:3},{value:"item",id:"item",level:3},{value:"map",id:"map",level:3},{value:"news",id:"news",level:3},{value:"search",id:"search",level:3},{value:"server",id:"server",level:3},{value:"server-list",id:"server-list",level:3}],m={toc:u};function f(e){var t=e.components,r=(0,a.Z)(e,l);return(0,o.kt)("wrapper",(0,n.Z)({},m,r,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("p",null,"Commands for interacting with the Teamwork.TF API for Team Fortress 2."),(0,o.kt)("h3",{id:"creator"},"creator"),(0,o.kt)("p",null,"Returns a community creator profile from teamwork.tf"),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/tf2 creator criticalflaw\n")),(0,o.kt)("h3",{id:"item"},"item"),(0,o.kt)("p",null,"Returns an item from the latest TF2 item schema."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/tf2 item natasha\n")),(0,o.kt)("h3",{id:"map"},"map"),(0,o.kt)("p",null,"Returns map information from teamwork.tf"),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/tf2 map pl_upward\n")),(0,o.kt)("h3",{id:"news"},"news"),(0,o.kt)("p",null,"Returns the latest news article from teamwork.tf"),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/tf2 news\n")),(0,o.kt)("h3",{id:"search"},"search"),(0,o.kt)("p",null,"Returns a game server with given ip address."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/tf2 search 164.132.233.16\n")),(0,o.kt)("h3",{id:"server"},"server"),(0,o.kt)("p",null,"Returns a list of servers for a given game-mode."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/tf2 server payload\n")),(0,o.kt)("h3",{id:"server-list"},"server-list"),(0,o.kt)("p",null,"Returns a curated list of servers."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/tf2 server-list\n")))}f.isMDXComponent=!0}}]);