using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivityDisplayManager : MonoBehaviour
{
    public string id = "";
    public Text m_Label;
    public RawImage m_Image;
    public GameObject m_LockIndicator;
    public GameObject m_StarsHolder;
    public List<GameObject> m_StarsList = new List<GameObject>();

    [Header("Debug")]
    public int m_Score = 0;
}
