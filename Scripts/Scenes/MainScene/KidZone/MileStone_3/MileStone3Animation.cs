using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MileStone3Animation : MonoBehaviour
{
    [Header("Tree leaves")]
    public GameObject m_TreeLeaveA;
    public GameObject m_TreeLeaveB;
    public GameObject m_TreeLeaveC;

    [Header("Animation settings")]
    public Vector3 m_AnimBound = new Vector3();

    private void Start()
    {
        Vector3 treeAMovePos = m_TreeLeaveA.transform.localPosition + m_AnimBound;
        LeanTween.moveLocal(m_TreeLeaveA, treeAMovePos, 1.5f).setLoopPingPong();

        Vector3 treeBMovePos = m_TreeLeaveB.transform.localPosition - m_AnimBound;
        LeanTween.moveLocal(m_TreeLeaveB, treeBMovePos, 1.75f).setLoopPingPong();

        Vector3 treeCMovePos = m_TreeLeaveC.transform.localPosition + m_AnimBound;
        LeanTween.moveLocal(m_TreeLeaveC, treeCMovePos, 1.0f).setLoopPingPong();
    }
}
