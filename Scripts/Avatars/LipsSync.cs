using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LipsSync : MonoBehaviour
{
    public SpriteRenderer m_MouthRenderer;
    public Sprite m_MouthDefaultSprite;

    public float m_TalkingSequenceLength = 5.0f;
    public List<Sprite> m_MouthSpritesList = new List<Sprite>();

    public bool m_TalkingOnStart = true;

    //internals
    Coroutine talkingCor = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_TalkingOnStart == true)
            talkingCor = StartCoroutine(TalkingSequence());
    }

    IEnumerator TalkingSequence()
    {
        foreach (Sprite mouth_sprite in m_MouthSpritesList)
        {
            m_MouthRenderer.sprite = mouth_sprite;
            yield return new WaitForSeconds(m_TalkingSequenceLength / m_MouthSpritesList.Count);
        }

        m_MouthRenderer.sprite = m_MouthDefaultSprite;
    }

    public void StartTalking()
    {
        if (talkingCor != null)
            StopCoroutine(talkingCor);

        talkingCor = StartCoroutine(TalkingSequence());
    }    
}
