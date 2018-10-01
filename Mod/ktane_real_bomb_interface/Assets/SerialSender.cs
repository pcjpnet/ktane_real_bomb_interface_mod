using System;
using System.IO;
using System.IO.Ports;
using UnityEngine;
//using Newtonsoft.Json;


public class SerialSender : MonoBehaviour {
	
	KMGameInfo gameInfo;
	KMBombInfo bombInfo;
	
	private string folderPath;
	
	private SerialPort uart;
	
	private KMGameInfo.State state_game;
	private bool state_alarm;
	private bool state_light;
	
	private float state_time;
	private string state_fmtTime;
	private int state_strikes;
	private int state_solvableModulesCnt;
	private int state_solvedModulesCnt;
	private bool state_isBombPresent;


	private void Log(string text)
	{
		File.AppendAllText(folderPath + "Log.txt", DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ") + text + Environment.NewLine);
	}


	private void listPorts()
	{
		
		File.WriteAllText(folderPath + "SerialPortList.txt", DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ") + "SerialPort List" + Environment.NewLine);
		string[] portNames = SerialPort.GetPortNames();
		foreach (string name in portNames)
		{
			File.AppendAllText(folderPath + "SerialPortList.txt", name + Environment.NewLine);
		}
	}


	private void writeLine(string line)
	{
		
		uart.WriteLine(line);
		
	}


	private void Awake()
	{
		// Create Directory
		// C:\Users\USER\AppData\LocalLow\Steel Crate Games\Keep Talking and Nobody Explodes\real_bomb_interface\
		folderPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "real_bomb_interface" + Path.DirectorySeparatorChar;
		(new FileInfo(folderPath)).Directory.Create();
		Log("Mod loaded");
		
		listPorts();
		Log("Listed SerialPorts");
		
		gameInfo = this.GetComponent<KMGameInfo>();
		gameInfo.OnStateChange += OnStateChange;
		gameInfo.OnAlarmClockChange += OnAlarmClockChange;
		gameInfo.OnLightsChange += OnLightsChange;
		
		bombInfo = this.GetComponent<KMBombInfo>();
		bombInfo.OnBombExploded += OnBombExploded;
		bombInfo.OnBombSolved += OnBombSolved;
		
		Log("Registered Callback Functions");
		
		uart = new SerialPort("COM3", 9600);
		uart.ReadTimeout = 50;
		uart.NewLine = "\r\n";
		uart.Open();
		writeLine("!PORT-OPEN");
		
		Log("UART:Opened");
		
		settings = JsonConvert.DeserializeObject<RealBombSettings>(modSettings.Settings);
		Log("Loaded");
		Log("Settings: " + settings.ip.ToString());
		
		
		//C:/Users/USER/AppData/LocalLow/Steel Crate Games/Keep Talking and Nobody Explodes/Modsettings/real_bomb_interface-settings.txt
		
		
		
		
		
		
		
		
		
		
	}


	private void Update()
	{
		if (bombInfo != null && bombInfo.IsBombPresent()) {
			
			if (state_time != bombInfo.GetTime()) {
				state_time = bombInfo.GetTime();
				//Log("GetTime: " + state_time.ToString("0000"));
			}
			
			if (state_fmtTime != bombInfo.GetFormattedTime() && !String.IsNullOrEmpty(bombInfo.GetFormattedTime())) {
				state_fmtTime = bombInfo.GetFormattedTime();
				writeLine("!TIME" + state_fmtTime);
				Log("GetFormattedTime: " + state_fmtTime);
			}
			
			if (state_strikes != bombInfo.GetStrikes()) {
				state_strikes = bombInfo.GetStrikes();
				writeLine("!STRIKE" + state_strikes.ToString("000"));
				Log("GetStrikes: " + state_strikes.ToString());
			}
			
			
			
			if (state_solvableModulesCnt != bombInfo.GetSolvableModuleNames().Count) {
				state_solvableModulesCnt = bombInfo.GetSolvableModuleNames().Count;
				writeLine("!MODULE" + state_solvableModulesCnt.ToString("000"));
				Log("GetSolvableModuleNamesCount: " + state_solvableModulesCnt.ToString());
			}
			
			if (state_solvedModulesCnt != bombInfo.GetSolvedModuleNames().Count) {
				state_solvedModulesCnt = bombInfo.GetSolvedModuleNames().Count;
				writeLine("!MSOLVE" + state_solvedModulesCnt.ToString("000"));
				Log("GetSolvedModuleNamesCount: " + state_solvedModulesCnt.ToString());
			}
			
			if (state_isBombPresent != bombInfo.IsBombPresent()) {
				state_isBombPresent = bombInfo.IsBombPresent();
				if (state_isBombPresent) {
					writeLine("!BOMB-TRUE");
				} else {
					writeLine("!BOMB-FALS");
				}
				Log("IsBombPresent: " + state_isBombPresent.ToString());
			}
			
		}
	}



	// ========== Callback Functions ========== //


	// Called when game state changes between gameplay, setup, postgame and loading
	// 0 [Gameplay] The gameplay state where defusing happens
	// 1 [Setup] The setup state in the office where options are chosen
	// 2 [PostGame] The state where results are shown
	// 3 [Transitioning] No current state, transitioning to a new state
	// 4 [Unlock] The unlock state where manual verification and tutorial take place
	// 5 [Quitting] Game is exiting
	protected void OnStateChange(KMGameInfo.State state) {
		if (state_game != state) {
			state_game = state;
			if (state_game.ToString() == "Quitting") {
				writeLine("!PORT-CLSE");
				uart.Close();
				Log("UART:Closed");
			} else {
				writeLine("!KM-STATE" + ((int)state_game).ToString());
			}
			Log("OnStateChange:" + state_game.ToString());
		}
	}


	// Called when alarm clock turns on or off
	protected void OnAlarmClockChange(bool state) {
		if (state_alarm != state) {
			state_alarm = state;
			if (state_alarm) {
				writeLine("!ALARMTRUE");
			} else {
				writeLine("!ALARMFALS");
			}
			Log("OnAlarmClockChange:" + state_alarm.ToString());
		}
	}


	// Called when in game lights change state
	protected void OnLightsChange(bool state) {
		if (state_light != state) {
			state_light = state;
			if (state_light) {
				writeLine("!LIGHTTRUE");
			} else {
				writeLine("!LIGHTFALS");
			}
			Log("OnLightsChange:" + state_light.ToString());
		}
	}


	protected void OnBombExploded() {
		writeLine("!BOMBEXPLD");
		Log("OnBombExploded");
	}


	protected void OnBombSolved() {
		writeLine("!BOMBSOLVD");
		Log("OnBombSolved");
	}


}

