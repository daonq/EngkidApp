using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ItemDuel : MonoBehaviour
{

    [SerializeField] private DuelInfo dataDuel;
    public Image thumbnailHolder;

    Texture2D tex;

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

    ListDuel listDuel = null;
    public void SetDuel(DuelInfo data, ListDuel newListDuel)
    {
        dataDuel = data;
        listDuel = newListDuel;
    }

    public DuelInfo GetDuel()
    {
        return dataDuel;
    }

    public void SetupDuel()
    {
        StartCoroutine(GetImage(dataDuel.avatar, thumbnailHolder));
    }

    public void ShowDetail()
    {
        //Debug.Log("nobita show chi tiet");
    }

    IEnumerator GetImage(string url, Image image)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            listDuel.ReadyPing();
        }
        else
        {
            tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            image.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            listDuel.ReadyPing();
        }
    }

    public void SelectedDuel()
    {

    }
}
