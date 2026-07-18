# Aura

A modern, native Windows personalization utility that automates color theme and wallpaper switching. Aura has been rewritten from the ground up using **WinUI 3** and the **Windows App SDK** to deliver a premium, seamless experience on Windows 10 and 11.

---

## 🚀 The WinUI 3 Upgrade

Aura represents a complete modernization of the original utility. Key architectural changes include:
* **Native Fluent Design**: Leverages the Windows UI Library (WinUI 3) to render controls matching Windows 11 design guidelines, support dark/light theme templates natively, and display rounded corners and drop shadows.
* **Modern App Lifecycle**: Rewritten targeting **.NET 10.0** and the SDK-style build system.
* **Non-Blocking Dialogs**: Migrated all modal views (About, Updates, and Alerts) to native `ContentDialog` panels with `XamlRoot` context, avoiding UI thread blocking.
* **Streamlined Engine**: Removed legacy WPF controls and heavy third-party styling packages, resulting in a lightweight, responsive utility with a footprint of nearly zero background resources.
* **Updated Networking**: Replaced deprecated WebClient references with a modern, stream-based `HttpClient` implementation featuring progress reporting.

---

## ✨ Features

* 🌓 **Scheduled Transitions**: Automate switching between Windows Light and Dark modes at designated times.
* 🎨 **Deep Customization**: Toggle individual elements:
  * System theme settings
  * Application theme settings
  * Specific desktop wallpapers for light and dark modes
* 📄 **Windows Theme Support**: Apply custom Windows `.theme` files directly during transitions.
* 🛡️ **Zero Background Footprint**: Instead of running a persistent background process, Aura registers lightweight daily task actions in the Windows Task Scheduler.
* 💻 **Command Line Support**: Run transition events directly via CLI commands.

---

## 🛠️ Installation

1. Download the latest release from the [Releases](https://github.com/VizXtreme/Aura/releases/latest) page.
2. Run the installer and configure your schedule.
3. *Note: Since the installer is self-signed, Windows Defender SmartScreen may display a warning on first run. You can safely proceed.*

---

## 💻 Command Line Interface

Aura supports direct command executions for system scripts or custom automation:

| Parameter | Action |
| --- | --- |
| `/light` | Forces transition to the configured light theme settings |
| `/dark` | Forces transition to the configured dark theme settings |
| `/change` | Triggers a transition check based on the current system clock time |
| `/update` | Silently queries the releases API and initiates updates |
| `/clean` | Cleans up and deletes all scheduled tasks registered by Aura |

---

## 📦 Building Aura

Aura targets the modern Windows App SDK. Ensure you have the .NET 10 SDK installed, then run:

```bash
# Clone the repository
git clone https://github.com/VizXtreme/Aura.git

# Build the project in Release configuration
dotnet build Aura.sln --configuration Release
```

---

## 🤝 Contributing

Contributions, issues, and feature suggestions are welcome! Feel free to open a pull request or report issues on the repository page.

Developed by [VizXtreme](https://github.com/VizXtreme).
