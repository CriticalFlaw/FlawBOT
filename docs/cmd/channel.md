Description: Commands for controlling channels.
Aliases: chn, ch, c

### category

Description: Create a new channel category.
Aliases: createcategory, newcategory, ct
Parameters: New category name.

```
.channel category Welcome
```

### clean

Description: Remove channel messages.
Aliases: clear
Parameters: Number of messages to remove from the current channel.

```
.channel clean 10
```

### delete

Description: Delete a channel. If a channel isn't specified, the current one will be deleted.
Aliases: remove
Parameters: Channel to delete.

```
.channel delete #text
```

### info

Description: Print channel information. If a channel isn't specified, the current one will be used.
Aliases: i
Parameters: Channel to retrieve information from.

```
.channel info #text
```

### purge

Description: Remove server user's channel messages.
Aliases: None
Parameters: Server user whose messages will be purged. Number of messages to purge.

```
.channel purge @CriticalFlaw 10
```

### rename

Description: Rename a channel. If a channel isn't specified, the current one will be used.
Aliases: setname
Parameters: Channel to rename. New channel name.

```
.channel rename #text newtext
```

### text

Description: Create a new text channel.
Aliases: createtext, newtext
Parameters: New text channel name.

```
.channel text texts
```

### topic

Description: Set current channel's topic.
Aliases: settopic
Parameters: New channel topic.

```
.channel topic Watermelon picking
```

### voice

Description: Create a new voice channel.
Aliases: createvoice, newvoice
Parameters: New voice channel name.

```
.channel voice voices
```

### vote

Description: Run a Yay or Nay poll in the current channel.
Aliases: poll
Parameters: Question to be asked in the poll.

```
.channel poll Am I correct?
```