using System;
using System.IO;
using System.IO.Ports;
using UnityEngine;


public class SerialSender : MonoBehaviour {

	KMGameInfo gameInfo;
	KMBombInfo bombInfo;
	
	private string folderPath;
	
	private string state_game;
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
	}


	private void Update()
	{
		if (bombInfo != null) {
			
			
			
			if (state_time != bombInfo.GetTime()) {
				state_time = bombInfo.GetTime();
				//Log("GetTime: " + state_time.ToString("0000"));
			}
			
			if (state_fmtTime != bombInfo.GetFormattedTime() && bombInfo.GetFormattedTime() != null) {
				state_fmtTime = bombInfo.GetFormattedTime();
				Log("GetFormattedTime: " + state_fmtTime);
			}
			
			if (state_strikes != bombInfo.GetStrikes()) {
				state_strikes = bombInfo.GetStrikes();
				Log("GetStrikes: " + state_strikes);
			}
			
			
			
			if (state_solvableModulesCnt != bombInfo.GetSolvableModuleNames().Count) {
				state_solvableModulesCnt = bombInfo.GetSolvableModuleNames().Count;
				Log("GetSolvableModuleNamesCount: " + state_solvableModulesCnt.ToString());
			}
			
			if (state_solvedModulesCnt != bombInfo.GetSolvedModuleNames().Count) {
				state_solvedModulesCnt = bombInfo.GetSolvedModuleNames().Count;
				Log("GetSolvedModuleNamesCount: " + state_solvedModulesCnt.ToString());
			}
			
			if (state_isBombPresent != bombInfo.IsBombPresent()) {
				state_isBombPresent = bombInfo.IsBombPresent();
				Log("IsBombPresent: " + state_isBombPresent.ToString());
			}
			
		}
	}



	// ========== Callback Functions ========== //


	// Called when game state changes between gameplay, setup, postgame and loading
	// [Gameplay] The gameplay state where defusing happens
	// [Setup] The setup state in the office where options are chosen
	// [PostGame] The state where results are shown
	// [Transitioning] No current state, transitioning to a new state
	// [Unlock] The unlock state where manual verification and tutorial take place
	// [Quitting] Game is exiting
	protected void OnStateChange(KMGameInfo.State state) {
		if (state_game != state.ToString()) {
			state_game = state.ToString();
			Log("OnStateChange:" + state_game);
		}
	}


	// Called when alarm clock turns on or off
	protected void OnAlarmClockChange(bool state) {
		if (state_alarm != state) {
			state_alarm = state;
			Log("OnAlarmClockChange:" + state_alarm.ToString());
		}
	}


	// Called when in game lights change state
	protected void OnLightsChange(bool state) {
		if (state_light != state) {
			state_light = state;
			Log("OnLightsChange:" + state_light.ToString());
		}
	}


	protected void OnBombExploded() {
		Log("OnBombExploded");
	}


	protected void OnBombSolved() {
		Log("OnBombSolved");
	}


}

