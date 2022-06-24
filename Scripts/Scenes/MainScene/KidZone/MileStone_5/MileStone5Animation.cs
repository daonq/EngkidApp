using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MileStone5Animation : MonoBehaviour
{
    [Header("Animation Settings")]
    public GameObject m_JelloA;
    public GameObject m_JelloB;
    public GameObject m_JelloC;

    public Transform m_JelloAMovePos;
    public Transform m_JelloBMovePos;
    public Transform m_JelloCMovePos;

    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveLocal(m_JelloA, m_JelloAMovePos.localPosition, 1.5f).setLoopPingPong();
        LeanTween.moveLocal(m_JelloB, m_JelloBMovePos.localPosition, 2.0f).setLoopPingPong();
        LeanTween.moveLocal(m_JelloC, m_JelloCMovePos.localPosition, 1.75f).setLoopPingPong();
    }
}
