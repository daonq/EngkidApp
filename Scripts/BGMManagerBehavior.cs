using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManagerBehavior : MonoBehaviour
{
    #region Singleton
    private static BGMManagerBehavior _Instance;
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public static BGMManagerBehavior GetInstance()
    {
        if (_Instance == null)
            Debug.LogError("Error: BGMManagerBehavior instance is null.");

        return _Instance;
    }
    #endregion

    [Header("Audio Player Settings")]
    public AudioSource m_BGMAudioSource;
    public AudioClip m_KidZoneBGM;

    [Header("Common Audio Clips")]
    public AudioClip m_ScrollviewSwipeClip;
    public AudioClip m_ButtonTapClip;
    public AudioClip m_ButtonBackClip;

    public void PauseBGM()
    {
        float start_volume = m_BGMAudioSource.volume;
        LeanTween.value(this.gameObject, start_volume, 0.0f, 1.0f).setOnUpdate((float val) =>
        {
            m_BGMAudioSource.volume = val;
        }).setOnComplete(() =>
        {
            m_BGMAudioSource.Pause();
        });
    }

    public void StopBGM()
    {
        float start_volume = m_BGMAudioSource.volume;
        LeanTween.value(this.gameObject, start_volume, 0.0f, 1.0f).setOnUpdate((float val) =>
        {
            m_BGMAudioSource.volume = val;
        }).setOnComplete(() =>
        {
            m_BGMAudioSource.Stop();
        });
    }    

    public void ResumeBGM()
    {
        LeanTween.value(this.gameObject, 0.0f, 1.0f, 1.0f).setOnUpdate((float val) =>
        {
            m_BGMAudioSource.volume = val;
        }).setOnComplete(() =>
        {
            m_BGMAudioSource.Play();
        });
    }

    public void PlayBGM(AudioClip new_bgm)
    {
        if (m_BGMAudioSource.clip == new_bgm)
            return;

        float start_volume = m_BGMAudioSource.volume;
        ChangeBGM(new_bgm, start_volume, 1.0f);
    }

    public void SetBGMVolume(float start_vol, float new_vol = 0.0f)
    {
        LeanTween.value(this.gameObject, start_vol, new_vol, 1.0f).setOnUpdate((float val) =>
        {
            m_BGMAudioSource.volume = val;
        });
    }    

    private void ChangeBGM(AudioClip new_bgm, float start_volume, float dur)
    {
        if (m_BGMAudioSource.isPlaying == false)
        {
            FadeInNewBGM(new_bgm, start_volume, dur);
            return;
        }

        LeanTween.value(this.gameObject, start_volume, 0.0f, dur).setOnUpdate((float val) =>
        {
            m_BGMAudioSource.volume = val;
        }).setOnComplete(() =>
        {
            FadeInNewBGM(new_bgm, start_volume, dur);
        });
    }

    private void FadeInNewBGM(AudioClip new_bgm, float start_volume, float dur)
    {
        m_BGMAudioSource.Stop();

        m_BGMAudioSource.clip = new_bgm;
        m_BGMAudioSource.Play();
        m_BGMAudioSource.loop = true;

        LeanTween.value(this.gameObject, 0.0f, start_volume, dur).setOnUpdate((float val) =>
        {
            m_BGMAudioSource.volume = val;
        });
    }
}
