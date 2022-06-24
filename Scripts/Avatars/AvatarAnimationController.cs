using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAnimationController : MonoBehaviour
{
    public LipsSync m_LipSynch;

    //internals
    Animator animatorController;

    private void Start()
    {
        if (animatorController == null)
            animatorController = this.GetComponent<Animator>();
    }

    public void OnStartTalking()
    {
        if (m_LipSynch != null)
        {
            m_LipSynch.StartTalking();
        }    
    }    

    public void OnJump()
    {
        if (animatorController == null)
            animatorController = this.GetComponent<Animator>();

        animatorController.SetTrigger("Jump");
    }

    public void OnFly()
    {
        if (animatorController == null)
            animatorController = this.GetComponent<Animator>();

        animatorController.SetTrigger("Fly");
    }
}
