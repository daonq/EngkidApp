using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashSceneBehavior : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource m_SplashScreenAudioSource;
    public float m_SplashDefaultVolume = 1.0f;

    [Header("Mask Animations")]
    public GameObject m_SplashScreenMask;
    public Transform m_SplashScreenMaskEndPos;

    [Header("Text Animation")]
    public List<GameObject> m_LettersList = new List<GameObject>();
    [Range(0.0f, 360.0f)]public float m_TextRotationValue = 12.5f;

    [Header("Small Icon animation")]
    public GameObject m_Icon;
    public Transform m_IconEndPos;
    public Transform m_IconMidControl;

    [Header("Avatars Animation")]
    public GameObject m_BoiAvatar;
    public Sprite m_BoiNormal;
    public Sprite m_BoiWink;
    public GameObject m_GirlAvatar;
    public Sprite m_GirlNormal;
    public Sprite m_GrilWink;

    [Header("Star ANimation")]
    public GameObject m_Star;
    public List<Transform> m_StarSplinePointsList = new List<Transform>();

    private void Start()
    {
        //audio
        m_SplashScreenAudioSource.volume = 0.0f;
        m_SplashScreenAudioSource.Play();
        FadeAudio(m_SplashDefaultVolume, 2.0f);

        //animation
        //mask
        LeanTween.move(m_SplashScreenMask, m_SplashScreenMaskEndPos.position, 3.0f).setDelay(1.0f);
        //letters
        foreach (GameObject letter in m_LettersList)
        {
            int rotation_dir = Random.Range(0, 2);
            if (rotation_dir == 0)
                rotation_dir = -1;
            float val = letter.transform.rotation.z + (m_TextRotationValue * rotation_dir);
            LeanTween.rotateZ(letter, val, 0.25f).setDelay(1.0f).setLoopPingPong();
        }
        //icon
        LTBezierPath ltPath = new LTBezierPath(new Vector3[] { m_Icon.transform.localPosition, 
                                                               m_IconMidControl.localPosition,
                                                               m_IconMidControl.localPosition, 
                                                               m_IconEndPos.localPosition });
        LeanTween.moveLocal(m_Icon, ltPath, 1.0f).setDelay(1.25f).setOnComplete(() => {
            m_Icon.SetActive(false);
        });
        //avatars
        m_BoiAvatar.transform.localScale = new Vector3();
        LeanTween.scale(m_BoiAvatar, Vector3.one, 0.5f).setDelay(2.25f);
        m_GirlAvatar.transform.localScale = new Vector3();
        LeanTween.scale(m_GirlAvatar, Vector3.one, 0.5f).setDelay(3.0f);
        StartCoroutine(SpriteSwapper(m_BoiAvatar.GetComponent<Image>(), m_BoiNormal, m_BoiWink));
        StartCoroutine(SpriteSwapper(m_GirlAvatar.GetComponent<Image>(), m_GirlNormal, m_GrilWink));
        //star
        LTSpline ltSpline = new LTSpline(new Vector3[] { m_StarSplinePointsList[0].transform.position,
                                                         m_StarSplinePointsList[0].transform.position,
                                                         m_StarSplinePointsList[1].transform.position,
                                                         m_StarSplinePointsList[2].transform.position,
                                                         m_StarSplinePointsList[3].transform.position,
                                                         m_StarSplinePointsList[4].transform.position,
                                                         m_Star.transform.position,
                                                         m_Star.transform.position });
        LeanTween.moveSpline(m_Star, ltSpline, 1.0f).setDelay(3.0f);
    }

    IEnumerator SpriteSwapper(Image renderer, Sprite normal_sprite, Sprite changed_sprite)
    {
        bool is_normal = true;
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (is_normal)
            {
                is_normal = false;
                renderer.sprite = changed_sprite;
            }
            else
            {
                is_normal = true;
                renderer.sprite = normal_sprite;
            }
            renderer.SetNativeSize();
        }
    }

    private void OnDisable()
    {
        //LeanTween.cancelAll();
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        //LeanTween.cancelAll();
        StopAllCoroutines();
    }

    //fade audio volume louder/quieter
    public void FadeAudio(float end_volume, float duration)
    {
        float start_volume = m_SplashScreenAudioSource.volume;
        LeanTween.value(m_SplashScreenAudioSource.gameObject, start_volume, end_volume, duration).setOnUpdate((float val) =>
        {
            m_SplashScreenAudioSource.volume = val;
        });
    }
}
