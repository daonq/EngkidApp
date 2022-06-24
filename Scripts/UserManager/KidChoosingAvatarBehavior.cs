using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class KidChoosingAvatarBehavior : MonoBehaviour
{
    public Image m_AvatarImage;
    public RawImage m_AvatarRawImage;
    public string m_KidName = "";
    public string m_KidAccountID = "";

    Texture2D tex = null;

    public void OnSetDisplayData(string sprite_uri, string name, string id)
    {
        StartCoroutine(GetImage(sprite_uri));
        m_KidName = name;
        m_KidAccountID = id;

        Debug.Log("Hoc sinh: " + m_KidAccountID + " vs " + m_KidName + " vs " + sprite_uri);
    }

    IEnumerator GetImage(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Request URL: " + url + "\nError: " + www.error);
        }
        else
        {
            DestroyImmediate(tex);
            tex = (DownloadHandlerTexture.GetContent(www)) as Texture2D;
            m_AvatarRawImage.texture = tex;
            //DestroyImmediate(((DownloadHandlerTexture)www.downloadHandler).texture);
        }


        www.Dispose();
        www = null;
        Resources.UnloadUnusedAssets();
    }

    private void OnDestroy()
    {
        if (tex != null)
            DestroyImmediate(tex);

        StopAllCoroutines();
    }
}