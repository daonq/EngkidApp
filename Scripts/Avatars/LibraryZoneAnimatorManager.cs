using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryZoneAnimatorManager : MonoBehaviour
{
    public Animator m_Animator;
    public string m_LibraryZoneIntroAnimName = "";

    private void Start()
    {
        m_Animator.Play(m_LibraryZoneIntroAnimName);
    }
}
