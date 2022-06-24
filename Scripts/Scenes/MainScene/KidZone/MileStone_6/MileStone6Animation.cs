using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MileStone6Animation : MonoBehaviour
{
    [Header("Ices animation")]
    public GameObject m_IceA;
    public GameObject m_IceB;
    public Transform m_IceAMovePos;
    public Transform m_IceBMovePos;

    private void Start()
    {
        LeanTween.moveLocal(m_IceA, m_IceAMovePos.localPosition, 2.5f).setLoopPingPong();
        LeanTween.moveLocal(m_IceB, m_IceBMovePos.localPosition, 2.5f).setLoopPingPong();
    }
}
