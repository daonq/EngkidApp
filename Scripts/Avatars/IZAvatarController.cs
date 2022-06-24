using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IZAvatarController : MonoBehaviour
{
    public Animator m_AvatarAnimator;
    public string m_StartAnimationName = "";

    // Start is called before the first frame update
    void Start()
    {
        m_AvatarAnimator.Play(m_StartAnimationName);
    }

    public void SwapAvatar(Animator new_animator)
    {
        m_AvatarAnimator.gameObject.SetActive(false);
        m_AvatarAnimator = new_animator;
        m_AvatarAnimator.gameObject.SetActive(true);
        m_AvatarAnimator.Play(m_StartAnimationName);
    }    
}
