﻿using UnityEngine;
using Ramses.Confactory;
using Ramses.Confactory.Addons;
using System.Collections.Generic;
using System;

public class AudioLibrary : ScriptableObject, IConfactory, IConStructUser {

	[SerializeField]
	private AudioInfo[] allAudio;

	private Dictionary<string, AudioClip> audioDic = null;

	public void ConClear()
	{

	}

	public IConfactory ConStruct(IConfactoryFinder confactoryFinder)
	{
		AudioLibrary Library = Resources.Load<AudioLibrary>("Libraries/AudioLibrary");
		if(Library == null)
		{
			Debug.LogError("AudioLibrary not found at: " + "Resources/Libraries/AudioLibrary");
			return null;
		}
		return Library;
    }

	public AudioClip GetAudioClip(string nameAudio)
	{
		if(audioDic == null)
		{
			audioDic = new Dictionary<string, AudioClip>();
			foreach(AudioInfo info in allAudio)
			{
				if (!audioDic.ContainsKey(info.Name))
				{
					audioDic.Add(info.Name, info.AudioClip);
				}
				else
				{
					Debug.LogError("Cannot have multiple audioclips in AudioLibrary with the name: '" + info.Name+"'!");
				}
			}
		}
		if (audioDic.ContainsKey(nameAudio))
		{
			return audioDic[nameAudio];
		}else
		{
			Debug.LogError("Could not find audio with name: " + nameAudio + "! Be sure the audioLibrary item you desire has the name given!");
			return null;
		}
    }

	[Serializable]
	private class AudioInfo
	{
		[SerializeField]
		public string Name;
		[SerializeField]
		public AudioClip AudioClip;
	}
}
