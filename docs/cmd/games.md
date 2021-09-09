## Nintendo

### amiibo

Description: Retrieve information about an Amiibo figurine.
Aliases: amib
Parameters: Name of the Amiibo figurine.

```
.amiibo luigi
```

### pokemon

Description: Retrieve a Pokémon card.
Aliases: poke, pk
Parameters: Name of the Pokémon.

```
.pokemon pikachu
```

---

## Speedrun

### speedrun

Description: Retrieve a game from Speedrun.com.
Aliases: game, run
Parameters: Game to find on Speedrun.com.

```
.speedrun Super Mario 64
```

---

## Steam

Description: Commands finding Steam games and users.

### connect

Description: Format a game connection string into a link.
Aliases: None.
Parameters: Connection string (ex. IP:PORT)."

```
.tf2 connect 123.345.56.789:000; password hello
```

### game

Description: Retrieve information on a Steam game.
Aliases: None.
Parameters: Game to find on Steam.

```
.steam game Team Fortress 2
```

### user

Description: Retrieve information on a Steam user.
Aliases: player
Parameters: User to find on Steam.

```
.steam user criticalflaw
```

---

## Team Fortress 2

Description: Commands related to Team Fortress 2.

### item

Description: Retrieve an item from the latest TF2 item schema.
Aliases: schema, hat
Parameters: Item to find in the TF2 schema.

```
.tf2 item natasha
```

### map

Description: Retrieve map information from teamwork.tf.
Aliases: maps
Parameters: Normalized map name, like pl_upward.

```
.tf2 map pl_upward
```

### news

Description: Retrieve the latest news article from teamwork.tf.
Aliases: None
Parameters: Page number from which to retrieve the news.

```
.tf2 news
```

### creator

Description: Retrieve a community creator profile from teamwork.tf.
Aliases: creators, youtuber
Parameters: Name of the community creator to find.

```
.tf2 creator criticalflaw
```

### server

Description: Retrieve a list of servers with given game-mode.
Aliases: servers
Parameters: Name of the game-mode, like payload.

```
.tf2 server payload
```