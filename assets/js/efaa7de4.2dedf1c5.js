"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[696],{3905:(e,t,r)=>{r.d(t,{Zo:()=>s,kt:()=>m});var n=r(7294);function o(e,t,r){return t in e?Object.defineProperty(e,t,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[t]=r,e}function i(e,t){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),r.push.apply(r,n)}return r}function a(e){for(var t=1;t<arguments.length;t++){var r=null!=arguments[t]?arguments[t]:{};t%2?i(Object(r),!0).forEach((function(t){o(e,t,r[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):i(Object(r)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(r,t))}))}return e}function l(e,t){if(null==e)return{};var r,n,o=function(e,t){if(null==e)return{};var r,n,o={},i=Object.keys(e);for(n=0;n<i.length;n++)r=i[n],t.indexOf(r)>=0||(o[r]=e[r]);return o}(e,t);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(n=0;n<i.length;n++)r=i[n],t.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(o[r]=e[r])}return o}var d=n.createContext({}),p=function(e){var t=n.useContext(d),r=t;return e&&(r="function"==typeof e?e(t):a(a({},t),e)),r},s=function(e){var t=p(e.components);return n.createElement(d.Provider,{value:t},e.children)},u={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},c=n.forwardRef((function(e,t){var r=e.components,o=e.mdxType,i=e.originalType,d=e.parentName,s=l(e,["components","mdxType","originalType","parentName"]),c=p(r),m=o,f=c["".concat(d,".").concat(m)]||c[m]||u[m]||i;return r?n.createElement(f,a(a({ref:t},s),{},{components:r})):n.createElement(f,a({ref:t},s))}));function m(e,t){var r=arguments,o=t&&t.mdxType;if("string"==typeof e||o){var i=r.length,a=new Array(i);a[0]=c;var l={};for(var d in t)hasOwnProperty.call(t,d)&&(l[d]=t[d]);l.originalType=e,l.mdxType="string"==typeof e?e:o,a[1]=l;for(var p=2;p<i;p++)a[p]=r[p];return n.createElement.apply(null,a)}return n.createElement.apply(null,r)}c.displayName="MDXCreateElement"},6155:(e,t,r)=>{r.r(t),r.d(t,{assets:()=>s,contentTitle:()=>d,default:()=>m,frontMatter:()=>l,metadata:()=>p,toc:()=>u});var n=r(7462),o=r(3366),i=(r(7294),r(3905)),a=["components"],l={},d=void 0,p={unversionedId:"modules/reddit",id:"modules/reddit",title:"reddit",description:"Commands for calling the Reddit API for posts from various subreddits. The prefix is .reddit",source:"@site/docs/modules/reddit.md",sourceDirName:"modules",slug:"/modules/reddit",permalink:"/FlawBOT/modules/reddit",draft:!1,editUrl:"https://github.com/CriticalFlaw/FlawBOT/tree/master/docs/modules/reddit.md",tags:[],version:"current",lastUpdatedAt:1684985519,formattedLastUpdatedAt:"May 25, 2023",frontMatter:{},sidebar:"jsonSideBar",previous:{title:"music",permalink:"/FlawBOT/modules/music"},next:{title:"role",permalink:"/FlawBOT/modules/role"}},s={},u=[{value:"hot",id:"hot",level:3},{value:"new",id:"new",level:3},{value:"top",id:"top",level:3}],c={toc:u};function m(e){var t=e.components,r=(0,o.Z)(e,a);return(0,i.kt)("wrapper",(0,n.Z)({},c,r,{components:t,mdxType:"MDXLayout"}),(0,i.kt)("p",null,"Commands for calling the Reddit API for posts from various subreddits. The prefix is ",(0,i.kt)("inlineCode",{parentName:"p"},".reddit")),(0,i.kt)("h3",{id:"hot"},"hot"),(0,i.kt)("p",null,"Returns the hottest posts from a given subreddit."),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".reddit hot tf2\n")),(0,i.kt)("h3",{id:"new"},"new"),(0,i.kt)("p",null,"Returns the latest posts from a given subreddit."),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".reddit new tf2\n")),(0,i.kt)("h3",{id:"top"},"top"),(0,i.kt)("p",null,"Returns the top posts from a given subreddit."),(0,i.kt)("pre",null,(0,i.kt)("code",{parentName:"pre"},".reddit top tf2\n")))}m.isMDXComponent=!0}}]);