"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[319],{3905:(e,t,r)=>{r.d(t,{Zo:()=>c,kt:()=>m});var n=r(7294);function a(e,t,r){return t in e?Object.defineProperty(e,t,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[t]=r,e}function o(e,t){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),r.push.apply(r,n)}return r}function l(e){for(var t=1;t<arguments.length;t++){var r=null!=arguments[t]?arguments[t]:{};t%2?o(Object(r),!0).forEach((function(t){a(e,t,r[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):o(Object(r)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(r,t))}))}return e}function i(e,t){if(null==e)return{};var r,n,a=function(e,t){if(null==e)return{};var r,n,a={},o=Object.keys(e);for(n=0;n<o.length;n++)r=o[n],t.indexOf(r)>=0||(a[r]=e[r]);return a}(e,t);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(n=0;n<o.length;n++)r=o[n],t.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(a[r]=e[r])}return a}var u=n.createContext({}),p=function(e){var t=n.useContext(u),r=t;return e&&(r="function"==typeof e?e(t):l(l({},t),e)),r},c=function(e){var t=p(e.components);return n.createElement(u.Provider,{value:t},e.children)},s={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},d=n.forwardRef((function(e,t){var r=e.components,a=e.mdxType,o=e.originalType,u=e.parentName,c=i(e,["components","mdxType","originalType","parentName"]),d=p(r),m=a,f=d["".concat(u,".").concat(m)]||d[m]||s[m]||o;return r?n.createElement(f,l(l({ref:t},c),{},{components:r})):n.createElement(f,l({ref:t},c))}));function m(e,t){var r=arguments,a=t&&t.mdxType;if("string"==typeof e||a){var o=r.length,l=new Array(o);l[0]=d;var i={};for(var u in t)hasOwnProperty.call(t,u)&&(i[u]=t[u]);i.originalType=e,i.mdxType="string"==typeof e?e:a,l[1]=i;for(var p=2;p<o;p++)l[p]=r[p];return n.createElement.apply(null,l)}return n.createElement.apply(null,r)}d.displayName="MDXCreateElement"},5788:(e,t,r)=>{r.r(t),r.d(t,{assets:()=>c,contentTitle:()=>u,default:()=>m,frontMatter:()=>i,metadata:()=>p,toc:()=>s});var n=r(7462),a=r(3366),o=(r(7294),r(3905)),l=["components"],i={},u=void 0,p={unversionedId:"cmd/youtube",id:"cmd/youtube",title:"youtube",description:"Commands for calling the YouTube API for videos and playlists. The prefix is .youtube",source:"@site/docs/cmd/youtube.md",sourceDirName:"cmd",slug:"/cmd/youtube",permalink:"/FlawBOT/cmd/youtube",draft:!1,editUrl:"https://github.com/CriticalFlaw/FlawBOT/tree/master/docs/cmd/youtube.md",tags:[],version:"current",lastUpdatedAt:1670646509,formattedLastUpdatedAt:"Dec 10, 2022",frontMatter:{},sidebar:"jsonSideBar",previous:{title:"user",permalink:"/FlawBOT/cmd/user"}},c={},s=[{value:"channel",id:"channel",level:3},{value:"playlist",id:"playlist",level:3},{value:"search",id:"search",level:3},{value:"video",id:"video",level:3}],d={toc:s};function m(e){var t=e.components,r=(0,a.Z)(e,l);return(0,o.kt)("wrapper",(0,n.Z)({},d,r,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("p",null,"Commands for calling the YouTube API for videos and playlists. The prefix is ",(0,o.kt)("inlineCode",{parentName:"p"},".youtube")),(0,o.kt)("h3",{id:"channel"},"channel"),(0,o.kt)("p",null,"Returns a list of YouTube channels. Other aliases: ",(0,o.kt)("inlineCode",{parentName:"p"},"channels"),", ",(0,o.kt)("inlineCode",{parentName:"p"},"chn")),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".youtube channel pewdiepie\n")),(0,o.kt)("h3",{id:"playlist"},"playlist"),(0,o.kt)("p",null,"Returns a list of YouTube playlists. Other aliases: ",(0,o.kt)("inlineCode",{parentName:"p"},"playlists"),", ",(0,o.kt)("inlineCode",{parentName:"p"},"list")),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".youtube playlist Let's Drown Out\n")),(0,o.kt)("h3",{id:"search"},"search"),(0,o.kt)("p",null,"Returns the first YouTube search result. Other aliases: ",(0,o.kt)("inlineCode",{parentName:"p"},"find"),", ",(0,o.kt)("inlineCode",{parentName:"p"},"watch")),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".youtube search Accursed Farms\n")),(0,o.kt)("h3",{id:"video"},"video"),(0,o.kt)("p",null,"Returns a list of YouTube videos. Other aliases: ",(0,o.kt)("inlineCode",{parentName:"p"},"videos"),", ",(0,o.kt)("inlineCode",{parentName:"p"},"vid")),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".youtube video Zero Punctuation\n")))}m.isMDXComponent=!0}}]);