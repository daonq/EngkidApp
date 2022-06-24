using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivityWordShooter : BaseActivityBehavior
{
    [Header("Bullets selector")]
    public List<GameObject> m_BulletsSelectButtonsList = new List<GameObject>();
    public List<GameObject> m_DragableBulletsList = new List<GameObject>();
    public List<string> m_BulletValuesList = new List<string>();
    public Transform m_DragBulletHolder;

    [Header("Monster Settings")]
    public List<string> m_MonsterValuesList = new List<string>();

    [Header("Result Board")]
    public GameObject m_ResultBoard;
    public List<GameObject> m_StarsList = new List<GameObject>();
    public Text m_XPText;

    [Header("Audio Players")]
    public AudioSource m_AudioSource;
    public AudioClip m_CanonTapAudioClip;
    public AudioClip m_ChamberingAudioClip;
    public List<AudioClip> m_WordAudioClipsList = new List<AudioClip>();

    //internal
    GameObject copyOfThis;
    bool isMouseDown = false;
    GameObject currentDragBullet = null;

    public override void StartActivity()
    {
        base.StartActivity();
    }

    public override void EndActivity()
    {
        base.EndActivity();
    }

    private void Start()
    {
        copyOfThis = this.gameObject;

        if (m_ResultBoard != null)
            m_ResultBoard.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) == true) //on mouse left click or touch
        {
            if (isMouseDown == false) //just clicked
            {
                isMouseDown = true;

                //get mouse click pos
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                //check if clicked on bullet buttons
                if (hit.collider != null)
                {
                    if (m_BulletsSelectButtonsList.Contains(hit.collider.gameObject))
                    {
                        //TODO: instantiate a bullet depend on which button is triggered
                        if (currentDragBullet != null)
                            Destroy(currentDragBullet);
                        int bulletIndex = m_BulletsSelectButtonsList.IndexOf(hit.collider.gameObject);
                        currentDragBullet = Instantiate(m_DragableBulletsList[bulletIndex], m_DragBulletHolder);
                        currentDragBullet.GetComponent<DragBulletBehavior>().OnInit(this, bulletIndex);
                    }
                    else if (hit.collider.gameObject.GetComponent<CanonBehavior>() != null)
                    {
                        PlaySound(m_CanonTapAudioClip);
                    }
                    else if (hit.collider.gameObject.GetComponent<MonsterBehavior>() != null)
                    {
                        hit.collider.gameObject.GetComponent<MonsterBehavior>().OnMonsterTapped();
                    }
                }
            }
            else //holding -> draging
            {
                //move the bullet with mouse input
                if (currentDragBullet != null)
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    currentDragBullet.transform.position = new Vector3(mousePos.x, mousePos.y, 0.0f);
                }
            }
        }
        else //not clicking or touching or holding anymore -> release the draging bullet
        {
            if (isMouseDown == true)
            {
                isMouseDown = false;

                if (currentDragBullet != null)
                    Destroy(currentDragBullet);
            }
        }
    }

    public void OnChamberingBullet(int bulletIndex)
    {
        if (currentDragBullet != null)
            Destroy(currentDragBullet);

        PlaySound(m_ChamberingAudioClip);
    }

    private void PlaySound(AudioClip audio)
    {
        m_AudioSource.Stop();
        m_AudioSource.clip = audio;
        m_AudioSource.Play();
    }

    public void OnShowResult(int miss_count)
    {
        m_ResultBoard.SetActive(true);
        m_XPText.text = "";
        foreach (GameObject star in m_StarsList)
            star.SetActive(false);

        //TODO: animation...may be?
        Vector3 og_result_scale = m_ResultBoard.transform.localScale;
        m_ResultBoard.transform.localScale *= 0.0f;
        LeanTween.scale(m_ResultBoard, og_result_scale, 0.5f).setOnComplete(() => {
            //TODO: change displayed result
            int starsGotten = 3 - (miss_count / 5);
            if (starsGotten <= 0)
                starsGotten = 0;
            for (int i = 0; i < starsGotten; i++)
            {
                m_StarsList[i].SetActive(true);
                Vector3 og_scale = m_StarsList[i].transform.localScale;
                m_StarsList[i].transform.localScale *= 0.0f;
                LeanTween.scale(m_StarsList[i], og_scale, 0.25f).setDelay(0.5f * i);
            }

            LeanTween.value(this.gameObject, 0, 15, 1.5f).setOnUpdate((float val) =>
            {
                m_XPText.text = Mathf.RoundToInt(val).ToString();
            });
        });
    }

    public void OnQuitClicked()
    {
        #if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    //---------------------------------------------------------
    //TODO: this is only for the demo, 
    //remove this and don't ever do this in real app
    //---------------------------------------------------------
    public void OnPlayAgainClicked()
    {
        Instantiate(copyOfThis, null);
        Destroy(this.gameObject);
    }
}
