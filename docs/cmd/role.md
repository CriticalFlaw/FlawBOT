Description: Commands for controlling server roles.
Aliases: roles

### color

Description: Change the server role color.
Aliases: setcolor, clr
Parameters: Server role's new HEX color code. Server role to recolor

```
.roles color #F2A92B
```

### create

Description: Create a new server role.
Aliases: new
Parameters: New role name.

```
.roles create admin
```

### delete

Description: Delete a server role.
Aliases: remove
Parameters: Server role to remove.

```
.roles delete admin
```

### info

Description: Retrieve server role information.
Aliases: None.
Parameters: Server role name.

```
.roles info admin
```

### inrole

Description: Retrieve a list of users with a given role.
Aliases: None.
Parameters: Server role name.

```
.roles inrole admin
```

### mention

Description: Toggle whether this role can be mentioned by others.
Aliases: None.
Parameters: Server role name.

```
.roles mentionadmin
```

### revoke

Description: Remove a role from server user.
Aliases: None
Parameters: Server user to get revoked. Server role name.

```
.roles revoke @CriticalFlaw admin
```

### revokeall

Description: Remove all role from a server user.
Aliases: None.
Parameters: Server user name.

```
.roles revokeall @CriticalFlaw
```

### setrole

Description: Assign a role to server user.
Aliases: addrole
Parameters: Server user name. Server role name.

```
.roles setrole @CriticalFlaw admin
```

### show

Description: Toggle whether this role is seen or not.
Aliases: display, hide
Parameters: Server role name.

```
.roles show admin
```