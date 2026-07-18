# Aura

A super lightweight automatic theme changer for Windows 10.

## Features

- 🎉 Change Window light/dark theme based on set times
- 🎉 Change the Windows theme to a custom theme
- 🎉 Change system or app color theme
- 🎉 Change wallpaper for light/dark theme

## Description

Aura is a super lightweight app built using C# and WPF technologies. It creates task schedules for light/dark theme times. There are no running processes in the background and there is no need to start when Windows starts.

## Command line parameters

| Parameter | Description                                             |
| --------- | ------------------------------------------------------- |
| `/light`  | Switches to the light theme based on the saved settings |
| `/dark`   | Switches to the dark theme based on the saved settings  |
| `/update` | Silently checks for an update and installs it           |
| `/clean`  | Cleans all task schedules created by the app            |

## Build

Clone and open the solution in Visual Studio. Right-click on the project and click `Restore NuGet Packages`.
You can also rely on the included GitHub Actions workflow to build the project automatically on push.

## Creator

Created by [VizXtreme](https://github.com/VizXtreme).
