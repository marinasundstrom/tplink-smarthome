TP-Link Smart Home Client for .NET
==

Unofficial API for discovering and managing TP-Link Smart Home devices.

This is not tested and not suitable for use in production.

More on Smart Home: http://www.tp-link.com/us/home-networking/smart-home/.

## Device types
Currently, there is basic support for these types of devices:
* Smart Plugs
* Smart Bulbs

## SHClient
The solution includes a sample CLI.

### Plug

```sh
$ dotnet run plug <ip address> [-s <On|Off>]
```

Options

```
--state | -s <On|Off>
--help | -h 
```

### Bulb

```sh
$ dotnet run bulb <ip address> [-s <On|Off>] [-b <0-100>]
```

Options

```
--state | -s <On|Off>
--brightness | -b
--help | -h 
```

### Discover

```sh
$ dotnet run discover [--json]
```

Options

```
--json
```
