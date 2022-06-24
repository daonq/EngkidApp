using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectKidAvatarScrollViewHelper : MonoBehaviour
{
    public CreateKidBehavior m_CreateKidBehavior;
    public GameObject m_ContentHolder;

    //internal
    Vector2 contentElementSize;

    private void Start()
    {
        contentElementSize = m_CreateKidBehavior.m_AvatarPrefab.GetComponent<RectTransform>().sizeDelta;
        m_ContentHolder.GetComponent<HorizontalLayoutGroup>().padding.left = (Screen.width / (Screen.width / (int)Mathf.Abs(contentElementSize.x))) + (int)Mathf.Abs(contentElementSize.x);
        m_ContentHolder.GetComponent<HorizontalLayoutGroup>().padding.right = (Screen.width / (Screen.width / (int)Mathf.Abs(contentElementSize.x))) + (int)Mathf.Abs(contentElementSize.x);
    }

    private void Update()
    {
        contentElementSize = m_CreateKidBehavior.m_AvatarPrefab.GetComponent<RectTransform>().sizeDelta;
        m_ContentHolder.GetComponent<HorizontalLayoutGroup>().padding.left = (Screen.width / (Screen.width / (int)Mathf.Abs(contentElementSize.x))) + (int)Mathf.Abs(contentElementSize.x);
        m_ContentHolder.GetComponent<HorizontalLayoutGroup>().padding.right = (Screen.width / (Screen.width / (int)Mathf.Abs(contentElementSize.x))) + (int)Mathf.Abs(contentElementSize.x);
    }
}
