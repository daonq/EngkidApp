using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SpeakingLesson : MonoBehaviour
{
    public bool isFreemium = false;
    public ListLesson listLesson;
    private UsableDataInfo dataLesson;
    public Image thumbnailHolder;
    public Text title;
    public TextMeshProUGUI secondTitle;
    public GameObject badge;

    Texture2D tex;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetLesson(UsableDataInfo data)
    {
        dataLesson = data;
    }

    public UsableDataInfo GetLesson()
    {
        return dataLesson;
    }

    public IEnumerator SetupLesson()
    {
        yield return StartCoroutine(GetImage(dataLesson.index_thumbnail.value, thumbnailHolder));
        yield return null;

        title.text = dataLesson.title;
        secondTitle.text = dataLesson.title;
        if (dataLesson.isLearn)
        {
            badge.SetActive(true);
        }
        else
        {
            badge.SetActive(false);
        }
    }

    public void ShowDetail()
    {
        Debug.Log("nobita show chi tiet");
    }

    IEnumerator GetImage(string url, Image image)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            listLesson.LoadingPing();
        }
        else
        {
            tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            image.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            listLesson.LoadingPing();
        }
    }

    private void OnDestroy()
    {
        if (tex != null)
            Destroy(tex);
    }
}
