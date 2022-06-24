using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalCurtain : MonoBehaviour
{
    #region Singleton
    private static GlobalCurtain _Instance;
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public static GlobalCurtain GetInstance()
    {
        if (_Instance == null)
            Debug.LogError("Error: GlobalCurtain instance is null.");

        return _Instance;
    }
    #endregion

    public Spine.Unity.SkeletonGraphic m_Curtain;

    private void Start()
    {
        //TODO: adjust canvas based on screen ratio
        //if (Camera.main.aspect >= 1.7)
        //{
        //    m_Curtain.transform.parent.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
        //    m_Curtain.GetComponent<RectTransform>().SetBottom(-50.0f);
        //}
        ////else if (Camera.main.aspect >= 1.4)
        ////{
        ////    m_MainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5f;
        ////}
        //else
        //{
        //    m_Curtain.transform.parent.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
        //    m_Curtain.GetComponent<RectTransform>().SetBottom(50.0f);
        //}

        m_Curtain.gameObject.SetActive(false);
        m_Curtain.raycastTarget = false;
    }

    private void Update()
    {
        //TODO: adjust canvas based on screen ratio
        //if (Camera.main.aspect >= 1.7)
        //{
        //    m_Curtain.transform.parent.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
        //    m_Curtain.GetComponent<RectTransform>().SetBottom(-50.0f);
        //}
        ////else if (Camera.main.aspect >= 1.4)
        ////{
        ////    m_MainCanvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5f;
        ////}
        //else
        //{
        //    m_Curtain.transform.parent.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
        //    m_Curtain.GetComponent<RectTransform>().SetBottom(50.0f);
        //}
    }

    public void TriggerCurtain()
    {
        m_Curtain.gameObject.SetActive(true);
        //m_Curtain.Skeleton.SetBonesToSetupPose();
        m_Curtain.AnimationState.SetAnimation(0, "idle_close", loop: true);
        //m_Curtain.Skeleton.SetBonesToSetupPose();
        m_Curtain.raycastTarget = true;
    }

    Coroutine openCor = null;
    public void OpenCurtain()
    {
        //if (openCor != null)
        //    return;

        //openCor = StartCoroutine(OpenCurtainSequence());

        m_Curtain.AnimationState.SetAnimation(0, "idle_open", loop: true);
        m_Curtain.raycastTarget = false;
    }

    IEnumerator OpenCurtainSequence()
    {
        m_Curtain.AnimationState.SetAnimation(0, "out", loop: false);
        yield return new WaitForSeconds(2.0f);
        //m_Curtain.Skeleton.SetBonesToSetupPose();
        m_Curtain.AnimationState.SetAnimation(0, "idle_open", loop: true);
        //m_Curtain.Skeleton.SetBonesToSetupPose();
        openCor = null;
    }

    public void CloseCurtain()
    {
        //m_Curtain.Skeleton.SetBonesToSetupPose();
        m_Curtain.AnimationState.SetAnimation(0, "idle_close", loop: true);
        //m_Curtain.Skeleton.SetBonesToSetupPose();
        m_Curtain.raycastTarget = true;
    }

    public void HideCurtain()
    {
        m_Curtain.raycastTarget = false;
        m_Curtain.gameObject.SetActive(false);
    }
}


public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}