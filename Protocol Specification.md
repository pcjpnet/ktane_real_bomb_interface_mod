# KTaNE Real Bomb Interface Mod Protocol Specification

Send ASCII code via serial connection.

line feed code is "\r\n";.

The instruction code is based on 12 bytes.

ex:
!PORT-OPEN\r\n

The 1st byte is the ASCII code exclamation mark "!".

and next 9 bytes are instruction code.

last two bytes are line feed codes.

In the case of the "TIME" operation code, the length may not be 9 bytes.

For this reason, it is necessary to read data until a line feed code arrives at the receiving side.



# Instruction Code List

| Data | Description |
----|----
| !PORT-OPEN | Serial port opened (When Mod is loaded) |
| !PORT-CLSE | Serial port closed (When KTaNE game is closed) |
| !BOMB-TRUE | When the bomb is valid |
| !BOMB-FALS | When the bomb is invalid |
| !KM-STATE%1d | When the state of the KTaNE game changes |
| !TIME%s    | Bomb time display contents (ex: 01:00 / 30.00) |
| !STRIKE%3d | Number of times when strike occurred |
| !MODULE%3d | Total number of bomb modules |
| !MSOLVE%3d | Number of bomb modules solved |
| !ALARMTRUE | Alarm sounds |
| !ALARMFALS | Alarm sound stopped |
| !LIGHTTRUE | Light on |
| !LIGHTFALS | Light off |
| !BOMBEXPLD | Bomb exploded |
| !BOMBSOLVD | Bomb solved |




# State Code List

State code 5 is not issued. Since the serial connection is canceled first, "PORT-CLSE" is issued.


| State | Description |
----|----
| 0 [Gameplay] | The gameplay state where defusing happens |
| 1 [Setup] | The setup state in the office where options are chosen |
| 2 [PostGame] | The state where results are shown |
| 3 [Transitioning] | No current state, transitioning to a new state |
| 4 [Unlock] | The unlock state where manual verification and tutorial take place |
| 5 [Quitting] | Game is exiting |



# Communication Example



| Data | Description |
----|----
| - | Loading Mod |
| !PORT-OPEN | Start serial communication |
| !KM-STATE3 | Loading |
| - | Show Office |
| !LIGHTTRUE | Light on |
| !KM-STATE1 | In Office |
| - | Select Bomb and Loading |
| !KM-STATE3 | Loading |
| - | Show bomb room |
| !LIGHTFALS | Light off |
| !LIGHTTRUE | Light on |
| !KM-STATE0 | Game Start |
| !LIGHTFALS | Light off |
| !TIME03:00 | Time displayed on bomb |
| !MODULE003 | Number of bomb modules |
| !BOMB-TRUE | Bomb is valid |
| - | Light turns on after a few seconds |
| !LIGHTTRUE | Light on |
| - | Started bomb timer |
| !TIME02:59 | - |
| !TIME02:58 | - |
| !TIME02:57 | - |
| !STRIKE001 | 1 strike |
| !TIME02:52 | - |
| !TIME02:51 | - |
| !TIME02:50 | - |
| !MSOLVE001 | First module was solved |
| !TIME02:49 | - |
| !TIME02:48 | - |
| !TIME02:47 | - |
| !STRIKE002 | 2 strike |
| !TIME02:45 | - |
| !TIME02:44 | - |
| !TIME02:43 | - |
| !BOMBEXPLD | Bomb exploded |
| - | Result screen loading |
| !KM-STATE3 | Loading |
| - | Show result |
| !KM-STATE2 | Show result screen |
| - | Press the button and loading |
| !KM-STATE3 | Loading |
| - | Show Office and quitting |
| !KM-STATE1 | In Office |
| !KM-STATE3 | Loading |
| !PORT-CLSE | End serial communication |



