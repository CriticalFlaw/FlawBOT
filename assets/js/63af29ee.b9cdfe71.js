"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[705],{3905:(e,t,r)=>{r.d(t,{Zo:()=>c,kt:()=>m});var n=r(7294);function a(e,t,r){return t in e?Object.defineProperty(e,t,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[t]=r,e}function i(e,t){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),r.push.apply(r,n)}return r}function o(e){for(var t=1;t<arguments.length;t++){var r=null!=arguments[t]?arguments[t]:{};t%2?i(Object(r),!0).forEach((function(t){a(e,t,r[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):i(Object(r)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(r,t))}))}return e}function l(e,t){if(null==e)return{};var r,n,a=function(e,t){if(null==e)return{};var r,n,a={},i=Object.keys(e);for(n=0;n<i.length;n++)r=i[n],t.indexOf(r)>=0||(a[r]=e[r]);return a}(e,t);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(n=0;n<i.length;n++)r=i[n],t.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(a[r]=e[r])}return a}var p=n.createContext({}),s=function(e){var t=n.useContext(p),r=t;return e&&(r="function"==typeof e?e(t):o(o({},t),e)),r},c=function(e){var t=s(e.components);return n.createElement(p.Provider,{value:t},e.children)},u={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},d=n.forwardRef((function(e,t){var r=e.components,a=e.mdxType,i=e.originalType,p=e.parentName,c=l(e,["components","mdxType","originalType","parentName"]),d=s(r),m=a,k=d["".concat(p,".").concat(m)]||d[m]||u[m]||i;return r?n.createElement(k,o(o({ref:t},c),{},{components:r})):n.createElement(k,o({ref:t},c))}));function m(e,t){var r=arguments,a=t&&t.mdxType;if("string"==typeof e||a){var i=r.length,o=new Array(i);o[0]=d;var l={};for(var p in t)hasOwnProperty.call(t,p)&&(l[p]=t[p]);l.originalType=e,l.mdxType="string"==typeof e?e:a,o[1]=l;for(var s=2;s<i;s++)o[s]=r[s];return n.createElement.apply(null,o)}return n.createElement.apply(null,r)}d.displayName="MDXCreateElement"},3317:(e,t,r)=>{r.r(t),r.d(t,{assets:()=>c,contentTitle:()=>p,default:()=>m,frontMatter:()=>l,metadata:()=>s,toc:()=>u});var n=r(7462),a=r(3366),i=(r(7294),r(3905)),o=["components"],l={},p=void 0,s={unversionedId:"modules/search",id:"modules/search",title:"search",description:"dictionary",source:"@site/docs/modules/search.md",sourceDirName:"modules",slug:"/modules/search",permalink:"/FlawBOT/modules/search",draft:!1,editUrl:"https://github.com/CriticalFlaw/FlawBOT/tree/master/docs/modules/search.md",tags:[],version:"current",lastUpdatedAt:1678671227,formattedLastUpdatedAt:"Mar 13, 2023",frontMatter:{},sidebar:"jsonSideBar",previous:{title:"role",permalink:"/FlawBOT/modules/role"},next:{title:"server",permalink:"/FlawBOT/modules/server"}},c={},u=[{value:"dictionary",id:"dictionary",level:3},{value:"imgur",id:"imgur",level:3},{value:"nasa",id:"nasa",level:3},{value:"news",id:"news",level:3},{value:"omdb",id:"omdb",level:3},{value:"time",id:"time",level:3},{value:"weather",id:"weather",level:3},{value:"simpsons",id:"simpsons",level:3},{value:"frinkiac",id:"frinkiac",level:3},{value:"twitch",id:"twitch",level:3},{value:"wiki",id:"wiki",level:3}],d={toc:u};function m(e){var t=e.components,r=(0,a.Z)(e,o);return(0,i.kt)("wrapper",(0,n.Z)({},d,r,{components:t,mdxType:"MDXLayout"}),(0,i.kt)("h3",{id:"dictionary"},"dictionary"),(0,i.kt)("p",null,"Returns an Urban Dictionary definition for a word or phrase. Other aliases: ",(0,i.kt)("inlineCode",{parentName:"p"},"define"),", ",(0,i.kt)("inlineCode",{parentName:"p"},"def"),", ",(0,i.kt)("inlineCode",{parentName:"p"},"dic")),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".dictionary Discord\n")),(0,i.kt)("h3",{id:"imgur"},"imgur"),(0,i.kt)("p",null,"Returns an image from Imgur. Other alias: ",(0,i.kt)("inlineCode",{parentName:"p"},"image")),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".imgur Cats\n")),(0,i.kt)("h3",{id:"nasa"},"nasa"),(0,i.kt)("p",null,"Returns NASA's Astronomy Picture of the Day. Other aliases: ",(0,i.kt)("inlineCode",{parentName:"p"},"apod"),", ",(0,i.kt)("inlineCode",{parentName:"p"},"space")),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".nasa\n")),(0,i.kt)("h3",{id:"news"},"news"),(0,i.kt)("p",null,"Returns the news articles on a topic from NewsAPI.org."),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".news Nintendo\n")),(0,i.kt)("h3",{id:"omdb"},"omdb"),(0,i.kt)("p",null,"Returns information on a given movie or TV show from OMDB. Other aliases: ",(0,i.kt)("inlineCode",{parentName:"p"},"imdb"),", ",(0,i.kt)("inlineCode",{parentName:"p"},"movie")),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".imdb Lego Movie\n")),(0,i.kt)("h3",{id:"time"},"time"),(0,i.kt)("p",null,"Returns the current time from a given location. Other alias: ",(0,i.kt)("inlineCode",{parentName:"p"},"clock")),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".time Moscow\n")),(0,i.kt)("h3",{id:"weather"},"weather"),(0,i.kt)("p",null,"Returns the current weather from a given location."),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".weather Ottawa\n")),(0,i.kt)("h3",{id:"simpsons"},"simpsons"),(0,i.kt)("p",null,"Returns a random Simpsons screenshot and its episode. Other alias: ",(0,i.kt)("inlineCode",{parentName:"p"},"caramba"),", ",(0,i.kt)("inlineCode",{parentName:"p"},"futurama")),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".simpsons\n")),(0,i.kt)("h3",{id:"frinkiac"},"frinkiac"),(0,i.kt)("p",null,"Returns a random Simpsons gif. Other alias: ",(0,i.kt)("inlineCode",{parentName:"p"},"doh"),", ",(0,i.kt)("inlineCode",{parentName:"p"},"morbotron")),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".frinkiac oi\n")),(0,i.kt)("h3",{id:"twitch"},"twitch"),(0,i.kt)("p",null,"Returns information on a given Twitch stream."),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".twitch shoutfactorytv\n")),(0,i.kt)("h3",{id:"wiki"},"wiki"),(0,i.kt)("p",null,"Returns articles on a given topic found on Wikipedia."),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".wiki Steam\n")))}m.isMDXComponent=!0}}]);