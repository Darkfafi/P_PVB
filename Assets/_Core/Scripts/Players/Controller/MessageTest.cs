using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class MessageTest : MonoBehaviour {

	public Text SampleText;
	private string _sampleText;

	void Start()
	{
		AirConsole.instance.onMessage += OnMessageEvent;
	}

	void Update()
	{
		SampleText.text = _sampleText;
	}

	public void OnMessageEvent(int from, JToken data)
	{
		_sampleText = (string)data["sendMessage"];
	}
}