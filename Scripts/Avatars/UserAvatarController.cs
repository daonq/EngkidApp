using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAvatarController : MonoBehaviour
{
    [Header("Animations and Effects")]
    public GameObject m_AvatarObject;
    public GameObject m_Wings;

    [Header("Animation test")]
    public bool m_Jump = false;
    public bool m_Fly = false;

    //internals
    Animator avatarAnimator;
    Animator wingAnimator;

    private void Start()
    {
        avatarAnimator = m_AvatarObject.GetComponent<Animator>();

        if (m_Wings)
            wingAnimator = m_Wings.GetComponent<Animator>();
    }

    private void Update()
    {
        if (m_Jump == true)
        {
            avatarAnimator.SetTrigger("Jump");

            if (wingAnimator)
                wingAnimator.SetTrigger("Jump");

            m_Jump = false;
        }

        if (m_Fly == true)
        {
            avatarAnimator.SetTrigger("Fly");

            if (wingAnimator)
                wingAnimator.SetTrigger("Fly");

            m_Fly = false;
        }
    }

    public void OnJump()
    {
        avatarAnimator.SetTrigger("Jump");

        if (wingAnimator)
            wingAnimator.SetTrigger("Jump");
    }

    public void OnFly()
    {
        avatarAnimator.SetTrigger("Fly");

        if (wingAnimator)
            wingAnimator.SetTrigger("Fly");
    }
}
