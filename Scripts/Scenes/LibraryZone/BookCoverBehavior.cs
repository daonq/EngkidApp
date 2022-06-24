using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BookCoverBehavior : MonoBehaviour
{
    [Header("Book thumbnail")]
    public Text bookLabel;
    public Image bookthumbnail;
    public int index = -1;

    [HideInInspector] public LibrarySceneBehavior librarySceneBehavior = null;

    //internals
    Texture2D tex = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if (tex != null)
            Destroy(tex);
    }

    public void OnBookClicked()
    {
        if (librarySceneBehavior != null)
            librarySceneBehavior.ShowStoryPopUp(index);
    }

    public IEnumerator GetCoverImage(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            bookthumbnail.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        yield return null;
        if (librarySceneBehavior != null)
            librarySceneBehavior.LoadingPing();
    }
}
