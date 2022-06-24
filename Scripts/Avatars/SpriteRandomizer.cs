using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteRandomizer : MonoBehaviour
{
    public Image m_Image;
    public List<Sprite> m_SpritesList = new List<Sprite>();
    public int m_CooldownFrames = 30;

    int ogCDFrames = 0;

    private void Start()
    {
        ogCDFrames = m_CooldownFrames;
    }

    private void Update()
    {
        m_CooldownFrames--;

        if (m_CooldownFrames <= 0)
        {
            m_CooldownFrames = ogCDFrames;
            m_Image.sprite = m_SpritesList[Random.Range(0, m_SpritesList.Count)];
        }    
    }
}
