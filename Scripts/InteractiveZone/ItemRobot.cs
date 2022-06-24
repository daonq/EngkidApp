using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ItemRobot : MonoBehaviour
{

    public Text sentence;
    public Image chatBubble;
    public Sprite bubbleRobot;
    public Sprite bubblePlayer;
    public Image avatar;
    public Sprite robotAvatar;

    public GameObject typingMode;
    public GameObject basicMode;

    int mode;
    RectTransform parentRectTransform;
    RectTransform basicModeRectTransform;

    // Start is called before the first frame update
    void Start()
    {
        parentRectTransform = this.transform.parent.parent.parent.GetChild(0).GetComponent<RectTransform>();
        basicModeRectTransform = basicMode.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 chatBubbleDinamicSize = new Vector2(Mathf.Abs(parentRectTransform.rect.x - 150.0f), basicModeRectTransform.sizeDelta.y);
        basicModeRectTransform.sizeDelta = chatBubbleDinamicSize;
    }

    public void Setup(string value, int type)
    {
        sentence.text = value;
        if (type == 1)
        {
            chatBubble.sprite = bubblePlayer;
            StartCoroutine(GetAvatarImage(UserDataManagerBehavior.GetInstance().currentSelectedKidAvatarURL));
        }
        else
        {
            chatBubble.sprite = bubbleRobot;
            avatar.sprite = robotAvatar;
        }
    }

    IEnumerator GetAvatarImage(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            avatar.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }

    public void DisplayMode(int mode)
    {
        this.mode = mode;
        if (mode == 1)
        {
            typingMode.SetActive(true);
            basicMode.SetActive(false);
        }
        else
        {
            typingMode.SetActive(false);
            basicMode.SetActive(true);
        }
    }
}
