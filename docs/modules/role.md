Commands for managing server roles.

### create
Creates a new server role.
```
/role create admin
```

### delete
Deletes a server role.
```
/role delete admin
```

### info
Returns server role information.
```
/role info admin
```

### inrole
Returns list of users in a given role.

:::note
Listing server members requires the **Server Members Intent** to be enabled for your bot in the [Discord Developer Portal](https://discord.com/developers/applications) (Bot > Privileged Gateway Intents).
:::
```
/role inrole admin
```

### mention
Toggles whether the role can be mentioned by others.
```
/role mention admin
```