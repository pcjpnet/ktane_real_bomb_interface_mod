using System;
using System.IO;
using System.IO.Ports;
using UnityEngine;


public class SerialSender : MonoBehaviour {

	KMBombInfo bombInfo;


	private void listPorts()
	{
		// C:\Users\USER\AppData\LocalLow\Steel Crate Games\Keep Talking and Nobody Explodes\
		File.WriteAllText(Application.persistentDataPath + "/real_bomb_interface_portlist.txt", DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ") + "Portlist" + "\n");
		string[] portNames = SerialPort.GetPortNames();
		foreach (string name in portNames)
		{
			File.AppendAllText(Application.persistentDataPath + "/real_bomb_interface_portlist.txt", name + "\n");
		}
	}


	private void Log(string text)
	{
		// C:\Users\USER\AppData\LocalLow\Steel Crate Games\Keep Talking and Nobody Explodes\
		File.AppendAllText(Application.persistentDataPath + "/real_bomb_interface.log", DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss] ") + text + "\n");
	}


	private void Awake()
	{
		listPorts();
		Log("Awake");
		
	}


}
