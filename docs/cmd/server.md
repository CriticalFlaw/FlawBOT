[issues-link]: https://github.com/CriticalFlaw/FlawBOT/issues
[discord-link]: https://discord.gg/hTdtK9vBhE

This section is for common issues you may encounter and how to resolve them.

* For error or issues not on this page, please [open a ticket on our issue tracker][issues-link].
* For questions not covered in this documentation, [post in our Discord server][discord-link].

---

## Server

Description: Commands for controlling server.
Aliases: guild

### avatar

Description: Change the server avatar.
Aliases: setavatar
Parameters: URL image in JPG, PNG or IMG format.
```
.server avatar [Image URL]
```

### info

Description: Retrieve server information.
Aliases: None.
Parameters: None.

```
.server info
```

### invite

Description: Retrieve an instant invite link to the server.
Aliases: None.
Parameters: None.

```
.server invite
```

### prune

Description: Prune inactive server members.
Aliases: None.
Parameters: Number of days the user had to be inactive to get pruned.

```
.server prune 30
```

### rename

Description: Change the server name.
Aliases: setname
Parameters: New server name.

```
.server rename Cool Discord Server 
```

### warn

Description: Direct message user with a warning.
Aliases: scold
Parameters: Server user to warn. Warning message.

```
.server warn @CriticalFlaw stop spamming!
```