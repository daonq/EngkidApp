using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewSizeController : MonoBehaviour
{
    public GameObject m_ContentElementPrefab;
    public GameObject m_ContentHolder;

    //internal
    Vector2 contentElementSize;

    private void Start()
    {
        contentElementSize = m_ContentElementPrefab.GetComponent<RectTransform>().sizeDelta;
        m_ContentHolder.GetComponent<HorizontalLayoutGroup>().padding.left = -(int)(contentElementSize.x * 0.5f) + Screen.width / 2;
        m_ContentHolder.GetComponent<HorizontalLayoutGroup>().padding.right = -(int)(contentElementSize.x * 0.5f) + Screen.width / 2;
    }
}
