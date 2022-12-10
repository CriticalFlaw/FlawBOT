Commands for managing server users. The prefixes are `.user`, `.users` and `.usr`

### avatar
Returns server user's profile picture. Other aliases: `getavatar`, `image`, `pfp`
```
.user avatar @CriticalFlaw
```

### ban
Bans a server user. Optionally include a reason.
```
.user ban @CriticalFlaw Spammer
```

### deafen
Deafens a server user. Optionally include a reason. Other alias: `deaf`
```
.user deafen @CriticalFlaw 
```

### info
Returns information on a given server user. Other aliases: `poke`, `pk`
```
.user info @CriticalFlaw 
```

### kick
Kicks a user from the server. Optionally include a reason. Other alias: `remove`
```
.user kick @CriticalFlaw 
```

### mute
Mutes a server user. Optionally include a reason. Other alias: `silence`
```
.user mute @CriticalFlaw 
```

### nickname
Changes server user's nickname. Other aliases: `setnick`, `nick`
```
.user nickname @CriticalFlaw critical
```

### perms
Returns permissions of a server user.
```
.user perms @CriticalFlaw #text
```

### unban
Unbans a server user. Optionally include a reason. Only user's Discord ID is accepted.
```
.user unban 1234567
```

### undeafen
Undeafens a server user. Optionally include a reason. Other alias: `undeaf`
```
.user undeafen @CriticalFlaw
```

### unmute
Unmutes a server user. Optionally include a reason.
```
.user unmute @CriticalFlaw
```