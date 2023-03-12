"use strict";(self.webpackChunk=self.webpackChunk||[]).push([[276],{3905:(e,r,t)=>{t.d(r,{Zo:()=>d,kt:()=>m});var n=t(7294);function l(e,r,t){return r in e?Object.defineProperty(e,r,{value:t,enumerable:!0,configurable:!0,writable:!0}):e[r]=t,e}function o(e,r){var t=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);r&&(n=n.filter((function(r){return Object.getOwnPropertyDescriptor(e,r).enumerable}))),t.push.apply(t,n)}return t}function a(e){for(var r=1;r<arguments.length;r++){var t=null!=arguments[r]?arguments[r]:{};r%2?o(Object(t),!0).forEach((function(r){l(e,r,t[r])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(t)):o(Object(t)).forEach((function(r){Object.defineProperty(e,r,Object.getOwnPropertyDescriptor(t,r))}))}return e}function i(e,r){if(null==e)return{};var t,n,l=function(e,r){if(null==e)return{};var t,n,l={},o=Object.keys(e);for(n=0;n<o.length;n++)t=o[n],r.indexOf(t)>=0||(l[t]=e[t]);return l}(e,r);if(Object.getOwnPropertySymbols){var o=Object.getOwnPropertySymbols(e);for(n=0;n<o.length;n++)t=o[n],r.indexOf(t)>=0||Object.prototype.propertyIsEnumerable.call(e,t)&&(l[t]=e[t])}return l}var p=n.createContext({}),s=function(e){var r=n.useContext(p),t=r;return e&&(t="function"==typeof e?e(r):a(a({},r),e)),t},d=function(e){var r=s(e.components);return n.createElement(p.Provider,{value:r},e.children)},u={inlineCode:"code",wrapper:function(e){var r=e.children;return n.createElement(n.Fragment,{},r)}},c=n.forwardRef((function(e,r){var t=e.components,l=e.mdxType,o=e.originalType,p=e.parentName,d=i(e,["components","mdxType","originalType","parentName"]),c=s(t),m=l,v=c["".concat(p,".").concat(m)]||c[m]||u[m]||o;return t?n.createElement(v,a(a({ref:r},d),{},{components:t})):n.createElement(v,a({ref:r},d))}));function m(e,r){var t=arguments,l=r&&r.mdxType;if("string"==typeof e||l){var o=t.length,a=new Array(o);a[0]=c;var i={};for(var p in r)hasOwnProperty.call(r,p)&&(i[p]=r[p]);i.originalType=e,i.mdxType="string"==typeof e?e:l,a[1]=i;for(var s=2;s<o;s++)a[s]=t[s];return n.createElement.apply(null,a)}return n.createElement.apply(null,t)}c.displayName="MDXCreateElement"},6704:(e,r,t)=>{t.r(r),t.d(r,{assets:()=>d,contentTitle:()=>p,default:()=>m,frontMatter:()=>i,metadata:()=>s,toc:()=>u});var n=t(7462),l=t(3366),o=(t(7294),t(3905)),a=["components"],i={},p=void 0,s={unversionedId:"modules/role",id:"modules/role",title:"role",description:"Commands for managing server roles. The prefixes are .role and .roles",source:"@site/docs/modules/role.md",sourceDirName:"modules",slug:"/modules/role",permalink:"/FlawBOT/modules/role",draft:!1,editUrl:"https://github.com/CriticalFlaw/FlawBOT/tree/master/docs/modules/role.md",tags:[],version:"current",lastUpdatedAt:1678587264,formattedLastUpdatedAt:"Mar 12, 2023",frontMatter:{},sidebar:"jsonSideBar",previous:{title:"reddit",permalink:"/FlawBOT/modules/reddit"},next:{title:"search",permalink:"/FlawBOT/modules/search"}},d={},u=[{value:"color",id:"color",level:3},{value:"create",id:"create",level:3},{value:"delete",id:"delete",level:3},{value:"info",id:"info",level:3},{value:"inrole",id:"inrole",level:3},{value:"mention",id:"mention",level:3},{value:"revoke",id:"revoke",level:3},{value:"revokeall",id:"revokeall",level:3},{value:"setrole",id:"setrole",level:3},{value:"show",id:"show",level:3}],c={toc:u};function m(e){var r=e.components,t=(0,l.Z)(e,a);return(0,o.kt)("wrapper",(0,n.Z)({},c,t,{components:r,mdxType:"MDXLayout"}),(0,o.kt)("p",null,"Commands for managing server roles. The prefixes are ",(0,o.kt)("inlineCode",{parentName:"p"},".role")," and ",(0,o.kt)("inlineCode",{parentName:"p"},".roles")),(0,o.kt)("h3",{id:"color"},"color"),(0,o.kt)("p",null,"Changes the server role color in HEX format. Other aliases: ",(0,o.kt)("inlineCode",{parentName:"p"},"setcolor"),", ",(0,o.kt)("inlineCode",{parentName:"p"},"clr")),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".role color #F2A92B\n")),(0,o.kt)("h3",{id:"create"},"create"),(0,o.kt)("p",null,"Creates a new server role. Other aliases: ",(0,o.kt)("inlineCode",{parentName:"p"},"new"),", ",(0,o.kt)("inlineCode",{parentName:"p"},"add")),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".role create admin\n")),(0,o.kt)("h3",{id:"delete"},"delete"),(0,o.kt)("p",null,"Deletes a server role. Other alias: ",(0,o.kt)("inlineCode",{parentName:"p"},"remove")),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".role delete admin\n")),(0,o.kt)("h3",{id:"info"},"info"),(0,o.kt)("p",null,"Returns information on a given server role."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".role info admin\n")),(0,o.kt)("h3",{id:"inrole"},"inrole"),(0,o.kt)("p",null,"Returns a list of server users with a given role."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".role inrole admin\n")),(0,o.kt)("h3",{id:"mention"},"mention"),(0,o.kt)("p",null,"Toggles server role being mentionable by other users."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".role mention admin\n")),(0,o.kt)("h3",{id:"revoke"},"revoke"),(0,o.kt)("p",null,"Removes a role from a server user."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".role revoke @CriticalFlaw admin\n")),(0,o.kt)("h3",{id:"revokeall"},"revokeall"),(0,o.kt)("p",null,"Removes all roles from a server user."),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".role revokeall @CriticalFlaw\n")),(0,o.kt)("h3",{id:"setrole"},"setrole"),(0,o.kt)("p",null,"Assigns a role to a server user. Other alias: ",(0,o.kt)("inlineCode",{parentName:"p"},"addrole")),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".role setrole @CriticalFlaw admin\n")),(0,o.kt)("h3",{id:"show"},"show"),(0,o.kt)("p",null,"Toggles server role being visible to users. Other aliases: ",(0,o.kt)("inlineCode",{parentName:"p"},"display"),", ",(0,o.kt)("inlineCode",{parentName:"p"},"hide")),(0,o.kt)("pre",null,(0,o.kt)("code",{parentName:"pre"},".role show admin\n")))}m.isMDXComponent=!0}}]);