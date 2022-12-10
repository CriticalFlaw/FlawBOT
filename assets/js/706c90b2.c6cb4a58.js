"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[216],{3905:(e,t,n)=>{n.d(t,{Zo:()=>s,kt:()=>m});var r=n(7294);function a(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function l(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var r=Object.getOwnPropertySymbols(e);t&&(r=r.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,r)}return n}function o(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?l(Object(n),!0).forEach((function(t){a(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):l(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function i(e,t){if(null==e)return{};var n,r,a=function(e,t){if(null==e)return{};var n,r,a={},l=Object.keys(e);for(r=0;r<l.length;r++)n=l[r],t.indexOf(n)>=0||(a[n]=e[n]);return a}(e,t);if(Object.getOwnPropertySymbols){var l=Object.getOwnPropertySymbols(e);for(r=0;r<l.length;r++)n=l[r],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(a[n]=e[n])}return a}var c=r.createContext({}),p=function(e){var t=r.useContext(c),n=t;return e&&(n="function"==typeof e?e(t):o(o({},t),e)),n},s=function(e){var t=p(e.components);return r.createElement(c.Provider,{value:t},e.children)},u={inlineCode:"code",wrapper:function(e){var t=e.children;return r.createElement(r.Fragment,{},t)}},d=r.forwardRef((function(e,t){var n=e.components,a=e.mdxType,l=e.originalType,c=e.parentName,s=i(e,["components","mdxType","originalType","parentName"]),d=p(n),m=a,h=d["".concat(c,".").concat(m)]||d[m]||u[m]||l;return n?r.createElement(h,o(o({ref:t},s),{},{components:n})):r.createElement(h,o({ref:t},s))}));function m(e,t){var n=arguments,a=t&&t.mdxType;if("string"==typeof e||a){var l=n.length,o=new Array(l);o[0]=d;var i={};for(var c in t)hasOwnProperty.call(t,c)&&(i[c]=t[c]);i.originalType=e,i.mdxType="string"==typeof e?e:a,o[1]=i;for(var p=2;p<l;p++)o[p]=n[p];return r.createElement.apply(null,o)}return r.createElement.apply(null,n)}d.displayName="MDXCreateElement"},7842:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>s,contentTitle:()=>c,default:()=>m,frontMatter:()=>i,metadata:()=>p,toc:()=>u});var r=n(7462),a=n(3366),l=(n(7294),n(3905)),o=["components"],i={},c=void 0,p={unversionedId:"modules/channel",id:"modules/channel",title:"channel",description:"Commands for managing server channels. The prefixes are .chn and.ch",source:"@site/docs/modules/channel.md",sourceDirName:"modules",slug:"/modules/channel",permalink:"/FlawBOT/modules/channel",draft:!1,editUrl:"https://github.com/CriticalFlaw/FlawBOT/tree/master/docs/modules/channel.md",tags:[],version:"current",lastUpdatedAt:1670707977,formattedLastUpdatedAt:"Dec 10, 2022",frontMatter:{},sidebar:"jsonSideBar",previous:{title:"bot",permalink:"/FlawBOT/modules/bot"},next:{title:"emoji",permalink:"/FlawBOT/modules/emoji"}},s={},u=[{value:"category",id:"category",level:3},{value:"clean",id:"clean",level:3},{value:"delete",id:"delete",level:3},{value:"info",id:"info",level:3},{value:"purge",id:"purge",level:3},{value:"rename",id:"rename",level:3},{value:"text",id:"text",level:3},{value:"topic",id:"topic",level:3},{value:"voice",id:"voice",level:3},{value:"vote",id:"vote",level:3}],d={toc:u};function m(e){var t=e.components,n=(0,a.Z)(e,o);return(0,l.kt)("wrapper",(0,r.Z)({},d,n,{components:t,mdxType:"MDXLayout"}),(0,l.kt)("p",null,"Commands for managing server channels. The prefixes are ",(0,l.kt)("inlineCode",{parentName:"p"},".chn")," and",(0,l.kt)("inlineCode",{parentName:"p"},".ch")),(0,l.kt)("h3",{id:"category"},"category"),(0,l.kt)("p",null,"Creates a new channel category. Other aliases: ",(0,l.kt)("inlineCode",{parentName:"p"},"createcategory"),", ",(0,l.kt)("inlineCode",{parentName:"p"},"newcategory")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".channel category Welcome\n")),(0,l.kt)("h3",{id:"clean"},"clean"),(0,l.kt)("p",null,"Removes last X number of messages from the current channel. Other alias: ",(0,l.kt)("inlineCode",{parentName:"p"},"clear")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".channel clean 10\n")),(0,l.kt)("h3",{id:"delete"},"delete"),(0,l.kt)("p",null,"Deletes a server channel. If a channel is not specified, the current one is used. Other alias: ",(0,l.kt)("inlineCode",{parentName:"p"},"remove")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".channel delete #text\n")),(0,l.kt)("h3",{id:"info"},"info"),(0,l.kt)("p",null,"Returns information on a given server channel. If a channel is not specified, the current one is used."),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".channel info #text\n")),(0,l.kt)("h3",{id:"purge"},"purge"),(0,l.kt)("p",null,"Removes server user's last X number of messages from the current channel."),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".channel purge @CriticalFlaw 10\n")),(0,l.kt)("h3",{id:"rename"},"rename"),(0,l.kt)("p",null,"Renames a server channel. Other alias: ",(0,l.kt)("inlineCode",{parentName:"p"},"setname")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".channel rename #text newtext\n")),(0,l.kt)("h3",{id:"text"},"text"),(0,l.kt)("p",null,"Creates a new text channel. Other aliases: ",(0,l.kt)("inlineCode",{parentName:"p"},"createtext"),", ",(0,l.kt)("inlineCode",{parentName:"p"},"newtext")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".channel text texts\n")),(0,l.kt)("h3",{id:"topic"},"topic"),(0,l.kt)("p",null,"Changes the current channel's topic. Other alias: ",(0,l.kt)("inlineCode",{parentName:"p"},"settopic")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".channel topic Watermelon picking\n")),(0,l.kt)("h3",{id:"voice"},"voice"),(0,l.kt)("p",null,"Creates a new voice channel. Other aliases: ",(0,l.kt)("inlineCode",{parentName:"p"},"createvoice"),", ",(0,l.kt)("inlineCode",{parentName:"p"},"newvoice")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".channel voice voices\n")),(0,l.kt)("h3",{id:"vote"},"vote"),(0,l.kt)("p",null,"Starts a Yay or Nay poll in the current channel. Other alias: ",(0,l.kt)("inlineCode",{parentName:"p"},"poll")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".channel poll Am I correct?\n")))}m.isMDXComponent=!0}}]);