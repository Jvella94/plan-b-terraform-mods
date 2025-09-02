# plan-b-terraform-mods
Unity/BepInEx mods for the game **Plan B Terraform** [@ Steam](https://store.steampowered.com/app/1894430/Plan_B_Terraform/)

Many thanks to <a href='https://github.com/akarnokd/plan-b-terraform-mods/tree/main'>akarnokd</a> for all his mods he had already made to help me get started.

## Version <a href='https://github.com/JVella94/plan-b-terraform-mods/releases'><img src='https://img.shields.io/github/v/release/JVella94/plan-b-terraform-mods' alt='Latest GitHub Release Version'/></a>

[![Github All Releases](https://img.shields.io/github/downloads/JVella94/plan-b-terraform-mods/total.svg)](https://github.com/JVella94/plan-b-terraform-mods/releases)

## Supported version: Release (v1.0 build 1166)

### Notable file paths

#### Game install directory

**(for example)**

`c:\Program Files (x86)\Steam\steamapps\common\Plan B Terraform\`

#### Save file location

`%USERPROFILE%\AppData\LocalLow\Gaddy Games\Plan B Terraform\{yoursteamid}\Saves`

:information_source: where `{yoursteamid}` is a bunch of numbers representing your Steam Account ID.

#### Game log location

`%USERPROFILE%\AppData\LocalLow\Gaddy Games\Plan B Terraform\Player.log`

#### BepInEx log location

**(depending on your game's install directory)**

`c:\Program Files (x86)\Steam\steamapps\common\Plan B Terraform\BepInEx\LogOutput.log`

#### Plugin directory

**(depending on your game's install directory)**

`c:\Program Files (x86)\Steam\steamapps\common\Plan B Terraform\BepInEx\plugins`

#### Plugin config directory

**(depending on your game's install directory)**

`c:\Program Files (x86)\Steam\steamapps\common\Plan B Terraform\BepInEx\config`

## Installation

1. *[One time only]* Download the 64-bit **BepInEx 5.4.21+** from [BepInEx releases](https://github.com/BepInEx/BepInEx/releases)
    - Make sure you **don't download** the latest, which is the 6.x.y line.
    - Make sure you download the correct version for your operating system.
2. *[One time only]* Unpack the BepInEx zip into the game's folder
    - Example: `c:\Program Files (x86)\Steam\steamapps\common\Plan B Terraform\`
3. *[One time only]* Run the game. Quit the game
    - You should now see the `BepInEx\plugins` directory in the game's directory
4. Unpack the mod zip into the `BepInEx\plugins` directory.
    - [Visit the download page](https://github.com/JVella94/plan-b-terraform-mods/releases/latest) and **look for the `Averax-YYYYY.zip` files!**
    - I highly recommend keeping the directory structure of the zip intact, so, for example, it will look like `BepInEx\plugins\Averax - (Cheat) Find the Secret Treasures`
    - It makes updating or removing mods much easier.
5. If the mods don't appear to work, check the `BepInEx\OutputLog.log` for errors.
    - Also check the game's own log in `%USERPROFILE%\AppData\LocalLow\Gaddy Games\Plan B Terraform\Player.log`
6. Many mods have configuration files you can edit under `BepInEx\config`.
    - *[Once per mod]* For the config file to show up, run the game and quit in the main menu.
    - The config file will be under the `BepInEx\config` (for example, `BepInEx\config\Averax.TresureMap.cfg`). You can edit with any text editor.
    - If something stops working, delete the `cfg` file and the mod will create a default one the next time the game is run.

## Uninstallation

1. Locate the `BepInEx\plugins` directory (or files if you haven't kept the directory structure from the zip).
   - Example: `c:\Program Files (x86)\Steam\steamapps\common\Plan B Terraform\BepInEx\plugins`
2. Delete the plugin's directory, including all files inside it
   - Example: `BepInEx\plugins\Averax - (Cheat) Find the Secret Treasures`
3. *[Optional]* Delete the mod's configuration from the `BepInEx\config` directory
   - Example: `BepInEx\config\averax.TreasureMap.cfg`

# Current Mods

:information_source: Latest Downloads: <a href='https://github.com/Jvella94/plan-b-terraform-mods/releases/latest'><img src='https://img.shields.io/github/v/release/Jvella94/plan-b-terraform-mods' alt='Latest GitHub Latest Release Version'/></a>

### Features

### Cheats

- [Treasure Map](#treasure-map) - Helps finding the locations of the secrets on the map.

### Discontinued

# Mod details

## Treasure Map

Allows finding the locations of the secrets on the map.

A panel will pop up showing you that a secret is nearby (using your mouse position as reference)

Visiblity Types:

(Note the below zoom estimates are using a 27 inch 1440p screen, different sized screens might notice the distances meantioned aren't the same)

- <b>Default</b>: Checks if mouse position is within 26 columns and 21 rows. (Roughly within the screen when doing a 2* zoom out level from closes ground)
- <b>FarAboveGround</b>: Checks if mouse position is within 111 columns and 71 rows. (Roughly within the screen  2* zoom in level from planet level, it's when you first can select objects on the ground)
- <b>MagnifyingGlass</b>: Checks if mouse position is within 6 columns and 6 rows. (Roughly within the screen when doing a full zoom in)
- <b>PlanetWide</b>: Will always show the secrets, regardless of mouse position.

Works fine with Akarnokd's <a href='https://github.com/akarnokd/plan-b-terraform-mods/blob/main/README.md#navigate-to-points-of-interest'>Navigate to Points of Interest</a>

#### Configuration

<details><summary><code>averax.TreasureMap.cfg</code></summary>

```
[General]

## Enable/Disable this mod
# Setting type: Boolean
# Default value: true
ModEnabled = true

## The font size of the panel text
# Setting type: Int32
# Default value: 20
FontSize = 20

## The top position of the panel relative to the top of the screen
# Setting type: Int32
# Default value: 620
PanelTop = 620

## The key to show/hide the panel
# Setting type: KeyCode
# Default value: T
# Acceptable values: None, Backspace, Tab, Clear, Return, Pause, Escape, Space, Exclaim, DoubleQuote, Hash, Dollar, Percent, Ampersand, Quote, LeftParen, RightParen, Asterisk, Plus, Comma, Minus, Period, Slash, Alpha0, Alpha1, Alpha2, Alpha3, Alpha4, Alpha5, Alpha6, Alpha7, Alpha8, Alpha9, Colon, Semicolon, Less, Equals, Greater, Question, At, LeftBracket, Backslash, RightBracket, Caret, Underscore, BackQuote, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, LeftCurlyBracket, Pipe, RightCurlyBracket, Tilde, Delete, Keypad0, Keypad1, Keypad2, Keypad3, Keypad4, Keypad5, Keypad6, Keypad7, Keypad8, Keypad9, KeypadPeriod, KeypadDivide, KeypadMultiply, KeypadMinus, KeypadPlus, KeypadEnter, KeypadEquals, UpArrow, DownArrow, RightArrow, LeftArrow, Insert, Home, End, PageUp, PageDown, F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15, Numlock, CapsLock, ScrollLock, RightShift, LeftShift, RightControl, LeftControl, RightAlt, LeftAlt, RightCommand, RightMeta, RightApple, LeftCommand, LeftMeta, LeftApple, LeftWindows, RightWindows, AltGr, Help, Print, SysReq, Break, Menu, WheelUp, WheelDown, Mouse0, Mouse1, Mouse2, Mouse3, Mouse4, Mouse5, Mouse6, JoystickButton0, JoystickButton1, JoystickButton2, JoystickButton3, JoystickButton4, JoystickButton5, JoystickButton6, JoystickButton7, JoystickButton8, JoystickButton9, JoystickButton10, JoystickButton11, JoystickButton12, JoystickButton13, JoystickButton14, JoystickButton15, JoystickButton16, JoystickButton17, JoystickButton18, JoystickButton19, Joystick1Button0, Joystick1Button1, Joystick1Button2, Joystick1Button3, Joystick1Button4, Joystick1Button5, Joystick1Button6, Joystick1Button7, Joystick1Button8, Joystick1Button9, Joystick1Button10, Joystick1Button11, Joystick1Button12, Joystick1Button13, Joystick1Button14, Joystick1Button15, Joystick1Button16, Joystick1Button17, Joystick1Button18, Joystick1Button19, Joystick2Button0, Joystick2Button1, Joystick2Button2, Joystick2Button3, Joystick2Button4, Joystick2Button5, Joystick2Button6, Joystick2Button7, Joystick2Button8, Joystick2Button9, Joystick2Button10, Joystick2Button11, Joystick2Button12, Joystick2Button13, Joystick2Button14, Joystick2Button15, Joystick2Button16, Joystick2Button17, Joystick2Button18, Joystick2Button19, Joystick3Button0, Joystick3Button1, Joystick3Button2, Joystick3Button3, Joystick3Button4, Joystick3Button5, Joystick3Button6, Joystick3Button7, Joystick3Button8, Joystick3Button9, Joystick3Button10, Joystick3Button11, Joystick3Button12, Joystick3Button13, Joystick3Button14, Joystick3Button15, Joystick3Button16, Joystick3Button17, Joystick3Button18, Joystick3Button19, Joystick4Button0, Joystick4Button1, Joystick4Button2, Joystick4Button3, Joystick4Button4, Joystick4Button5, Joystick4Button6, Joystick4Button7, Joystick4Button8, Joystick4Button9, Joystick4Button10, Joystick4Button11, Joystick4Button12, Joystick4Button13, Joystick4Button14, Joystick4Button15, Joystick4Button16, Joystick4Button17, Joystick4Button18, Joystick4Button19, Joystick5Button0, Joystick5Button1, Joystick5Button2, Joystick5Button3, Joystick5Button4, Joystick5Button5, Joystick5Button6, Joystick5Button7, Joystick5Button8, Joystick5Button9, Joystick5Button10, Joystick5Button11, Joystick5Button12, Joystick5Button13, Joystick5Button14, Joystick5Button15, Joystick5Button16, Joystick5Button17, Joystick5Button18, Joystick5Button19, Joystick6Button0, Joystick6Button1, Joystick6Button2, Joystick6Button3, Joystick6Button4, Joystick6Button5, Joystick6Button6, Joystick6Button7, Joystick6Button8, Joystick6Button9, Joystick6Button10, Joystick6Button11, Joystick6Button12, Joystick6Button13, Joystick6Button14, Joystick6Button15, Joystick6Button16, Joystick6Button17, Joystick6Button18, Joystick6Button19, Joystick7Button0, Joystick7Button1, Joystick7Button2, Joystick7Button3, Joystick7Button4, Joystick7Button5, Joystick7Button6, Joystick7Button7, Joystick7Button8, Joystick7Button9, Joystick7Button10, Joystick7Button11, Joystick7Button12, Joystick7Button13, Joystick7Button14, Joystick7Button15, Joystick7Button16, Joystick7Button17, Joystick7Button18, Joystick7Button19, Joystick8Button0, Joystick8Button1, Joystick8Button2, Joystick8Button3, Joystick8Button4, Joystick8Button5, Joystick8Button6, Joystick8Button7, Joystick8Button8, Joystick8Button9, Joystick8Button10, Joystick8Button11, Joystick8Button12, Joystick8Button13, Joystick8Button14, Joystick8Button15, Joystick8Button16, Joystick8Button17, Joystick8Button18, Joystick8Button19, F16, F17, F18, F19, F20, F21, F22, F23, F24
TogglePanelKey = T

## Scale the position and size of the button with the UI scale of the game?
# Setting type: Boolean
# Default value: true
AutoScale = true

## The names of the secrets are hidden by default.
# Setting type: Boolean
# Default value: true
HideSecretNames = true

## Allows the user to click the notification to go straight to the secret.
# Setting type: Boolean
# Default value: false
ClickToSecretEnabled = false

## How close the mouse has to be for the notification to pop up. Check Mod Info for detailed explanation of choices.
# Setting type: VisibilityTypes
# Default value: Default
# Acceptable values: Default, PlanetWide, FarAboveGround, MagnifyingGlass
VisiblityType = Default
```
</details>
