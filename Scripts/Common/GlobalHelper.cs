using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GlobalHelper : MonoBehaviour
{
    #region Singleton
    private static GlobalHelper _Instance;
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
    public static GlobalHelper GetInstance()
    {
        if (_Instance == null)
            Debug.LogError("Error: GlobalHelper instance is null.");

        return _Instance;
    }
    #endregion

    Texture2D tex;
    Sprite sprite;

    public void ClearTexture()
    {
        if (tex != null)
            Destroy(tex);

        if (sprite != null)
            Destroy(sprite);
    }

    public void GetSpriteFromURL(string url, System.Action<Sprite> callback_sprite)
    {
        if (string.IsNullOrEmpty(url) == true)
            callback_sprite.Invoke(null);
        else
            StartCoroutine(GetSpriteFromURLSequence(url, callback_sprite));
    }

    public IEnumerator GetSpriteFromURLSequence(string url, System.Action<Sprite> callback_sprite)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        www.timeout = 30;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            callback_sprite.Invoke(null);
        }
        else
        {
            tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            callback_sprite.Invoke(sprite);
        }
    }

    public void GetGetAudioClipFromURL(string link, System.Action<AudioClip> callback_audio_clip)
    {
        if (string.IsNullOrEmpty(link) == true)
            callback_audio_clip.Invoke(null);
        else
            StartCoroutine(GetAudioClipFromURLSequence(link, callback_audio_clip));
    }

    public IEnumerator GetAudioClipFromURLSequence(string link, System.Action<AudioClip> callback_audio_clip)
    {
        //Debug.Log("link audio == " + link);
        if (link.EndsWith(".wav"))
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.WAV))
            {
                www.timeout = 30;
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                    callback_audio_clip.Invoke(null);
                }
                else
                {
                    AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                    callback_audio_clip.Invoke(myClip);
                }
            }
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(link, AudioType.MPEG))
            {
                www.timeout = 30;
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                    callback_audio_clip.Invoke(null);
                }
                else
                {
                    AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                    callback_audio_clip.Invoke(myClip);
                }
            }
        }
    }
}
