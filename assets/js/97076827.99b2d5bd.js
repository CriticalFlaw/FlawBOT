"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[680],{3905:(e,r,t)=>{t.d(r,{Zo:()=>c,kt:()=>v});var n=t(7294);function a(e,r,t){return r in e?Object.defineProperty(e,r,{value:t,enumerable:!0,configurable:!0,writable:!0}):e[r]=t,e}function o(e,r){var t=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);r&&(n=n.filter((function(r){return Object.getOwnPropertyDescriptor(e,r).enumerable}))),t.push.apply(t,n)}return t}function i(e){for(var r=1;r<arguments.length;r++){var t=null!=arguments[r]?arguments[r]:{};r%2?o(Object(t),!0).forEach((function(r){a(e,r,t[r])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(t)):o(Object(t)).forEach((function(r){Object.defineProperty(e,r,Object.getOwnPropertyDescriptor(t,r))}))}return e}function l(e,r){if(null==e)return{};var t,n,a=function(e,r){if(null==e)return{};var t,n,a={},o=Object.keys(e);for(n=0;n<o.length;n++)t=o[n],r.indexOf(t)>=0||(a[t]=e[t]);return a}(e,r);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(n=0;n<o.length;n++)t=o[n],r.indexOf(t)>=0||Object.prototype.propertyIsEnumerable.call(e,t)&&(a[t]=e[t])}return a}var s=n.createContext({}),p=function(e){var r=n.useContext(s),t=r;return e&&(t="function"==typeof e?e(r):i(i({},r),e)),t},c=function(e){var r=p(e.components);return n.createElement(s.Provider,{value:r},e.children)},u={inlineCode:"code",wrapper:function(e){var r=e.children;return n.createElement(n.Fragment,{},r)}},m=n.forwardRef((function(e,r){var t=e.components,a=e.mdxType,o=e.originalType,s=e.parentName,c=l(e,["components","mdxType","originalType","parentName"]),m=p(t),v=a,d=m["".concat(s,".").concat(v)]||m[v]||u[v]||o;return t?n.createElement(d,i(i({ref:r},c),{},{components:t})):n.createElement(d,i({ref:r},c))}));function v(e,r){var t=arguments,a=r&&r.mdxType;if("string"==typeof e||a){var o=t.length,i=new Array(o);i[0]=m;var l={};for(var s in r)hasOwnProperty.call(r,s)&&(l[s]=r[s]);l.originalType=e,l.mdxType="string"==typeof e?e:a,i[1]=l;for(var p=2;p<o;p++)i[p]=t[p];return n.createElement.apply(null,i)}return n.createElement.apply(null,t)}m.displayName="MDXCreateElement"},1090:(e,r,t)=>{t.r(r),t.d(r,{assets:()=>c,contentTitle:()=>s,default:()=>v,frontMatter:()=>l,metadata:()=>p,toc:()=>u});var n=t(7462),a=t(3366),o=(t(7294),t(3905)),i=["components"],l={},s=void 0,p={unversionedId:"modules/server",id:"modules/server",title:"server",description:"Commands for managing the Discord server. The prefix is /server.",source:"@site/docs/modules/server.md",sourceDirName:"modules",slug:"/modules/server",permalink:"/FlawBOT/modules/server",draft:!1,editUrl:"https://github.com/CriticalFlaw/FlawBOT/tree/master/docs/modules/server.md",tags:[],version:"current",lastUpdatedAt:1689388805,formattedLastUpdatedAt:"Jul 15, 2023",frontMatter:{},sidebar:"jsonSideBar",previous:{title:"role",permalink:"/FlawBOT/modules/role"},next:{title:"simpsons",permalink:"/FlawBOT/modules/simpsons"}},c={},u=[{value:"avatar",id:"avatar",level:3},{value:"info",id:"info",level:3},{value:"invite",id:"invite",level:3},{value:"rename",id:"rename",level:3}],m={toc:u};function v(e){var r=e.components,t=(0,a.Z)(e,i);return(0,o.kt)("wrapper",(0,n.Z)({},m,t,{components:r,mdxType:"MDXLayout"}),(0,o.kt)("p",null,"Commands for managing the Discord server. The prefix is ",(0,o.kt)("inlineCode",{parentName:"p"},"/server"),"."),(0,o.kt)("h3",{id:"avatar"},"avatar"),(0,o.kt)("p",null,"Changes the server avatar."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/server avatar https://frinkiac.com/img/S03E20/301341.jpg\n")),(0,o.kt)("h3",{id:"info"},"info"),(0,o.kt)("p",null,"Returns server information."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/server info\n")),(0,o.kt)("h3",{id:"invite"},"invite"),(0,o.kt)("p",null,"Returns server invite link."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/server invite\n")),(0,o.kt)("h3",{id:"rename"},"rename"),(0,o.kt)("p",null,"Changes the name of the server."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},"/server rename Cool Discord Server \n")))}v.isMDXComponent=!0}}]);