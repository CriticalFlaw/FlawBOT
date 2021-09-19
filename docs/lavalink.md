Lavalink is a standalone audio sending node. It's required to use FlawBOT's music commands.

The latest version of Lavalink is included with FlawBOT, however it still needs to be configured and enabled.

1. Open **application.yml** and change `address` to the local IP address the machine you'll be running Lavalink from.
```yml
server: # REST and WS server
  port: 2333
  address: 192.168.2.38
```
2. Open **config.json** and make sure `Address` matches the address set in Step 1. Also make sure `Enabled` is set to true.
```json
"Lavalink": {
   "Enabled": true,
   "Address": "192.168.2.38",
   "Port": 2333,
   "Password": "youshallnotpass"
}
```
3. Open the command prompt and start Lavalink by entering: `java -jar Lavalink.jar`
   * In case of errors, install the [latest Java Development Kit][java-link].

[lavalink-repo]: https://github.com/freyacodes/Lavalink
[java-link]: https://www.oracle.com/java/technologies/downloads/