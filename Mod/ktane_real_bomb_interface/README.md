# KTaNE Real Bomb Interface Mod


## require
 - Unity 2017.1.0p4


## source
https://github.com/keeptalkinggame/ktanemodkit/wiki


# How to Build Mod

1. Install Unity 2017.1.0p4.
2. Open "Mod/ktane_real_bomb_interface" folder with Unity 2017.1.0p4.
3. Press menu "Keep Talking ModKit" -> "Build AssetBundle" or F6 key.
4. Build in the "Mod/ktane_real_bomb_interface/build" folder.


# How to Install Mod

Copy the "real_bomb_interface" folder to the Mod folder.

Typically Mod folder path.

C:\Program Files (x86)\Steam\steamapps\common\Keep Talking and Nobody Explodes\mods


# How to Use Mod

When loaded, configuration file is created in the Modsettings folder.

Typically Modsettings folder path.

C:\Users\USER\AppData\LocalLow\Steel Crate Games\Keep Talking and Nobody Explodes

Edit the configuration file "real_bomb_interface-settings.txt".

At a minimum, set the serial port to output data.

If "uart_list_enable" is "true", the recognized serial ports are listed in the file.

C:\Users\pcjpnet\AppData\LocalLow\Steel Crate Games\Keep Talking and Nobody Explodes\real_bomb_interface\SerialPortList.txt

Set the serial port name to be used to "uart_name".


If "log_enable" is "true", Log.txt is generated in the following location.

C:\Users\USER\AppData\LocalLow\Steel Crate Games\Keep Talking and Nobody Explodes\real_bomb_interface\


If "debug_log_enable" is "true", it is logged in the game log file.

Typically game log file path.

C:\Program Files (x86)\Steam\steamapps\common\Keep Talking and Nobody Explodes\ktane_Data\output_log.txt


