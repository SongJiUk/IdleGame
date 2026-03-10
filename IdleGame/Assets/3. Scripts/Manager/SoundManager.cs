using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager
{
    public AudioSource[] audioSources = new AudioSource[(int)Define.Sound.Max];
    Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    List<AudioSource> effectSources = new List<AudioSource>();
    private int maxEffectsSources = 10;
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

        GameObject bgmGo = new GameObject { name = "BgmChannel" };
        audioSources[(int)Define.Sound.Bgm] = bgmGo.AddComponent<AudioSource>();
        bgmGo.transform.parent = soundRoot.transform;
        audioSources[(int)Define.Sound.Bgm].loop = true;


        for (int i = 0; i < maxEffectsSources; i++)
        {
            GameObject go = new GameObject { name = $"EffectSource_{i}" };
            go.transform.parent = soundRoot.transform;
            effectSources.Add(go.AddComponent<AudioSource>());
        }
    }
    public void Clear()
    {
        foreach (AudioSource audio in audioSources)
            if (audio != null) audio.Stop();

        foreach (var effectSource in effectSources)
            if (effectSource != null) effectSource.Stop();

        audioClips.Clear();
        Debug.Log("[SoundManager] 모든 사운드 정지 및 캐시 초기화 완료");
    }

    public void Play(Define.Sound _sound, string _label, float _pitch = 1f)
    {

        if (_sound == Define.Sound.Effect)
        {
            AudioSource freeSoucre = effectSources.Find(s => !s.isPlaying);
            if (freeSoucre == null) freeSoucre = effectSources[0];
            PlayInternal(_sound, _label, _pitch, freeSoucre);
        }
        else
        {

            AudioSource audio = audioSources[(int)_sound];
            PlayInternal(_sound, _label, _pitch, audio);
        }
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

        switch (_sound)
        {
            case Define.Sound.Bgm:
                _source.pitch = _pitch;
                if (_source.isPlaying) _source.Stop();
                _source.clip = _clip;
                _source.volume = BgmValue;
                _source.loop = true;
                _source.Play();
                break;

            case Define.Sound.Effect:
                AudioSource availableSource = effectSources.Find(s => !s.isPlaying);
                if (availableSource == null) availableSource = effectSources[0];

                availableSource.clip = _clip;
                availableSource.volume = EffectValue;
                availableSource.Play();

                break;

            default:
                if (PlayerPrefs.GetFloat("EFFECT") > 0)
                    _source.PlayOneShot(_clip);
                break;
        }
    }
    public void PlayButtonClick() => Play(Define.Sound.Effect, "Click");
    public void PlayPopupClose() => Play(Define.Sound.Effect, "PopupClose");

    public void PlayGoldDice() => Play(Define.Sound.Effect, "Dice");



    public void Stop(Define.Sound _sound)
    {
        AudioSource audio = audioSources[(int)_sound];
        if (audio.isPlaying) audio.Stop();
    }

    public void SetEffectVolume(float _value)
    {
        EffectValue = _value;
        foreach (AudioSource source in effectSources)
        {
            if (source != null)
            {
                source.volume = _value;
            }
        }
        PlayerPrefs.SetFloat("EFFECT", _value);
        PlayerPrefs.Save();
    }

    public void MuteEffectVolume(bool _isMute)
    {
        foreach (AudioSource source in effectSources)
        {
            if (source != null)
            {
                source.mute = _isMute;
            }
        }
    }
    public void MuteBgmVolume(bool _isMute)
    {
        audioSources[(int)Define.Sound.Bgm].mute = _isMute;
    }
}
