---
title: How to Setup Lavalink
---

[Lavalink][lavalink-repo] is a standalone audio sending node. It's required to use FlawBOT's [music commands][music-link].
FlawBOT 5.0 uses **Lavalink v4**. Older Lavalink v3 servers are no longer supported.

1. Install [Java 17 or newer][java-link].
2. Download **Lavalink.jar** from the [latest Lavalink v4 release][lavalink-releases].
3. Put **Lavalink.jar** in the same folder as the **application.yml** file that ships with FlawBOT (in the `Resources` folder).
4. Open **application.yml** and change `address` to the IP of the machine you'll be running Lavalink from. Use `0.0.0.0` to listen on every network interface. Also change the `password`.
```yml
server: # REST and WS server
  port: 2333
  address: 192.168.2.38
lavalink:
  server:
    password: "youshallnotpass"
```
5. Open **config.json** and change `Address`, `Port` and `Password` to match the values set in Step 4. Set `Enabled` to true.
```json
"Lavalink": {
   "Enabled": true,
   "Address": "192.168.2.38",
   "Port": 2333,
   "Password": "youshallnotpass"
}
```
6. Open the command prompt in that folder and start Lavalink by entering: `java -jar Lavalink.jar`
7. Start FlawBOT. The `/music` commands are only registered when Lavalink is enabled.

### YouTube playback
YouTube support in Lavalink v4 is provided by the [youtube-source plugin][youtube-plugin]. The included **application.yml** already
enables it and disables Lavalink's old built-in YouTube source. Lavalink downloads the plugin automatically on first start.

YouTube changes often, so if playback stops working, update the plugin version in **application.yml** to the
[latest release][youtube-releases] and restart Lavalink:
```yml
lavalink:
  plugins:
    - dependency: "dev.lavalink.youtube:youtube-plugin:1.18.2"
```

:::note
Lavalink must be running before you use any music command. If FlawBOT can't connect, it logs
"Exception occurred when connecting the lavalink node." and the other commands keep working.
:::

[lavalink-repo]: https://github.com/lavalink-devs/Lavalink
[lavalink-releases]: https://github.com/lavalink-devs/Lavalink/releases/latest
[youtube-plugin]: https://github.com/lavalink-devs/youtube-source
[youtube-releases]: https://github.com/lavalink-devs/youtube-source/releases
[java-link]: https://adoptium.net/
[music-link]: https://criticalflaw.ca/FlawBOT/modules/music/
