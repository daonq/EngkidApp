using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBehavior : MonoBehaviour
{
    [Header("Activity Manager")]
    public ActivityWordShooter m_ActivityManager;

    [Header("Monster Settings")]
    [Range(1, 100)] public int m_HealthPointPerOutfit = 5;
    public List<GameObject> m_MonsterOutfitsList = new List<GameObject>();
    public Text m_MonsterNameText;
    public Transform m_MonsterNameAnchor;

    [Header("Monster animation sprites")]
    [Range(0.05f, 0.5f)] public float m_AnimationResetDuration = 0.1f;
    public List<Sprite> m_GetHitSpritesList = new List<Sprite>();
    public List<Sprite> m_BlockSpritesList = new List<Sprite>();
    public List<Sprite> m_DeadSpritesList = new List<Sprite>();

    [Header("Combat Settings")]
    public GameObject m_ShieldPrefab;
    public List<GameObject> m_HitVfxList = new List<GameObject>();
    public List<GameObject> m_SpawnVfxList = new List<GameObject>();
    public List<GameObject> m_DeathVfxList = new List<GameObject>();

    [Header("Audio Settings")]
    public AudioSource m_AudioSource;
    public AudioClip m_DamageSound;
    public AudioClip m_DeadSound;
    public AudioClip m_VictorySound;
    public List<AudioClip> m_WordsSoundsList = new List<AudioClip>();

    //internal
    List<Sprite> defaultSpritesList = new List<Sprite>();
    Coroutine animationChangingCoroutine = null;
    int currentMonsterIndex = 0;
    bool isInvulnerable = false;
    int missCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentMonsterIndex = 0;
        missCounter = 0;
        m_HealthPointPerOutfit = 5;

        defaultSpritesList.Clear();
        foreach (GameObject outfit in m_MonsterOutfitsList)
        {
            defaultSpritesList.Add(outfit.GetComponent<SpriteRenderer>().sprite);
            outfit.SetActive(false);
        }
        m_MonsterOutfitsList[currentMonsterIndex].SetActive(true);

        Vector3 world_pos = m_MonsterNameAnchor.position;
        Vector3 screen_pos = RectTransformUtility.WorldToScreenPoint(Camera.main, world_pos);
        m_MonsterNameText.transform.position = screen_pos;
        m_MonsterNameText.text = m_ActivityManager.m_MonsterValuesList[0];
    }

    private void Update()
    {
        Vector3 world_pos = m_MonsterNameAnchor.position;
        Vector3 screen_pos = RectTransformUtility.WorldToScreenPoint(Camera.main, world_pos);
        m_MonsterNameText.transform.position = screen_pos;
    }

    public bool OnGetHit(int bullet_index)
    {
        if (currentMonsterIndex >= 4)
            return false;

        if (bullet_index == currentMonsterIndex && isInvulnerable == false)
        {
            //TODO: animation and vfx
            GameObject hit_vfx = Instantiate(m_HitVfxList[currentMonsterIndex], this.transform);
            Vector3 og_scale = hit_vfx.transform.localScale;
            hit_vfx.transform.localScale *= 0.5f;
            LeanTween.scale(hit_vfx, og_scale, 0.1f);
            hit_vfx.AddComponent<EngKidAPI.SelfDestroy>().InitSelfDestroy(0.2f);

            //TODO: get hit animation
            m_MonsterOutfitsList[currentMonsterIndex].GetComponent<SpriteRenderer>().sprite = m_GetHitSpritesList[currentMonsterIndex];
            if (animationChangingCoroutine != null)
                StopCoroutine(animationChangingCoroutine);
            animationChangingCoroutine = StartCoroutine(DelayedResetSprite());

            //TODO: deal damage
            m_HealthPointPerOutfit--;
            if (m_HealthPointPerOutfit <= 0)
                ChangeOutfit();

            //audio
            PlaySound(m_DamageSound);
        }
        else
        {
            //TODO: defended animation and vfx
            GameObject shield = Instantiate(m_ShieldPrefab, this.transform);
            Vector3 og_scale = shield.transform.localScale;
            shield.transform.localScale *= 0.5f;
            LeanTween.scale(shield, og_scale, 0.1f);
            shield.AddComponent<EngKidAPI.SelfDestroy>().InitSelfDestroy(0.2f);

            //TODO: block animation
            m_MonsterOutfitsList[currentMonsterIndex].GetComponent<SpriteRenderer>().sprite = m_BlockSpritesList[currentMonsterIndex];
            if (animationChangingCoroutine != null)
                StopCoroutine(animationChangingCoroutine);
            animationChangingCoroutine = StartCoroutine(DelayedResetSprite());

            missCounter++;
        }

        return true;
    }

    private void ChangeOutfit()
    {
        isInvulnerable = true;
        StartCoroutine(DelayedTurnOffInvulnerable());
    }

    IEnumerator DelayedResetSprite(float duration = -1.0f)
    {
        if (duration < 0.0f)
        {
            yield return new WaitForSeconds(m_AnimationResetDuration);
            m_MonsterOutfitsList[currentMonsterIndex].GetComponent<SpriteRenderer>().sprite = defaultSpritesList[currentMonsterIndex];
        }
        else
        {
            yield return new WaitForSeconds(duration);
            m_MonsterOutfitsList[currentMonsterIndex].GetComponent<SpriteRenderer>().sprite = defaultSpritesList[currentMonsterIndex];
        }
    }

    IEnumerator DelayedTurnOffInvulnerable()
    {
        //dead vfx
        GameObject death_vfx = Instantiate(m_DeathVfxList[currentMonsterIndex], this.transform);
        LeanTween.scaleX(death_vfx, 0, 0.5f);
        death_vfx.AddComponent<EngKidAPI.SelfDestroy>().InitSelfDestroy(0.5f);

        //dead audio
        PlaySound(m_DeadSound);

        //dead animation
        m_MonsterOutfitsList[currentMonsterIndex].GetComponent<SpriteRenderer>().sprite = m_DeadSpritesList[currentMonsterIndex];
        if (animationChangingCoroutine != null)
            StopCoroutine(animationChangingCoroutine);
        animationChangingCoroutine = StartCoroutine(DelayedResetSprite(0.5f));

        yield return new WaitForSeconds(0.5f);
        currentMonsterIndex++; //increase index of monster outfits
        if (currentMonsterIndex >= 4) // monster run out of outfits
        {
            //TODO: end game
            foreach (GameObject outfit in m_MonsterOutfitsList)
            {
                outfit.SetActive(false);
            }
            PlaySound(m_VictorySound);

            //TODO: calculate result here
            m_ActivityManager.OnShowResult(missCounter);
        } 
        else //monster change to next outfit
        {
            //spawning vfx
            GameObject spawn_vfx = Instantiate(m_SpawnVfxList[currentMonsterIndex], this.transform);
            Vector3 og_scale = spawn_vfx.transform.localScale;
            spawn_vfx.transform.localScale = new Vector3(spawn_vfx.transform.localScale.x * 0.5f, spawn_vfx.transform.localScale.y, spawn_vfx.transform.localScale.z);
            LeanTween.scaleX(spawn_vfx, og_scale.x, 0.5f);
            spawn_vfx.AddComponent<EngKidAPI.SelfDestroy>().InitSelfDestroy(0.5f);

            //reset health point and change display to next outfit
            m_HealthPointPerOutfit = 5;
            foreach (GameObject outfit in m_MonsterOutfitsList)
            {
                outfit.SetActive(false);
            }
            m_MonsterOutfitsList[currentMonsterIndex].SetActive(true);

            yield return new WaitForSeconds(0.5f);
            m_MonsterNameText.text = m_ActivityManager.m_MonsterValuesList[currentMonsterIndex];
            isInvulnerable = false; //let the monster be killable again
        }
    }

    public void OnMonsterTapped()
    {
        PlaySound(m_WordsSoundsList[currentMonsterIndex]);
    }

    private void PlaySound(AudioClip audio)
    {
        m_AudioSource.Stop();
        m_AudioSource.clip = audio;
        m_AudioSource.Play();
    }
}
