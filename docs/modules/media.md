[issues-link]: https://github.com/CriticalFlaw/FlawBOT/issues
[discord-link]: https://discord.gg/hTdtK9vBhE

This section is for common issues you may encounter and how to resolve them.

* For error or issues not on this page, please [open a ticket on our issue tracker][issues-link].
* For questions not covered in this documentation, [post in our Discord server][discord-link].

---

## Amiibo

### amiibo

Description: Retrieve Amiibo figurine information
Aliases: amib
Parameters: Name of the Amiibo figurine

```
.amiibo luigi
```

---

## Dictionary

### dictionary

Description: Retrieve an Urban Dictionary definition of a word or phrase
Aliases: define, def, dic
Parameters: Query to pass to Urban Dictionary

```
.dictionary Discord
```

---

## Google

### time

Description: Retrieve the time for specified location
Aliases: clock
Parameters: Location to retrieve time data from

```
.time Moscow
```

### news

Description: Retrieve the latest news articles from NewsAPI.org
Aliases: None
Parameters: Article topic to find on Google News

```
.news Nintendo
```

### weather

Description: Retrieve the weather for specified location
Aliases: None
Parameters: Location to retrieve weather data from

```
.weather Ottawa
```

---

## Imgur

### imgur

Description: Retrieve an imager from Imgur
Aliases: image
Parameters: Search query to pass to Imgur

```
.imgur Cats
```

---

## NASA

### nasa

Description: Retrieve NASA's Astronomy Picture of the Day
Aliases: apod
Parameters: None

```
.nasa
```

---

## OMDB

### omdb

Description: Retrieve a movie or TV show from IMDB
Aliases: imdb, movie
Parameters: Movie or TV show to find on IMDB

```
.imdb Lego Movie
```

---

## Pokemon

### pokemon

Description: Retrieve a Pokemon card
Aliases: poke, pk
Parameters: Name of the pokemon

```
.pokemon pikachu
```

---

## Reddit

### hot

Description: Get hottest posts for a subreddit.
Aliases: None
Parameters: Name of the subreddit.

```
.reddit hot tf2
```

### new

Description: Get newest posts for a subreddit.
Aliases: None
Parameters: Name of the subreddit.

```
.reddit new tf2
```

### top

Description: Get top posts for a subreddit.
Aliases: None
Parameters: Name of the subreddit.

```
.reddit top tf2
```

---

## Simpsons

### simpsons

Description: Retrieve a random Simpsons screenshot and episode
Aliases: caramba
Parameters: None

```
.simpsons
```

### simpsonsgif

Description: Retrieve a random Simpsons gif
Aliases: doh
Parameters: Inputting anything will add episode information

```
.simpsonsgif oi
```

### futurama

Description: Retrieve a random Futurama screenshot and episode
Aliases: bite
Parameters: None

```
.futurama
```

### futuramagif

Description: Retrieve a random Futurama gif
Aliases: neat
Parameters: Inputting anything will add episode information

```
.futuramagif oi
```

### rick

Description: Retrieve a random Rick and Morty screenshot and episode
Aliases: morty
Parameters: None

```
.rick
```

### rickgif

Description: Retrieve a random Rick and Morty gif
Aliases: mortygif
Parameters: Inputting anything will add episode information

```
.rickgif oi
```

---

## Speedrun

### speedrun

Description: Retrieve a game from Speedrun.com
Aliases: game, run
Parameters: Game to search on Speedrun.com

```
.speedrun Super Mario 64
```

---

## Steam

### connect

Description: Format TF2 connection information into a clickable link
Aliases: None
Parameters: Connection string

```
.tf2 connect 123.345.56.789:000; password hello
```

### game

Description: Retrieve Steam game information
Aliases: None
Parameters: Game to find on Steam

```
.steam game Team Fortress 2
```

### user

Description: Retrieve Steam user information
Aliases: None
Parameters: User to find on Steam

```
.steam user criticalflaw
```

---

## Team Fortress 2

### item

Description: Retrieve an item from the latest TF2 item schema
Aliases: schema
Parameters: Item to find in the TF2 schema

```
.tf2 item natasha
```

### map

Description: Retrieve map information from teamwork.tf
Aliases: maps
Parameters: Normalized map name, like pl_upward

```
.tf2 map pl_upward
```

### news

Description: Retrieve the latest news article from teamwork.tf
Aliases: None
Parameters: None

```
.tf2 news
```

### server

Description: Retrieve a list of servers with given gamemode
Aliases: servers
Parameters: Name of the gamemode, like payload

```
.tf2 server payload
```

---

## Wikipedia

### wiki

Description: Search Wikipedia for a given query
Aliases: wikipedia
Parameters: Query to search on Wikipedia

```
.wiki Steam
```

---

## YouTube

### channel

Description: Retrieve a list of YouTube channel given a query
Aliases: channels, chn
Parameters: Channels to find on YouTube

```
.youtube channel pewdiepie
```

### playlist

Description: Retrieve a list of YouTube playlists given a query
Aliases: playlists, list
Parameters: Playlist to find on YouTube

```
.youtube playlist Let's Drown Out
```

### search

Description: Retrieve the first YouTube search result given a query
Aliases: find
Parameters: First result video to find on YouTube

```
.youtube search Accursed Farms
```

### video

Description: Retrieve a list of YouTube videos given a query
Aliases: videos, vid
Parameters: Video to find on YouTube

```
.youtube video Zero Punctuation
```