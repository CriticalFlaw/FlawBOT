[issues-link]: https://github.com/CriticalFlaw/FlawBOT/issues
[discord-link]: https://discord.gg/hTdtK9vBhE

This section is for common issues you may encounter and how to resolve them.

* For error or issues not on this page, please [open a ticket on our issue tracker][issues-link].
* For questions not covered in this documentation, [post in our Discord server][discord-link].

---

## Math

### math

Description: Perform a basic math operation.
Aliases: calculate
Parameters: First operand. Operator. Second operand.

```
.math 2 + 2
```

### sum

Description: Calculate the sum of all inputted values.
Aliases: total
Parameters: Numbers to sum up.

```
.sum 2 5 3
```

---

## Miscellaneous

### ask

Description: Ask an 8-ball a question.
Aliases: 8, 8b, 8ball, ball
Parameters: Question to ask the 8-ball.

```
.ask Am I lucky?
```

### cat

Description: Retrieve a random cat fact.
Aliases: meow, catfact, randomcat
Parameters: Retrieve a random cat fact and picture.

```
.cat
```

### coinflip

Description: Flip a coin.
Aliases: coin, flip
Parameters: None.

```
.coinflip
```

### diceroll

Description: Roll a six-sided die.
Aliases: dice, roll, rolldice, die
Parameters: None.

```
.diceroll
```

### dog

Description: Retrieve a random dog photo.
Aliases: woof, bark, randomdog
Parameters: None.

```
.dog
```

### hello

Description: Say hello to another user to the server.
Aliases: hi, howdy
Parameters: User to say hello to.

```
.hello @CriticalFlaw
```

### tts

Description: Make FlawBOT repeat a message as text-to-speech.
Aliases: echo, repeat, say, talk
Parameters: Message for FlawBOT to repeat.

```
.tts Hello World
```