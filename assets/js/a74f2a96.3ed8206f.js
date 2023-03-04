"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[560],{3905:(e,n,r)=>{r.d(n,{Zo:()=>p,kt:()=>m});var t=r(7294);function a(e,n,r){return n in e?Object.defineProperty(e,n,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[n]=r,e}function l(e,n){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var t=Object.getOwnPropertySymbols(e);n&&(t=t.filter((function(n){return Object.getOwnPropertyDescriptor(e,n).enumerable}))),r.push.apply(r,t)}return r}function i(e){for(var n=1;n<arguments.length;n++){var r=null!=arguments[n]?arguments[n]:{};n%2?l(Object(r),!0).forEach((function(n){a(e,n,r[n])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):l(Object(r)).forEach((function(n){Object.defineProperty(e,n,Object.getOwnPropertyDescriptor(r,n))}))}return e}function u(e,n){if(null==e)return{};var r,t,a=function(e,n){if(null==e)return{};var r,t,a={},l=Object.keys(e);for(t=0;t<l.length;t++)r=l[t],n.indexOf(r)>=0||(a[r]=e[r]);return a}(e,n);if(Object.getOwnPropertySymbols){var l=Object.getOwnPropertySymbols(e);for(t=0;t<l.length;t++)r=l[t],n.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(a[r]=e[r])}return a}var o=t.createContext({}),s=function(e){var n=t.useContext(o),r=n;return e&&(r="function"==typeof e?e(n):i(i({},n),e)),r},p=function(e){var n=s(e.components);return t.createElement(o.Provider,{value:n},e.children)},c={inlineCode:"code",wrapper:function(e){var n=e.children;return t.createElement(t.Fragment,{},n)}},d=t.forwardRef((function(e,n){var r=e.components,a=e.mdxType,l=e.originalType,o=e.parentName,p=u(e,["components","mdxType","originalType","parentName"]),d=s(r),m=a,k=d["".concat(o,".").concat(m)]||d[m]||c[m]||l;return r?t.createElement(k,i(i({ref:n},p),{},{components:r})):t.createElement(k,i({ref:n},p))}));function m(e,n){var r=arguments,a=n&&n.mdxType;if("string"==typeof e||a){var l=r.length,i=new Array(l);i[0]=d;var u={};for(var o in n)hasOwnProperty.call(n,o)&&(u[o]=n[o]);u.originalType=e,u.mdxType="string"==typeof e?e:a,i[1]=u;for(var s=2;s<l;s++)i[s]=r[s];return t.createElement.apply(null,i)}return t.createElement.apply(null,r)}d.displayName="MDXCreateElement"},7497:(e,n,r)=>{r.r(n),r.d(n,{assets:()=>p,contentTitle:()=>o,default:()=>m,frontMatter:()=>u,metadata:()=>s,toc:()=>c});var t=r(7462),a=r(3366),l=(r(7294),r(3905)),i=["components"],u={},o=void 0,s={unversionedId:"modules/user",id:"modules/user",title:"user",description:"Commands for managing server users. The prefixes are .user, .users and .usr",source:"@site/docs/modules/user.md",sourceDirName:"modules",slug:"/modules/user",permalink:"/FlawBOT/modules/user",draft:!1,editUrl:"https://github.com/CriticalFlaw/FlawBOT/tree/master/docs/modules/user.md",tags:[],version:"current",lastUpdatedAt:1677905905,formattedLastUpdatedAt:"Mar 4, 2023",frontMatter:{},sidebar:"jsonSideBar",previous:{title:"tf2",permalink:"/FlawBOT/modules/tf2"},next:{title:"youtube",permalink:"/FlawBOT/modules/youtube"}},p={},c=[{value:"avatar",id:"avatar",level:3},{value:"ban",id:"ban",level:3},{value:"deafen",id:"deafen",level:3},{value:"info",id:"info",level:3},{value:"kick",id:"kick",level:3},{value:"mute",id:"mute",level:3},{value:"nickname",id:"nickname",level:3},{value:"perms",id:"perms",level:3},{value:"unban",id:"unban",level:3},{value:"undeafen",id:"undeafen",level:3},{value:"unmute",id:"unmute",level:3}],d={toc:c};function m(e){var n=e.components,r=(0,a.Z)(e,i);return(0,l.kt)("wrapper",(0,t.Z)({},d,r,{components:n,mdxType:"MDXLayout"}),(0,l.kt)("p",null,"Commands for managing server users. The prefixes are ",(0,l.kt)("inlineCode",{parentName:"p"},".user"),", ",(0,l.kt)("inlineCode",{parentName:"p"},".users")," and ",(0,l.kt)("inlineCode",{parentName:"p"},".usr")),(0,l.kt)("h3",{id:"avatar"},"avatar"),(0,l.kt)("p",null,"Returns server user's profile picture. Other aliases: ",(0,l.kt)("inlineCode",{parentName:"p"},"getavatar"),", ",(0,l.kt)("inlineCode",{parentName:"p"},"image"),", ",(0,l.kt)("inlineCode",{parentName:"p"},"pfp")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".user avatar @CriticalFlaw\n")),(0,l.kt)("h3",{id:"ban"},"ban"),(0,l.kt)("p",null,"Bans a server user. Optionally include a reason."),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".user ban @CriticalFlaw Spammer\n")),(0,l.kt)("h3",{id:"deafen"},"deafen"),(0,l.kt)("p",null,"Deafens a server user. Optionally include a reason. Other alias: ",(0,l.kt)("inlineCode",{parentName:"p"},"deaf")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".user deafen @CriticalFlaw \n")),(0,l.kt)("h3",{id:"info"},"info"),(0,l.kt)("p",null,"Returns information on a given server user. Other aliases: ",(0,l.kt)("inlineCode",{parentName:"p"},"poke"),", ",(0,l.kt)("inlineCode",{parentName:"p"},"pk")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".user info @CriticalFlaw \n")),(0,l.kt)("h3",{id:"kick"},"kick"),(0,l.kt)("p",null,"Kicks a user from the server. Optionally include a reason. Other alias: ",(0,l.kt)("inlineCode",{parentName:"p"},"remove")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".user kick @CriticalFlaw \n")),(0,l.kt)("h3",{id:"mute"},"mute"),(0,l.kt)("p",null,"Mutes a server user. Optionally include a reason. Other alias: ",(0,l.kt)("inlineCode",{parentName:"p"},"silence")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".user mute @CriticalFlaw \n")),(0,l.kt)("h3",{id:"nickname"},"nickname"),(0,l.kt)("p",null,"Changes server user's nickname. Other aliases: ",(0,l.kt)("inlineCode",{parentName:"p"},"setnick"),", ",(0,l.kt)("inlineCode",{parentName:"p"},"nick")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".user nickname @CriticalFlaw critical\n")),(0,l.kt)("h3",{id:"perms"},"perms"),(0,l.kt)("p",null,"Returns permissions of a server user."),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".user perms @CriticalFlaw #text\n")),(0,l.kt)("h3",{id:"unban"},"unban"),(0,l.kt)("p",null,"Unbans a server user. Optionally include a reason. Only user's Discord ID is accepted."),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".user unban 1234567\n")),(0,l.kt)("h3",{id:"undeafen"},"undeafen"),(0,l.kt)("p",null,"Undeafens a server user. Optionally include a reason. Other alias: ",(0,l.kt)("inlineCode",{parentName:"p"},"undeaf")),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".user undeafen @CriticalFlaw\n")),(0,l.kt)("h3",{id:"unmute"},"unmute"),(0,l.kt)("p",null,"Unmutes a server user. Optionally include a reason."),(0,l.kt)("pre",null,(0,l.kt)("code",{parentName:"pre"},".user unmute @CriticalFlaw\n")))}m.isMDXComponent=!0}}]);