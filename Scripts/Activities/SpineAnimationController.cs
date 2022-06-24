using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SpineAnimationController : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public string m_DefaultAnimationName = "animation";

    public void TriggerDefaultANimation()
    {
        this.GetComponent<MeshRenderer>().enabled = true;
        skeletonAnimation.state.ClearTracks();
        skeletonAnimation.skeleton.SetToSetupPose();
        skeletonAnimation.timeScale = 1.0f;
        skeletonAnimation.AnimationName = m_DefaultAnimationName;
        skeletonAnimation.state.SetAnimation(0, m_DefaultAnimationName, false);
        //StartCoroutine(DelayedHideAnimation(skeletonAnimation.skeleton.Data.FindAnimation(m_DefaultAnimationName).Duration));
    }

    IEnumerator DelayedHideAnimation(float dur)
    {
        yield return new WaitForSeconds(dur);
        this.GetComponent<MeshRenderer>().enabled = false;
    }    

    private void OnDisable()
    {
        StopAllCoroutines();
        skeletonAnimation.state.ClearTracks();
        skeletonAnimation.skeleton.SetToSetupPose();
    }   
}
