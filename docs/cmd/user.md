Description: Commands for controlling server users.
Aliases: users, usr

### avatar

Description: Retrieve server user's profile picture.
Aliases: getavatar, image, pfp
Parameters: Server user whose profile picture to retrieve.

```
.user avatar @CriticalFlaw
```

### ban

Description: Ban a server user.
Aliases: None.
Parameters: Server user to ban. Reason for the ban.

```
.user ban @CriticalFlaw Spammer
```

### deafen

Description: Deafen a server user.
Aliases: deaf
Parameters: Server user to deafen. Reason for the deafen.

```
.user deafen @CriticalFlaw 
```

### info

Description: Retrieve server user's information.
Aliases: None.
Parameters: Server user whose information to retrieve.

```
.user info @CriticalFlaw 
```

### kick

Description: Kick a user from the server.
Aliases: remove
Parameters: Server user to kick. Reason for the kick.

```
.user kick @CriticalFlaw 
```

### mute

Description: Mute a server user.
Aliases: silence
Parameters: Server user to mute. Reason for the mute.

```
.user mute @CriticalFlaw 
```

### nickname

Description: Change server user's nickname.
Aliases: setnick, nick
Parameters: Server user name. New nickname for the name.

```
.user nickname @CriticalFlaw critical
```

### perms

Description: Retrieve server user's permissions.
Aliases: None.
Parameters: Server user name. Server channel.

```
.user perms @CriticalFlaw #text
```

### unban

Description: Unban a server user.
Aliases: None.
Parameters: Discord user ID to unban from the server. Reason for the unban.

```
.user unban 1234567
```

### undeafen

Description: Undeafen a server user.
Aliases: undeaf
Parameters: Server user to undeafen. Reason for the deafen.

```
.user undeafen @CriticalFlaw
```

### unmute

Description: Unmute a server user.
Aliases: None.
Parameters: Server user to unmute. Reason for the deafen.

```
.user unmute @CriticalFlaw
```