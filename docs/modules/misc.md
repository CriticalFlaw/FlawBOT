[issues-link]: https://github.com/CriticalFlaw/FlawBOT/issues
[discord-link]: https://discord.gg/hTdtK9vBhE

This section is for common issues you may encounter and how to resolve them.

* For error or issues not on this page, please [open a ticket on our issue tracker][issues-link].
* For questions not covered in this documentation, [post in our Discord server][discord-link].

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

### tts

Description: Make FlawBOT repeat a message in text-to-speech
Aliases: talk
Parameters: Message for the bot to convert to speech

```
.bot tts Hello World
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

## Poll

### poll

Description: Run a Yay or Nay poll in the current channel
Aliases: None
Parameters: Question to be polled.

```
.poll Am I correct?
```