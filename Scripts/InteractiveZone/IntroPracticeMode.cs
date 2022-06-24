using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroPracticeMode : MonoBehaviour
{

    public GameObject step1;
    public GameObject step2;
    public GameObject step3;
    public GameObject step4;
    public GameObject step5WIN;
    public GameObject step5LOSE;

    public Text txtQuest;
    public Text txtResultAI;
    public Text txtResult;
    public Image imvResult;

    [Header("Audio")]
    public SoundHolder soundHolder;
    public AudioClip step1Audio;
    public AudioClip step2Audio;
    public AudioClip step3Audio;
    public AudioClip step4Audio;
    public AudioClip step5WAudio;
    public AudioClip step5LAudio;

    [Header("Anim")]
    public GameObject snail;
    public GameObject speaker;

    // Start is called before the first frame update
    void Start()
    {
        snail.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowStep1()
    {
        //Time.timeScale = 0.0f;
        gameObject.SetActive(true);
        step1.SetActive(true);
        step2.SetActive(false);
        step3.SetActive(false);
        step4.SetActive(false);
        step5WIN.SetActive(false);
        step5LOSE.SetActive(false);

        soundHolder.PlaySound(step1Audio);

        PlayerPrefs.SetInt("E_INTERACTIVE_ZONE_STEP_1", 1);
        PlayerPrefs.Save();

        Time.timeScale = 1.0f;
    }

    public void ShowStep2()
    {
        gameObject.SetActive(true);
        step1.SetActive(false);
        step2.SetActive(true);
        step3.SetActive(false);
        step4.SetActive(false);
        step5WIN.SetActive(false);
        step5LOSE.SetActive(false);

        soundHolder.PlaySound(step2Audio);

        PlayerPrefs.SetInt("E_INTERACTIVE_ZONE_STEP_2", 1);
        PlayerPrefs.Save();

        Time.timeScale = 1.0f;
    }

    public void ShowStep3()
    {
        gameObject.SetActive(true);
        step1.SetActive(false);
        step2.SetActive(false);
        step3.SetActive(true);
        step4.SetActive(false);
        step5WIN.SetActive(false);
        step5LOSE.SetActive(false);

        soundHolder.PlaySound(step3Audio);

        PlayerPrefs.SetInt("E_INTERACTIVE_ZONE_STEP_3", 1);
        PlayerPrefs.Save();

        Time.timeScale = 1.0f;
    }

    public void ShowStep4()
    {
        gameObject.SetActive(true);
        step1.SetActive(false);
        step2.SetActive(false);
        step3.SetActive(false);
        step4.SetActive(true);
        step5WIN.SetActive(false);
        step5LOSE.SetActive(false);

        soundHolder.PlaySound(step4Audio);

        PlayerPrefs.SetInt("E_INTERACTIVE_ZONE_STEP_4", 1);
        PlayerPrefs.Save();

        Time.timeScale = 1.0f;
    }

    public void ShowStep5W()
    {
        gameObject.SetActive(true);
        step1.SetActive(false);
        step2.SetActive(false);
        step3.SetActive(false);
        step4.SetActive(false);
        step5WIN.SetActive(true);
        step5LOSE.SetActive(false);

        soundHolder.PlaySound(step5WAudio);

        PlayerPrefs.SetInt("E_INTERACTIVE_ZONE_STEP_5_W", 1);
        PlayerPrefs.Save();
    }

    public void ShowStep5L()
    {
        gameObject.SetActive(true);
        step1.SetActive(false);
        step2.SetActive(false);
        step3.SetActive(false);
        step4.SetActive(false);
        step5WIN.SetActive(false);
        step5LOSE.SetActive(true);

        soundHolder.PlaySound(step5LAudio);

        PlayerPrefs.SetInt("E_INTERACTIVE_ZONE_STEP_5_L", 1);
        PlayerPrefs.Save();
    }

    public void PlayNormalSpeed()
    {
        speaker.SetActive(true);
    }

    public void StopNormalSpeed()
    {
        speaker.SetActive(false);
    }

    public void PlaySlowSpeed()
    {
        snail.SetActive(true);
    }

    public void StopSlowSpeed()
    {
        snail.SetActive(false);
    }
}
