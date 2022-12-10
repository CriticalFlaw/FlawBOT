Commands for managing server channels. The prefixes are `.chn` and`.ch`

### category
Creates a new channel category. Other aliases: `createcategory`, `newcategory`
```
.channel category Welcome
```

### clean
Removes last X number of messages from the current channel. Other alias: `clear`
```
.channel clean 10
```

### delete
Deletes a server channel. If a channel is not specified, the current one is used. Other alias: `remove`
```
.channel delete #text
```

### info
Returns information on a given server channel. If a channel is not specified, the current one is used.
```
.channel info #text
```

### purge
Removes server user's last X number of messages from the current channel.
```
.channel purge @CriticalFlaw 10
```

### rename
Renames a server channel. Other alias: `setname`
```
.channel rename #text newtext
```

### text
Creates a new text channel. Other aliases: `createtext`, `newtext`
```
.channel text texts
```

### topic
Changes the current channel's topic. Other alias: `settopic`
```
.channel topic Watermelon picking
```

### voice
Creates a new voice channel. Other aliases: `createvoice`, `newvoice`
```
.channel voice voices
```

### vote
Starts a Yay or Nay poll in the current channel. Other alias: `poll`
```
.channel poll Am I correct?
```