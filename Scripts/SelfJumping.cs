using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfJumping : MonoBehaviour
{
    public float m_JumpHeight = 10.0f;
    public float m_JumpDelay = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 new_pos = this.transform.localPosition + new Vector3(0.0f, m_JumpHeight, 0.0f);
        LeanTween.moveLocal(this.gameObject, new_pos, 0.25f).setDelay(m_JumpDelay).setLoopPingPong();
    }
}
