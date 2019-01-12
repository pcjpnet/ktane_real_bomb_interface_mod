using System;
using System.IO;
using System.IO.Ports;
using UnityEngine;
using Newtonsoft.Json;


public class SerialSender : MonoBehaviour {
	
	public KMGameInfo gameInfo;
	public KMBombInfo bombInfo;
	public KMModSettings modSettings;
	
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
	
	class Settings {
		public bool uart_enable;
		public bool uart_list_enable;
		public string uart_name;
		public int uart_baud;
		public bool log_enable;
		public bool debug_log_enable;
	}
	
	Settings clsSettings;
	
	private bool set_uart_enable = false;
	private bool set_uart_list_enable = false;
	private string set_uart_name;
	private int set_uart_baud = 9600;
	private bool set_log_enable = false;
	private bool set_debug_log_enable = false;


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


	private void Start()
	{
		// Load Settings
		//C:\Users\USER\AppData\LocalLow\Steel Crate Games\Keep Talking and Nobody Explodes\Modsettings\real_bomb_interface-settings.txt
		clsSettings = JsonConvert.DeserializeObject<Settings>(modSettings.Settings);
		set_uart_enable = clsSettings.uart_enable;
		set_uart_list_enable = clsSettings.uart_list_enable;
		set_uart_name = clsSettings.uart_name;
		set_uart_baud = clsSettings.uart_baud;
		set_log_enable = clsSettings.log_enable;
		set_debug_log_enable = clsSettings.debug_log_enable;
		
		// Create Directory
		// C:\Users\USER\AppData\LocalLow\Steel Crate Games\Keep Talking and Nobody Explodes\real_bomb_interface\
		folderPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "real_bomb_interface" + Path.DirectorySeparatorChar;
		if (set_log_enable || set_uart_list_enable) (new FileInfo(folderPath)).Directory.Create();
		if (set_log_enable) Log("Mod loaded");
		if (set_debug_log_enable) Debug.Log("Mod loaded");
		
		// List SerialPorts
		if (set_uart_list_enable) listPorts();
		if (set_log_enable) Log("Listed SerialPorts");
		if (set_debug_log_enable) Debug.Log("Listed SerialPorts");
		
		// Register Callback Functions
		gameInfo.OnStateChange += OnStateChange;
		gameInfo.OnAlarmClockChange += OnAlarmClockChange;
		gameInfo.OnLightsChange += OnLightsChange;
		bombInfo.OnBombExploded += OnBombExploded;
		bombInfo.OnBombSolved += OnBombSolved;
		if (set_log_enable) Log("Registered Callback Functions");
		if (set_debug_log_enable) Debug.Log("Registered Callback Functions");
		
		// Open SerialPort
		if (set_uart_enable) {
			uart = new SerialPort(set_uart_name, set_uart_baud);
			uart.ReadTimeout = 50;
			uart.NewLine = "\n";
			uart.Open();
			writeLine("!PORT-OPEN");
			if (set_log_enable) Log("UART:Opened");
			if (set_debug_log_enable) Debug.Log("UART:Opened");
		}
		
	}


