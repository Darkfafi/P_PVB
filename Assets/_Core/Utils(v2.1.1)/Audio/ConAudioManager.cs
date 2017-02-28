using UnityEngine;
using Ramses.Confactory.Addons;
using Ramses.Confactory;
using System;
using System.Collections.Generic;

public class ConAudioManager : MonoBehaviour, IConfactory, IConStructUser {

	public const int DEFAULT_STATION = 0;
	public const int EFFECTS_STATION = 1;
	public const int MUSIC_STATION = 2;

	private const float startDefaultVolume = 1f;
	private const float startMusicVolume = 0.45f;
	private const float startEffectsVolume = 0.7f;

	private const float startDefaultPitch = 1.0f;
	private const float startMusicPitch = 1.0f;
	private const float startEffectsPitch = 1.0f;

	public AudioLibrary Library { get; private set; }
	private Dictionary<int, AudioSource> audioStations = new Dictionary<int, AudioSource>();
	private Dictionary<int, StationSettings> audioStationsSettings = new Dictionary<int, StationSettings>();

	public void PlayAudio(string name, int station = 0, float volumeScale = 1)
	{
		PlayAudio(Library.GetAudioClip(name), station, volumeScale);
    }

	public void PlaySoloAudio(string name, int station = 0, float volumeScale = 1, float delayInSeconds = 0)
	{
		PlaySoloAudio(Library.GetAudioClip(name), station, volumeScale, delayInSeconds);
	}

	public void PlaySoloAudio(AudioClip clip, int station = 0, float volumeScale = 1, float delayInSeconds = 0)
	{
		if (audioStations.ContainsKey(station))
		{
			if (audioStations[station] == null)
			{
				audioStations.Remove(station);
			}
		}
		AudioSource currentStation = GetAudioStation(station);
		currentStation.clip = clip;
		ulong delayFromSecToHz = Convert.ToUInt64(44100 * delayInSeconds);
		currentStation.Play(delayFromSecToHz);
	}

	public void SoloAudioLoopToggle(int station, bool loop)
	{
		GetStationSettings(station).Loop = loop;
		SetSettingsForAudioStation(station);
    }

	public void StopAudio()
	{
		foreach (KeyValuePair<int, AudioSource> pair in audioStations)
		{
			StopAudio(pair.Key);
		}
	}

	public void StopAudio(int station)
	{
		if (audioStations.ContainsKey(station))
		{
			audioStations[station].Stop();
		}
	}

	public void PlayAudio(AudioClip clip, int station = 0, float volumeScale = 1)
	{
		if(audioStations.ContainsKey(station))
		{
			if(audioStations[station] == null)
			{
				audioStations.Remove(station);
			}
        }
		GetAudioStation(station).PlayOneShot(clip, volumeScale);
	}

	public AudioSource GetAudioStation(int station)
	{
		if (!audioStations.ContainsKey(station))
		{
			AudioSource source = gameObject.AddComponent<AudioSource>();
			audioStations.Add(station, source);
			SetSettingsForAudioStation(station);
		}
		return audioStations[station];
	}

	private StationSettings GetStationSettings(int station)
	{
		if (!audioStationsSettings.ContainsKey(station))
		{
			audioStationsSettings.Add(station, new StationSettings());
		}
		return audioStationsSettings[station];
	}

	private void SetSettingsForAudioStation(int station)
	{
		StationSettings settings = GetStationSettings(station);
		AudioSource aStation = GetAudioStation(station);
		aStation.pitch = settings.Pitch;
		aStation.volume = settings.Volume;
		aStation.loop = settings.Loop;
	}

	public float GetAudioStationVolume(int station)
	{
		return GetStationSettings(station).Volume;
	}

	public void SetAudioStationVolume(int station, float volume)
	{
		GetStationSettings(station).Volume = volume;
		SetSettingsForAudioStation(station);
    }

	public float GetAudioStationPitch(int station)
	{
		return GetStationSettings(station).Pitch;
	}

	public void SetAudioStationPitch(int station, float pitch)
	{
		GetStationSettings(station).Pitch = pitch;
		SetSettingsForAudioStation(station);
    }

	public void ConClear()
	{
		
	}

	public IConfactory ConStruct(IConfactoryFinder confactoryFinder)
	{
		Library = confactoryFinder.Get<AudioLibrary>();
        SetAudioStationVolume(DEFAULT_STATION, startDefaultVolume);
		SetAudioStationVolume(EFFECTS_STATION, startEffectsVolume);
		SetAudioStationVolume(MUSIC_STATION, startMusicVolume);
		SetAudioStationPitch(DEFAULT_STATION, startDefaultPitch);
		SetAudioStationPitch(EFFECTS_STATION, startEffectsPitch);
		SetAudioStationPitch(MUSIC_STATION, startMusicPitch);
		SoloAudioLoopToggle(MUSIC_STATION, true);
		return null;
	}

	private class StationSettings
	{
		public float Pitch = 1;
		public float Volume = 1;
		public bool Loop = false;
	}
}
