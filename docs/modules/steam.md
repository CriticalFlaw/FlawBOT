Commands for interacting with the Steam API.

### connect
Formats a server connection string, with an optional password, into a link.
```
/steam connect 192.168.0.200:27015
/steam connect 192.168.0.200:27015; password hello
```

### game
Returns information on a Steam game. Accepts a game name or app ID.
```
/steam game Team Fortress 2
```

### user
Returns information on a Steam user. Accepts a custom URL name, a 64-bit Steam ID or a profile link.
```
/steam user criticalflaw
/steam user 76561197960287930
```