using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanonBehavior : MonoBehaviour
{
    [Header("Activity Manager")]
    public ActivityWordShooter activityWordShooterManager;

    [Header("Bullet Labels")]
    public List<Text> m_BulletLabelTextsList = new List<Text>();

    [Header("Bullet capacity display settings")]
    public int m_BulletCapacity = 5;
    public List<GameObject> m_BulletLogosList = new List<GameObject>();
    public GameObject m_DisplayMask;
    public float m_MaskTopPos = 1.5f;
    public float m_MaskBotPos = 0.25f;

    [Header("Canon Settings")]
    [Range(0.05f, 0.5f)] public float m_CanonCoolDown = 0.5f;
    public Button m_FireButton;

    [Header("Shot bullets settings")]
    public Transform m_FlyingBulletsHolder;
    public float m_FlyingDistance = 20.0f;
    [Range(1.0f, 100.0f)] public float m_ShotBulletSpeed = 1.0f;
    [Range(0.0f, 10.0f)] public float m_FlightCurveControl = 5.0f;
    public List<GameObject> m_ShotBulletsList = new List<GameObject>();

    //internal
    float maskStep = 0.25f;
    int remainingBullet = 5;
    bool isReadyToShoot = true;
    GameObject currentBullet = null;
    int currentIndex = -1;
    List<GameObject> m_BulletSelectorsList = new List<GameObject>();

    private void Start()
    {
        foreach (GameObject logo in m_BulletLogosList)
        {
            logo.SetActive(false);
        }

        m_DisplayMask.transform.localPosition = new Vector3(m_DisplayMask.transform.localPosition.x, m_MaskBotPos, 0.0f);
        m_FireButton.gameObject.SetActive(false);

        maskStep = (m_MaskTopPos - m_MaskBotPos) / m_BulletCapacity;
        remainingBullet = m_BulletCapacity;

        m_BulletSelectorsList = activityWordShooterManager.m_BulletsSelectButtonsList;

        //adjust label position based on screen size
        //and change label text based on the input data
        foreach (Text label in m_BulletLabelTextsList)
        {
            Vector3 world_pos = activityWordShooterManager.m_BulletsSelectButtonsList[m_BulletLabelTextsList.IndexOf(label)].transform.position;
            Vector3 screen_pos = RectTransformUtility.WorldToScreenPoint(Camera.main, world_pos);
            label.transform.position = screen_pos;
            label.text = activityWordShooterManager.m_BulletValuesList[m_BulletLabelTextsList.IndexOf(label)];
        }
    }

    public void OnChamberingBullet(int index)
    {
        remainingBullet = m_BulletCapacity;
        LeanTween.moveLocalY(m_DisplayMask, m_MaskTopPos, m_CanonCoolDown * 0.75f);

        foreach (GameObject logo in m_BulletLogosList)
        {
            logo.SetActive(false);
        }
        m_BulletLogosList[index].SetActive(true);

        m_FireButton.gameObject.SetActive(true);
        m_FireButton.transform.GetChild(0).GetComponent<Text>().text = activityWordShooterManager.m_BulletValuesList[index];
        m_FireButton.gameObject.GetComponent<Image>().sprite = m_BulletLogosList[index].GetComponent<SpriteRenderer>().sprite;
        currentIndex = index;
    }

    public void OnShootingBullet()
    {
        if (isReadyToShoot || remainingBullet <= 0)
        {
            isReadyToShoot = false;
            StartCoroutine(ProcessCanonCooldown());
            remainingBullet--;
            LeanTween.moveLocalY(m_DisplayMask, m_MaskBotPos + maskStep * remainingBullet, m_CanonCoolDown * 0.75f);

            //TODO: instantiate bullets
            currentBullet = Instantiate(m_ShotBulletsList[currentIndex], m_FlyingBulletsHolder);
            Vector3 end_point = m_FlyingBulletsHolder.position + new Vector3(m_FlyingDistance, 0.0f, 0.0f);
            Vector3 control_point = ((m_FlyingBulletsHolder.position + end_point) * 0.5f) + Vector3.up * m_FlightCurveControl;
            currentBullet.GetComponent<ShotBulletBehavior>().ShotBulletInit(m_FlyingBulletsHolder.position, end_point, control_point, m_FlyingDistance / m_ShotBulletSpeed, currentIndex);
        }

        if (remainingBullet <= 0)
        {
            m_FireButton.gameObject.SetActive(false);
            foreach (GameObject logo in m_BulletLogosList)
            {
                logo.SetActive(false);
            }
        }
    }

    IEnumerator ProcessCanonCooldown()
    {
        yield return new WaitForSeconds(m_CanonCoolDown);
        isReadyToShoot = true;
    }
}
