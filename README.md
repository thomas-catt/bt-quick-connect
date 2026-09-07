# BT Quick Connect

A minimal Windows system tray application that connects to a paired Bluetooth device with a single click.

> this project was vibecoded and untested so everything other than this line on this entire repo is just AI.

## Usage

```
BtQuickConnect.exe <search-string>
```

The search string is matched (case-insensitive) against the names of your system's paired Bluetooth devices. Once launched, click the tray icon to trigger a connection.

### Example

```
BtQuickConnect.exe "Sony WH"
```

This finds any paired device whose name contains "Sony WH" and connects to it when you click the tray icon.

## Building

```
dotnet build
```

## Requirements

- Windows 10 (build 19041+)
- .NET 10
