Commands for managing server channels.

### clean
Removes up to 100 messages from the current server channel. Messages older than two weeks cannot be bulk-deleted by Discord and are skipped.
```
/channel clean 10
```

### delete
Deletes a server channel.
```
/channel delete #text
```

### info
Returns information about a server channel.
```
/channel info #text
```

### new-category
Creates a new channel category.
```
/channel new-category Welcome
```

### new-text
Creates a new text channel. Spaces are replaced with dashes.
```
/channel new-text general-chat
```

### new-voice
Creates a new voice channel.
```
/channel new-voice Music Room
```

### rename
Changes the server channel name.
```
/channel rename #text newtext
```

### topic
Changes the server channel topic.
```
/channel topic Watermelon picking
```