using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSequencing : MonoBehaviour
{
    [Header("Display settings")]
    public Image m_Image;
    public float m_FrameTime = 0.016f;
    public bool m_Looping = false;

    [Header("Images to sequencing")]
    public List<Sprite> m_ImagesList = new List<Sprite>();

    // Start is called before the first frame update
    void Start()
    {
        if (m_Looping == false)
        {
            StartCoroutine(NonLoopingSequence());
        }
    }

    IEnumerator NonLoopingSequence()
    {
        for (int i = 0; i < m_ImagesList.Count; i++)
        {
            yield return new WaitForSeconds(m_FrameTime);
            m_Image.sprite = m_ImagesList[i];
        }
    }

    IEnumerator LoopingSequence()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            for (int i = 0; i < m_ImagesList.Count; i++)
            {
                yield return new WaitForSeconds(m_FrameTime);
                m_Image.sprite = m_ImagesList[i];
            }
        }
    }
}
