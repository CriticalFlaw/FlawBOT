Description: Commands for managing server emojis.
Aliases: emote

### add

Description: Add a new server emoji using a URL image.
Aliases: new, create
Parameters: Image URL. Name for the emoji.

```
.emoji add homer [Image URL]
```

### delete

Description: Remove a server emoji. Note: Bots can only delete emojis they created.
Aliases: remove
Parameters: Server emoji to delete.

```
.emoji delete :homer:
```

### rename

Description: Rename a server emoji.
Aliases: edit, modify
Parameters: Emoji to rename. New emoji name.

```
.emoji rename :homer: homey
```

### info

Description: Retrieve server emoji information.
Aliases: None.
Parameters: Server emoji.

```
.emoji info :homer:
```

### list

Description: Retrieve a list of server emojis.
Aliases: print, all
Parameters: None.

```
.emoji list
```