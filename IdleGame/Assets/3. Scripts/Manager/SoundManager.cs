using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager
{
    public AudioSource[] audioSources = new AudioSource[(int)Define.Sound.Max];
    Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    GameObject soundRoot = null;

    public float BgmValue, EffectValue;
    public void Init()
    {
        BgmValue = PlayerPrefs.GetFloat("BGM", 1.0f);
        EffectValue = PlayerPrefs.GetFloat("EFFECT", 1.0f);

        if (soundRoot != null) return;
        soundRoot = GameObject.Find("@SoundRoot");
        if (soundRoot == null)
        {
            soundRoot = new GameObject { name = "@SoundRoot" };
            UnityEngine.Object.DontDestroyOnLoad(soundRoot);
        }

        string[] soundTypeNames = Enum.GetNames(typeof(Define.Sound));
        for (int count = 0; count < soundTypeNames.Length - 1; count++)
        {
            GameObject go = new GameObject { name = soundTypeNames[count] };
            audioSources[count] = go.AddComponent<AudioSource>();
            go.transform.parent = soundRoot.transform;
        }
        audioSources[(int)Define.Sound.Bgm].loop = true;

    }
    public void Clear()
    {
        foreach (AudioSource audio in audioSources)
            audio.Stop();

        audioClips.Clear();
    }

    public void Play(Define.Sound _sound, string _label, float _pitch = 1f)
    {
        AudioSource audio = audioSources[(int)_sound];
        PlayInternal(_sound, _label, _pitch, audio);
    }

    private void PlayInternal(Define.Sound _sound, string _key, float _pitch, AudioSource _source)
    {
        if (audioClips.TryGetValue(_key, out var clip) && clip != null)
        {
            DoPlay(_sound, _source, clip, _pitch);
            return;
        }

        var loadedClip = Managers.ResourceM.Load<AudioClip>(_key);
        if (loadedClip == null)
        {
            Debug.LogError($"SoundManger : 해당 키값이 없음 {_key}");
            return;
        }
        audioClips[_key] = loadedClip;
        DoPlay(_sound, _source, loadedClip, _pitch);
    }

    private void DoPlay(Define.Sound _sound, AudioSource _source, AudioClip _clip, float _pitch)
    {
        _source.pitch = _pitch;
        switch (_sound)
        {
            case Define.Sound.Bgm:
                if (_source.isPlaying) _source.Stop();
                _source.clip = _clip;
                _source.volume = BgmValue;
                _source.Play();
                break;

            case Define.Sound.Effect:
                if (_source.isPlaying) _source.Stop();
                _source.clip = _clip;
                _source.volume = EffectValue;
                _source.Play();
                break;

            default:
                if (PlayerPrefs.GetFloat("EFFECT") > 0)
                    _source.PlayOneShot(_clip);
                break;
        }
    }
    public void PlayButtonClick() => Play(Define.Sound.Effect, "Click");
    public void PlayPopupClose() => Play(Define.Sound.Effect, "PopupClose");



    public void Stop(Define.Sound _sound)
    {
        AudioSource audio = audioSources[(int)_sound];
        if (audio.isPlaying) audio.Stop();
    }
}
