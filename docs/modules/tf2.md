Commands for interacting with the Teamwork.TF API for Team Fortress 2.

### creator
Returns a community creator profile from teamwork.tf
```
/tf2 creator criticalflaw
```

### item
Returns an item from the latest TF2 item schema. Requires a Steam API token.
```
/tf2 item natasha
```

### map
Returns map information from teamwork.tf
```
/tf2 map pl_upward
```

### news
Returns the latest news article from teamwork.tf. Optionally pass a page number for older posts.
```
/tf2 news
/tf2 news 2
```

### search
Returns a game server with given ip address.
```
/tf2 search 164.132.233.16
```

### server
Returns a list of servers for a given game-mode.
```
/tf2 server payload
```

### server-list
Returns a curated list of servers.
```
/tf2 server-list
```