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

## Bot

### info

Description: Retrieve FlawBOT information
Aliases: i, about
Parameters: None

```
.bot info
```

### leave

Description: Make FlawBOT leave the current server
Aliases: None
Parameters: None. Respond with yes when prompted to complete the action.

```
.bot leave
```

### ping

Description: Ping the FlawBOT client
Aliases: pong
Parameters: None

```
.bot ping
```

### report

Description: Report a problem with FlawBOT to the developer.
Aliases: issue
Parameters: Detailed description of the problem. Minimum 50 characters.

```
.bot report [Description of the problem]
```

### say

Description: Make FlawBOT repeat a message
Aliases: echo
Parameters: Message for the bot to repeat

```
.bot say Hello World
```

### tts

Description: Make FlawBOT repeat a message in text-to-speech
Aliases: talk
Parameters: Message for the bot to convert to speech

```
.bot tts Hello World
```

### uptime

Description: Retrieve the FlawBOT uptime
Aliases: time
Parameters: None

```
.bot uptime
```

### activity

Description: Set FlawBOT's activity
Aliases: setactivity
Parameters: Name of the activity
Owner Only: YES

```
.bot activity Team Fortress 2
```

### avatar

Description: Set FlawBOT's avatar
Aliases: setavatar, pfp, photo
Parameters: Image URL. Must be in JPG, PNG or IMG format.
Owner Only: YES

```
.bot avatar [Image URL]
```

### status

Description: Set FlawBOT's status
Aliases: setstatus, state
Parameters: Activity Status. Online, Idle, DND or Offline
Owner Only: YES

```
.bot status Idle
```

### update

Description: Update FlawBOT libraries
Aliases: refresh
Parameters: None
Owner Only: YES

```
.sudo update
```

### username

Description: Set FlawBOT's username
Aliases: setusername, name, setname, nickname, nick
Parameters: New bot username
Owner Only: YES

```
.sudo setname FlowBOT
```

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

## Dictionary

### dictionary

Description: Retrieve an Urban Dictionary definition of a word or phrase
Aliases: define, def, dic
Parameters: Query to pass to Urban Dictionary

```
.dictionary Discord
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

## Math

### math

Description: Perform a basic math operation
Aliases: calculate
Parameters: First operand. Operator. Second operand

```
.math 2 + 2
```

### sum

Description: Calculate the sum of all inputted values
Aliases: total
Parameters: Numbers to sum up.

```
.sum 2 5 3
```

---

## Miscellaneous

### ask

Description: Ask an 8-ball a question
Aliases: 8b, 8ball, ball
Parameters: Question to ask the 8-ball

```
.ask Am I lucky?
```

### cat

Description: Retrieve a random cat fact
Aliases: catfact
Parameters: None

```
.cat
```

### randomcat

Description: Retrieve a random cat photo
Aliases: meow
Parameters: None

```
.randomcat
```

### coinflip

Description: Flip a coin
Aliases: coin, flip
Parameters: None

```
.coinflip
```

### color

Description: Retrieve color values for a given HEX code
Aliases: clr
Parameters: HEX color code to process

```
.color #F2A92B
```

### diceroll

Description: Roll a six-sided die
Aliases: dice, roll, rolldice, die
Parameters: None

```
.diceroll
```

### randomdog

Description: Retrieve a random dog photo
Aliases: dog, woof, bark
Parameters: None

```
.randomdog
```

### hello

Description: Welcome another user to the server
Aliases: hi, howdy
Parameters: User to say hello to

```
.hello @CriticalFlaw
```

### ip

Description: Retrieve IP address geolocation information
Aliases: ipstack, track
Parameters: IP Address

```
.ip 123.123.123.123
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

## Poll

### poll

Description: Run a Yay or Nay poll in the current channel
Aliases: None
Parameters: Question to be polled.

```
.poll Am I correct?
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

## Twitch

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