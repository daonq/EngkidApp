using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MileStone4Animation : MonoBehaviour
{
    [Header("Animation Settings")]
    public GameObject m_Sun;
    public Transform m_SunMovePos;
    public GameObject m_Corona;

    // Start is called before the first frame update
    void Start()
    {
        m_Corona.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        LeanTween.moveLocal(m_Sun, m_SunMovePos.localPosition, 1.5f).setLoopPingPong();
        LeanTween.scale(m_Corona, new Vector3(1.5f, 1.5f, 1.5f), 1.5f).setLoopPingPong();
    }
}
