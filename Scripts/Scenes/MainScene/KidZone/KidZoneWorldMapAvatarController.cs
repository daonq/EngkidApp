using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidZoneWorldMapAvatarController : MonoBehaviour
{
    [Header("Main Stage settings")]
    public KidZoneWorldMapBehavior worldMapBehavior;

    [Header("Avatar settings")]
    public Spine.Unity.SkeletonAnimation m_AvatarObject;

    [Header("Avatar SFX")]
    public AudioSource m_AudioSource;
    public AudioClip m_ShortFly;
    public AudioClip m_LongFly;

    //internals
    [HideInInspector] public int tweenID;
    //Vector3 avatarOGScale;
    bool idleCheck = true;

    IEnumerator WaitToChangeSkin()
    {
        //Debug.Log("Waittttttttttttttttttttt");
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return new WaitUntil(() => !string.IsNullOrEmpty(UserDataManagerBehavior.GetInstance().currentSkinName));
        Debug.Log("#2_________currentSkinName:" + UserDataManagerBehavior.GetInstance().currentSkinName);
        SetSkin(UserDataManagerBehavior.GetInstance().currentSkinName);
    }

    public void SetSkin(string skin_name)
    {
        //Debug.Log("Setting user costume: " + skin_name);
        skin_name = skin_name.Replace(" ", string.Empty);
        m_AvatarObject.skeleton.SetSlotsToSetupPose();
        m_AvatarObject.skeleton.SetSkin(skin_name);
        m_AvatarObject.skeleton.SetSlotsToSetupPose();
    }

    private void Start()
    {
        //if (!string.IsNullOrEmpty(UserDataManagerBehavior.GetInstance().currentSkinName))
        //{
        //    Debug.Log("#1_________currentSkinName:" + UserDataManagerBehavior.GetInstance().currentSkinName);
        //    SetSkin(UserDataManagerBehavior.GetInstance().currentSkinName);
        //}
        //else
        //{
        //    StartCoroutine(WaitToChangeSkin());
        //}

        StartCoroutine(WaitToChangeSkin());

        StartCoroutine(DelayedFirstJump());
    }

    private void Update()
    {
        if(!string.IsNullOrEmpty(UserDataManagerBehavior.GetInstance().currentSkinName))
            SetSkin(UserDataManagerBehavior.GetInstance().currentSkinName);
    }

    Coroutine idleCor = null;
    IEnumerator IdleCheckSequence()
    {
        while (idleCheck == true)
        {
            //Debug.Log("beng");
            yield return new WaitForSeconds(10.0f);
            m_AvatarObject.AnimationState.SetAnimation(0, "yahoo", true);
            yield return new WaitForSeconds(1.0f);
            m_AvatarObject.AnimationState.SetAnimation(0, "idle", true);
        }
    }

    IEnumerator DelayedFirstJump()
    {
        while (worldMapBehavior.is_ready == false)
        {
            yield return null;
        }
        yield return null;
        if (PlayerPrefs.HasKey("PLAYER_POS_X"))
            JumpToNextUnit(new Vector3(PlayerPrefs.GetFloat("PLAYER_POS_X"),
                                                                PlayerPrefs.GetFloat("PLAYER_POS_Y"),
                                                                PlayerPrefs.GetFloat("PLAYER_POS_Z")) + Vector3.up * 0.1f);
        yield return new WaitForSeconds(2.0f);
        idleCheck = true;
        idleCor = StartCoroutine(IdleCheckSequence());
    }

    public bool JumpToNextUnit(Vector3 new_unit_pos)
    {
        if (LeanTween.isTweening(tweenID) == true)
            return false;

        idleCheck = false;
        if (idleCor != null)
            StopCoroutine(idleCor);

        m_AudioSource.Stop();
        m_AudioSource.clip = m_ShortFly;
        m_AudioSource.Play();

        //change facing
        //if (new_unit_pos.x < m_AvatarObject.transform.position.x)
        //    m_AvatarObject.transform.localScale = new Vector3(-1.0f * avatarOGScale.x, avatarOGScale.y, avatarOGScale.z);

        //play animation
        m_AvatarObject.AnimationState.SetAnimation(0, "jump_demo", false);

        //TODO: make slight curve
        Vector3 mid_point = (m_AvatarObject.transform.position + new_unit_pos) * 0.5f;
        LTBezierPath ltPath = new LTBezierPath(new Vector3[] { m_AvatarObject.transform.position,
                                                               mid_point + Vector3.up * 1.0f,
                                                               mid_point + Vector3.up * 1.0f,
                                                               new_unit_pos });
        tweenID = LeanTween.move(m_AvatarObject.gameObject, ltPath, 1.567f).setOnComplete(() => {
            //m_AvatarObject.transform.localScale = avatarOGScale;
            m_AvatarObject.AnimationState.SetAnimation(0, "idle", true);
        }).id;
        return true;
    }

    public bool FlyToDistanceUnit(Vector3 new_unit_pos)
    {
        if (LeanTween.isTweening(tweenID) == true)
            return false;

        idleCheck = false;
        if (idleCor != null)
            StopCoroutine(idleCor);

        m_AudioSource.Stop();
        m_AudioSource.clip = m_LongFly;
        m_AudioSource.Play();

        //change facing
        //if (new_unit_pos.x < m_AvatarObject.transform.position.x)
        //    m_AvatarObject.transform.localScale = new Vector3(-1.0f * avatarOGScale.x, avatarOGScale.y, avatarOGScale.z);

        //play animation
        m_AvatarObject.AnimationState.SetAnimation(0, "flying", false);

        //TODO: make high curve
        LTBezierPath ltPath = new LTBezierPath(new Vector3[] { m_AvatarObject.transform.position,
                                                               new_unit_pos + Vector3.up * 10.0f,
                                                               m_AvatarObject.transform.position + Vector3.up * 10.0f,
                                                               new_unit_pos });
        tweenID = LeanTween.move(m_AvatarObject.gameObject, ltPath, 2.0f).setOnComplete(() => {
            //m_AvatarObject.transform.localScale = avatarOGScale;
            m_AvatarObject.AnimationState.SetAnimation(0, "idle", true);
        }).id;
        return true;
    }
}
