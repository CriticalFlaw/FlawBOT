[issues-link]: https://github.com/CriticalFlaw/FlawBOT/issues
[discord-link]: https://discord.gg/hTdtK9vBhE

This section is for common issues you may encounter and how to resolve them.

* For error or issues not on this page, please [open a ticket on our issue tracker][issues-link].
* For questions not covered in this documentation, [post in our Discord server][discord-link].

---

## Music

### play

Description: Play audio from provided URL or search by specified query.
Aliases: None.
Parameters: URL from which to play audio.

```
.play https://www.youtube.com/watch?v=4JkIs37a2JE
```

### pause

Description: Pause audio playback.
Aliases: None.
Parameters: None.

```
.pause
```

### resume

Description: Resume audio playback.
Aliases: unpause
Parameters: None.

```
.resume
```

### stop

Description: Stop audio playback and leave the voice channel.
Aliases: None.
Parameters: None.

```
.stop
```

### volume

Description: Set audio playback volume.
Aliases: None.
Parameters: Audio volume. Can be set to 0-150 (default 100).

```
.volume 50
```

### restart

Description: Restarts the playback of the current track.
Aliases: replay
Parameters: None.

```
.replay
```

### nowplaying

Description: Displays information about currently-played track.
Aliases: np
Parameters: None.

```
.nowplaying
```