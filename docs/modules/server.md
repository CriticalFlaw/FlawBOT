[issues-link]: https://github.com/CriticalFlaw/FlawBOT/issues
[discord-link]: https://discord.gg/hTdtK9vBhE

This section is for common issues you may encounter and how to resolve them.

* For error or issues not on this page, please [open a ticket on our issue tracker][issues-link].
* For questions not covered in this documentation, [post in our Discord server][discord-link].

---

## Channel

### category

Description: Create a new channel category
Aliases: createcategory, newcategory, ct
Parameters: New category name

```
.channel category Welcome
```

### clean

Description: Remove channel messages
Aliases: clear
Parameters: Number of message to remove from the current channel

```
.channel clean 10
```

### delete

Description: Delete a channel. If a channel isn't specified, the current one will be deleted
Aliases: remove
Parameters: Channel to delete

```
.channel delete #text
```

### info

Description: Print channel information. If a channel isn't specified, the current one will be used
Aliases: i
Parameters: Channel to retrieve information from

```
.channel info #text
```

### purge

Description: Remove server user's channel messages
Aliases: None
Parameters: Server user whose messages will be purged. Number of messages to purge.

```
.channel purge @CriticalFlaw 10
```

### rename

Description: Rename a channel. If a channel isn't specified, the current one will be used
Aliases: setname
Parameters: Channel to rename. New channel name.

```
.channel rename #text newtext
```

### text

Description: Create a new text channel
Aliases: createtext, newtext, ctc
Parameters: New text channel name

```
.channel text texts
```

### topic

Description: Set current channel's topic
Aliases: settopic, st
Parameters: New channel topic

```
.channel topic Watermelon picking
```

### voice

Description: Create a new voice channel
Aliases: createvoice, newvoice, cvc
Parameters: New voice channel name

```
.channel voice voices
```

---

## Emoji

### add

Description: Add a new server emoji through URL or as an attachment
Aliases: addnew, create
Parameters: Name for the emoji, Image URL

```
.emoji add homer [Image URL]
```

### delete

Description: Remove an existing server emoji. Note: Bots can only delete emojis they created.
Aliases: remove, rm, del
Parameters: Server emoji to delete

```
.emoji delete :homer:
```

### modify

Description: Edit the name of an existing server emoji.
Aliases: e, edit, rename
Parameters: Emoji to rename, New name

```
.emoji rename :homer: homey
```

### info

Description: Retrieve server emoji information
Aliases: i
Parameters: Server emoji information to retrieve

```
.emoji info :homer:
```

### list

Description: Retrieve list of server emojis
Aliases: print, l, ls, all
Parameters: None

```
.emoji list
```

---

## Role

### color

Description: Set the role color
Aliases: clr
Parameters: HEX color code to set for the role. Server role to recolor.

```
.roles color #F2A92B
```

### create

Description: Create a server role
Aliases: new
Parameters: New role name

```
.roles create admin
```

### delete

Description: Delete a server role
Aliases: remove
Parameters: Server role to delete

```
.roles delete admin
```

### info

Description: Retrieve role information
Aliases: i
Parameters: Server role information to retrieve

```
.roles info admin
```

### inrole

Description: Retrieve a list of users in a given role
Aliases: None
Parameters: Server role

```
.roles inrole admin
```

### mention

Description: Toggle whether this role can be mentioned by others
Aliases: None
Parameters: Server role to toggle

```
.roles mentionadmin
```

### revoke

Description: Remove a role from server user
Aliases: None
Parameters: Server user to get revoked. Server role to revoke from user

```
.roles revoke @CriticalFlaw admin
```

### revokeall

Description: Remove all role from server user
Aliases: None
Parameters: Server user to get revoked

```
.roles revokeall @CriticalFlaw
```

### setrole

Description: Assign a role to server user
Aliases: addrole, sr
Parameters: Server user to get role assigned. Server role to assign to the user

```
.roles setrole @CriticalFlaw admin
```

### show

Description: Toggle whether this role is seen or not
Aliases: display, hide
Parameters: Server role to toggle

```
.roles show admin
```

---

## Server

### avatar

Description: Set server avatar
Aliases: setavatar
Parameters: Image URL. Must be in jpg, png or img format.
```
.server avatar [Image URL]
```

### info

Description: Retrieve server information
Aliases: i
Parameters: None

```
.server info
```

### invite

Description: Retrieve an instant invite link to the server
Aliases: None
Parameters: None

```
.server invite
```

### prune

Description: Prune inactive server members
Aliases: None
Parameters: Number of days the user had to be inactive to get pruned

```
.server prune 30
```

### rename

Description: Set server name
Aliases: setname
Parameters: New server name

```
.server rename Cool Discord Server 
```

### warn

Description: Direct message user with a warning
Aliases: scold
Parameters: Server user to warn. Warning message.

```
.server warn @CriticalFlaw stop spamming!
```

---

## User

### avatar

Description: Retrieve server user's avatar
Aliases: getavatar
Parameters: Server user whose avatar to retrieve

```
.user avatar @CriticalFlaw
```

### ban

Description: Ban server user
Aliases: None
Parameters: Server user to ban. Reason for the ban. (optional)

```
.user ban @CriticalFlaw Spammer
```

### deafen

Description: Deafen server user
Aliases: None
Parameters: Server user to deafen. Reason for the deafen. (optional)

```
.user deafen @CriticalFlaw 
```

### info

Description: Retrieve user information
Aliases: i
Parameters: Server user whose information to retrieve

```
.user info @CriticalFlaw 
```

### kick

Description: Kick server user
Aliases: None
Parameters: Server user to kick. Reason for the kick. (optional)

```
.user kick @CriticalFlaw 
```

### mute

Description: Mute server user
Aliases: None
Parameters: Server user to mute. Reason for the mute. (optional)

```
.user mute @CriticalFlaw 
```

### nickname

Description: Set server user's nickname
Aliases: setnick
Parameters: Server user to nickname. The new nickname

```
.user nickname @CriticalFlaw critical
```

### perms

Description: Retrieve server user's permissions
Aliases: prm
Parameters: Server user whose permissions to retrieve. Server channel

```
.user perms @CriticalFlaw #text
```

### unban

Description: Unban server user
Aliases: None
Parameters: Discord user ID to unban from the server

```
.user unban 1234567
```

### undeafen

Description: Undeafen server user
Aliases: None
Parameters: Server user to undeafen

```
.user undeafen @CriticalFlaw
```

### unmute

Description: Unmute server user
Aliases: None
Parameters: Server user to unmute

```
.user unmute @CriticalFlaw
```