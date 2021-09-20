Commands for managing server roles. The prefixes are `.role` and `.roles`

### color
Changes the server role color in HEX format. Other aliases: `setcolor`, `clr`
```
.role color #F2A92B
```

### create
Creates a new server role. Other aliases: `new`, `add`
```
.role create admin
```

### delete
Deletes a server role. Other alias: `remove`
```
.role delete admin
```

### info
Returns information on a given server role.
```
.role info admin
```

### inrole
Returns a list of server users with a given role.
```
.role inrole admin
```

### mention
Toggles server role being mentionable by other users.
```
.role mention admin
```

### revoke
Removes a role from a server user.
```
.role revoke @CriticalFlaw admin
```

### revokeall
Removes all roles from a server user.
```
.role revokeall @CriticalFlaw
```

### setrole
Assigns a role to a server user. Other alias: `addrole`
```
.role setrole @CriticalFlaw admin
```

### show
Toggles server role being visible to users. Other aliases: `display`, `hide`
```
.role show admin
```