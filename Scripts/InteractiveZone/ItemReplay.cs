using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemReplay : MonoBehaviour
{

    public Image BubbleChat;
    public Text TextChat;
    public Sprite normal;
    public Sprite hightlight;

    SentenceAI sentence;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Display()
    {
        TextChat.text = sentence.result.textReference;
    }

    public void SetSentence(SentenceAI item)
    {
        sentence = item;
        Display();
    }

    public void SetHighlight(bool flag)
    {
        if (flag)
        {
            BubbleChat.sprite = hightlight;
        }
        else
        {
            BubbleChat.sprite = normal;
        }
    }
}
