Commands for playing audio in voice channels using [Lavalink](https://criticalflaw.ca/FlawBOT/lavalink).

:::note
Music commands are only registered when Lavalink is enabled in **config.json**. Playing a new track replaces the current one.
:::

### info
Returns the current audio track information.
```
/music info
```

### pause
Pauses audio playback.
```
/music pause
```

### play
Plays audio from a URL or search query. Plain text is searched on YouTube.
```
/music play https://www.youtube.com/watch?v=4JkIs37a2JE
/music play night of fire
```

### restart
Restarts audio playback.
```
/music restart
```

### resume
Resumes audio playback.
```
/music resume
```

### stop
Stops audio playback and leave the voice channel.
```
/music stop
```

### volume
Changes audio playback volume between 0% and 100%. Defaults to 100%.
```
/music volume 50
```