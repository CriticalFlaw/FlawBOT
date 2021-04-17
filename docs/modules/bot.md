[issues-link]: https://github.com/CriticalFlaw/FlawBOT/issues
[discord-link]: https://discord.gg/hTdtK9vBhE

This section is for common issues you may encounter and how to resolve them.

* For error or issues not on this page, please [open a ticket on our issue tracker][issues-link].
* For questions not covered in this documentation, [post in our Discord server][discord-link].

---

## Bot

### info

Description: Retrieve FlawBOT information.
Aliases: about
Parameters: None

```
.bot info
```

### leave

Description: Make FlawBOT leave the current server.
Aliases: None
Parameters: None. Respond with yes when prompted to complete the action.

```
.bot leave
```

### ping

Description: Ping the FlawBOT client.
Aliases: pong
Parameters: None

```
.bot ping
```

### report

Description: Report a problem with FlawBOT to the developer. Please do not abuse.
Aliases: issue
Parameters: Detailed description of the issue. Minimum 50 characters.

```
.bot report [Description of the problem]
```

### uptime

Description: Retrieve the FlawBOT uptime.
Aliases: time
Parameters: None


```
.bot uptime
```

---

## Owner Only

### activity

Description: Set FlawBOT's activity.
Aliases: setactivity
Parameters: Name of the activity.
```
.bot activity Team Fortress 2
```

### avatar

Description: Set FlawBOT's avatar.
Aliases: setavatar, pfp, photo
Parameters: Image URL. Must be in jpg, png or img format.

```
.bot avatar [Image URL]
```

### status

Description: Set FlawBOT's status.
Aliases: setstatus, state
Parameters: Activity Status. Online, Idle, DND or Offline.

```
.bot status Idle
```

### username

Description: Set FlawBOT's username.
Aliases: setusername, name, setname, nickname, nick
Parameters: New bot username.

```
.sudo setname FlowBOT
```