	private void Update()
	{
		if (bombInfo != null && bombInfo.IsBombPresent()) {
			
			if (state_time != bombInfo.GetTime()) {
				state_time = bombInfo.GetTime();
				//if (set_log_enable) Log("GetTime: " + state_time.ToString("0000"));
				//if (set_debug_log_enable) Debug.Log("GetTime: " + state_time.ToString("0000"));
			}
			
			if (state_fmtTime != bombInfo.GetFormattedTime() && !String.IsNullOrEmpty(bombInfo.GetFormattedTime())) {
				state_fmtTime = bombInfo.GetFormattedTime();
				if (set_uart_enable) writeLine("!TIME" + state_fmtTime);
				if (set_log_enable) Log("GetFormattedTime: " + state_fmtTime);
				if (set_debug_log_enable) Debug.Log("GetFormattedTime: " + state_fmtTime);
			}
			
			if (state_strikes != bombInfo.GetStrikes()) {
				state_strikes = bombInfo.GetStrikes();
				if (set_uart_enable) writeLine("!STRIKE" + state_strikes.ToString("000"));
				if (set_log_enable) Log("GetStrikes: " + state_strikes.ToString());
				if (set_debug_log_enable) Debug.Log("GetStrikes: " + state_strikes.ToString());
			}
			
			if (state_solvableModulesCnt != bombInfo.GetSolvableModuleNames().Count) {
				state_solvableModulesCnt = bombInfo.GetSolvableModuleNames().Count;
				if (set_uart_enable) writeLine("!MODULE" + state_solvableModulesCnt.ToString("000"));
				if (set_log_enable) Log("GetSolvableModuleNamesCount: " + state_solvableModulesCnt.ToString());
				if (set_debug_log_enable) Debug.Log("GetSolvableModuleNamesCount: " + state_solvableModulesCnt.ToString());
			}
			
			if (state_solvedModulesCnt != bombInfo.GetSolvedModuleNames().Count) {
				state_solvedModulesCnt = bombInfo.GetSolvedModuleNames().Count;
				if (set_uart_enable) writeLine("!MSOLVE" + state_solvedModulesCnt.ToString("000"));
				if (set_log_enable) Log("GetSolvedModuleNamesCount: " + state_solvedModulesCnt.ToString());
				if (set_debug_log_enable) Debug.Log("GetSolvedModuleNamesCount: " + state_solvedModulesCnt.ToString());
			}
			
			if (state_isBombPresent != bombInfo.IsBombPresent()) {
				state_isBombPresent = bombInfo.IsBombPresent();
				if (state_isBombPresent) {
					if (set_uart_enable) writeLine("!BOMB-TRUE");
				} else {
					if (set_uart_enable) writeLine("!BOMB-FALS");
				}
				if (set_log_enable) Log("IsBombPresent: " + state_isBombPresent.ToString());
				if (set_debug_log_enable) Debug.Log("IsBombPresent: " + state_isBombPresent.ToString());
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
				if (set_uart_enable) writeLine("!PORT-CLSE");
				if (set_uart_enable) uart.Close();
				if (set_log_enable) Log("UART:Closed");
				if (set_debug_log_enable) Debug.Log("UART:Closed");
			} else {
				if (set_uart_enable) writeLine("!KM-STATE" + ((int)state_game).ToString());
			}
			if (set_log_enable) Log("OnStateChange:" + ((int)state_game).ToString() + " " + state_game.ToString());
			if (set_debug_log_enable) Debug.Log("OnStateChange:" + ((int)state_game).ToString() + " " + state_game.ToString());
		}
	}


	// Called when alarm clock turns on or off
	protected void OnAlarmClockChange(bool state) {
		if (state_alarm != state) {
			state_alarm = state;
			if (state_alarm) {
				if (set_uart_enable) writeLine("!ALARMTRUE");
			} else {
				if (set_uart_enable) writeLine("!ALARMFALS");
			}
			if (set_log_enable) Log("OnAlarmClockChange:" + state_alarm.ToString());
			if (set_debug_log_enable) Debug.Log("OnAlarmClockChange:" + state_alarm.ToString());
		}
	}


	// Called when in game lights change state
	protected void OnLightsChange(bool state) {
		if (state_light != state) {
			state_light = state;
			if (state_light) {
				if (set_uart_enable) writeLine("!LIGHTTRUE");
			} else {
				if (set_uart_enable) writeLine("!LIGHTFALS");
			}
			if (set_log_enable) Log("OnLightsChange:" + state_light.ToString());
			if (set_debug_log_enable) Debug.Log("OnLightsChange:" + state_light.ToString());
		}
	}


	protected void OnBombExploded() {
		if (set_uart_enable) writeLine("!BOMBEXPLD");
		if (set_log_enable) Log("OnBombExploded");
		if (set_debug_log_enable) Debug.Log("OnBombExploded");
	}


	protected void OnBombSolved() {
		if (set_uart_enable) writeLine("!BOMBSOLVD");
		if (set_log_enable) Log("OnBombSolved");
		if (set_debug_log_enable) Debug.Log("OnBombSolved");
	}


}

