using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopupBehavior : MonoBehaviour
{
    public enum ResultPopUpTypes
    {
        STANDARD = 0,
        END_OF_LESSON,
        END_OF_UNIT
    }

    [Header("Display overrides")]
    public string m_RestartActivityBtnTxtOverride = "";
    public string m_ExitActivityBtnTxtOverride = "";

    [Header("Display settings")]
    public Text m_RestartActivityButtonTxt;
    public Text m_ExitActivityButtonTxt;

    [Header("General settings")]
    public bool m_IsStory = false;
    public string m_ActivityID = "";
    public ResultPopUpTypes m_ResultPopUpType = ResultPopUpTypes.STANDARD;

    public GameObject m_AllDisplaysObject;
    public GameObject m_BlackScreen;
    public List<GameObject> m_StarsList = new List<GameObject>();
    public Text m_XPText;
    public Text m_DiamondText;
    public AudioSource m_AudioSource;

    public AudioSource m_GoodjobSFX;
    public AudioSource m_ClappingSFX;
    public AudioClip m_GoodjobClip;
    public AudioClip m_HoorayClip;
    public AudioClip m_AwesomeClip;

    public GameObject m_ActivityOwned;

    int score;
    bool isStory;
    string typeStory;

    private void OnEnable()
    {
        m_XPText.text = "";
        m_DiamondText.text = "";

        if (!m_RestartActivityBtnTxtOverride.Equals(""))
            m_RestartActivityButtonTxt.text = m_RestartActivityBtnTxtOverride;

        if (!m_ExitActivityBtnTxtOverride.Equals(""))
            m_ExitActivityButtonTxt.text = m_ExitActivityBtnTxtOverride;

        m_ActivityID = UserDataManagerBehavior.GetInstance().currentSelectedActivityID;

        LeanTween.scale(m_AllDisplaysObject, Vector3.zero, 0.0f);
        foreach (GameObject star in m_StarsList)
        {
            LeanTween.scale(star, Vector3.zero, 0.0f);
        }

        LeanTween.alpha(m_BlackScreen.GetComponent<RectTransform>(), 0.0f, 0.0f);
        m_BlackScreen.SetActive(false);

        m_ActivityOwned = SceneManagerBehavior.GetInstance().transform.GetChild(2).gameObject;
    }

    private void OnDisable()
    {
        UserDataManagerBehavior.GetInstance().SetRecordTime(System.DateTime.Now);
    }

    public void OnQuitActivity()
    {
        SendEventFirebase.SendEventActivityPlay(m_ActivityOwned.name, "game",
                 UserDataManagerBehavior.GetInstance().GetRecordTime().ToString(),
                 "completed", score);
        /*
        SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */

        m_XPText.text = "";
        m_DiamondText.text = "";

        //m_ActivityOwned = SceneManagerBehavior.GetInstance().transform.GetChild(2).gameObject;

        //if (m_ResultPopUpType == ResultPopUpTypes.END_OF_LESSON)
        //{
        //    //TODO: send complete lesson signal to DB
        //    SceneManagerBehavior.GetInstance().BackToLastScene(m_ActivityOwned, false, 2);
        //}
        //else if (m_ResultPopUpType == ResultPopUpTypes.END_OF_UNIT)
        //{
        //    SceneManagerBehavior.GetInstance().BackToLastScene(m_ActivityOwned, false, 3);
        //}   
        //else
        //{
        //    SceneManagerBehavior.GetInstance().BackToLastScene(m_ActivityOwned);
        //}    

        if (m_ResultPopUpType == ResultPopUpTypes.END_OF_LESSON)
        {
            //TODO: send complete lesson signal to DB
            Debug.Log("End of lesson!");
            SceneManagerBehavior.GetInstance().GoToNextActivity(m_ActivityOwned, 2);
        }
        else if (m_ResultPopUpType == ResultPopUpTypes.END_OF_UNIT)
        {
            Debug.Log("End of unit!");
            SceneManagerBehavior.GetInstance().GoToNextActivity(m_ActivityOwned, 3);
        }
        else
        {
            Debug.Log("Normal activity!");
            SceneManagerBehavior.GetInstance().GoToNextActivity(m_ActivityOwned);
        }
    }

    public void OnPlayAgainActivity()
    {
        SendEventFirebase.SendEventActivityPlay(m_ActivityOwned.name, "game",
                UserDataManagerBehavior.GetInstance().GetRecordTime().ToString(),
                "play_again", score);
        /*
        SendEventFirebase.SendUserProperties(
                        UserDataManagerBehavior.GetInstance().currentSelectedKidStars,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidXP,
                        UserDataManagerBehavior.GetInstance().currentKidAccount.diamond.ToString(),
                        UserDataManagerBehavior.GetInstance().currentSelectedKidGender,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidAge,
                        UserDataManagerBehavior.GetInstance().user_premium_permission[0]); */
    }

    public void OnReadAgainStory()
    {
        SendEventFirebase.SendEventActivityPlay(m_ActivityOwned.name, "story_" + typeStory,
                UserDataManagerBehavior.GetInstance().GetRecordTime().ToString(),
                "read_again", score);
    }

    public void OnCompleteStory()
    {
        SendEventFirebase.SendEventActivityPlay(m_ActivityOwned.name, "story_" + typeStory,
                UserDataManagerBehavior.GetInstance().GetRecordTime().ToString(),
                "completed", score);
    }

    public void OnTriggerResultPopUp(int score = 3, bool isStory = false, string typeStory = "")
    {
        this.score = score;
        this.isStory = isStory;
        this.typeStory = typeStory;

        UserDataManagerBehavior.GetInstance().OnPostLearnTimeRecord("game_activity", "on_show_result_popup");
        //SendEventFirebase.SendEventActivityPlay(m_ActivityOwned.name, "game", UserDataManagerBehavior.GetInstance().GetRecordTime().ToString(), "completed", score);

        m_BlackScreen.SetActive(true);
        LeanTween.alpha(m_BlackScreen.GetComponent<RectTransform>(), 0.33f, 1.0f).setOnComplete(() =>
        {
            if (score < 0)
                score = 0;

            m_XPText.text = "";
            m_DiamondText.text = "";

            m_AudioSource.Play();
            LeanTween.scale(m_AllDisplaysObject, Vector3.one, 0.5f).setOnComplete(() =>
            {

                //stars
                int i = 1;
                foreach (GameObject star in m_StarsList)
                {
                    if (i <= score)
                    {
                        LeanTween.scale(star, Vector3.one, 0.5f).setDelay(0.5f * i);
                        star.GetComponent<AudioSource>().PlayDelayed(0.5f * i);
                    }
                    i++;
                }

                //XP text 
                m_XPText.gameObject.GetComponent<AudioSource>().Play();
                LeanTween.value(this.gameObject, 0, 5, 1.0f).setOnUpdate((float val) =>
                {
                    m_XPText.text = Mathf.RoundToInt(val).ToString();
                });

                //diamonds text
                m_DiamondText.gameObject.GetComponent<AudioSource>().PlayDelayed(1.0f); ;
                LeanTween.value(this.gameObject, 0, score, 1.0f).setDelay(1.0f).setOnUpdate((float val) =>
                {
                    m_DiamondText.text = Mathf.RoundToInt(val).ToString();
                });

                //SFX
                if (m_ResultPopUpType == ResultPopUpTypes.STANDARD)
                {
                    m_GoodjobSFX.clip = m_GoodjobClip;
                }
                else if (m_ResultPopUpType == ResultPopUpTypes.END_OF_LESSON)
                {
                    m_GoodjobSFX.clip = m_HoorayClip;
                }
                else
                {
                    m_GoodjobSFX.clip = m_AwesomeClip;
                }
                m_GoodjobSFX.PlayDelayed(2.0f);
                m_ClappingSFX.PlayDelayed(2.0f);
            });

            //post result
            if (m_IsStory == false)
            {
                m_ActivityID = UserDataManagerBehavior.GetInstance().currentSelectedActivityID;

                if (m_ResultPopUpType != ResultPopUpTypes.END_OF_LESSON)
                {
                    
                    ActivityRecord record = new ActivityRecord(m_ActivityID, score);
                    Debug.Log("Account: " + UserDataManagerBehavior.GetInstance().currentSelectedKidID + " Activity " + record.activity_id + " \nhas score: " + record.score + " \njson: " + JsonUtility.ToJson(record) + " \nkid token: " + UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken + "\nRequest URI: " + ((m_ResultPopUpType == ResultPopUpTypes.END_OF_LESSON || m_IsStory == true) ? DataBaseInterface.LESSON_COMPLETE : DataBaseInterface.ACTIVITY_SCORING_URI));
                    DataBaseInterface.GetInstance().PostJSONRequest(
                            //(m_IsStory == true)? DataBaseInterface.LESSON_COMPLETE:DataBaseInterface.ACTIVITY_SCORING_URI,
                            DataBaseInterface.ACTIVITY_SCORING_URI,
                            JsonUtility.ToJson(record),
                            null,
                            null,
                            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
                        );
                }
                else if (m_ResultPopUpType == ResultPopUpTypes.END_OF_LESSON)
                {
                    m_ActivityID = UserDataManagerBehavior.GetInstance().currentSelectedActivityID;

                    ActivityRecord record = new ActivityRecord(m_ActivityID, score);
                    Debug.Log("Account: " + UserDataManagerBehavior.GetInstance().currentSelectedKidID + " Activity " + record.activity_id + " \nhas score: " + record.score + " \njson: " + JsonUtility.ToJson(record) + " \nkid token: " + UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken + "\nRequest URI: " + ((m_ResultPopUpType == ResultPopUpTypes.END_OF_LESSON || m_IsStory == true) ? DataBaseInterface.LESSON_COMPLETE : DataBaseInterface.ACTIVITY_SCORING_URI));
                    DataBaseInterface.GetInstance().PostJSONRequest(
                            //(m_IsStory == true)? DataBaseInterface.LESSON_COMPLETE:DataBaseInterface.ACTIVITY_SCORING_URI,
                            DataBaseInterface.ACTIVITY_SCORING_URI,
                            JsonUtility.ToJson(record),
                            null,
                            null,
                            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
                        );


                    m_ActivityID = UserDataManagerBehavior.GetInstance().unitDetailsInfo.lessons[UserDataManagerBehavior.GetInstance().currentLessonIndex]._id;

                    LessonRecord record_lesson = new LessonRecord(m_ActivityID, score);
                    Debug.Log("Lesson " + record_lesson.lesson_id + " \nhas score: " + record_lesson.score + " \njson: " + JsonUtility.ToJson(record_lesson) + " \nkid token: " + UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken + "\nRequest URI: " + ((m_ResultPopUpType == ResultPopUpTypes.END_OF_LESSON || m_IsStory == true) ? DataBaseInterface.LESSON_COMPLETE : DataBaseInterface.ACTIVITY_SCORING_URI));
                    DataBaseInterface.GetInstance().PostJSONRequest(
                            //(m_IsStory == true)? DataBaseInterface.LESSON_COMPLETE:DataBaseInterface.ACTIVITY_SCORING_URI,
                            DataBaseInterface.LESSON_COMPLETE,
                            JsonUtility.ToJson(record_lesson),
                            null,
                            null,
                            UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
                        );
                }
            }
            else
            {
                m_ActivityID = UserDataManagerBehavior.GetInstance().unitDetailsInfo.lessons[2]._id;

                LessonRecord record = new LessonRecord(m_ActivityID, score);
                Debug.Log("Lesson " + record.lesson_id + " \nhas score: " + record.score + " \njson: " + JsonUtility.ToJson(record) + " \nkid token: " + UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken + "\nRequest URI: " + ((m_ResultPopUpType == ResultPopUpTypes.END_OF_LESSON || m_IsStory == true) ? DataBaseInterface.LESSON_COMPLETE : DataBaseInterface.ACTIVITY_SCORING_URI));
                DataBaseInterface.GetInstance().PostJSONRequest(
                        //(m_IsStory == true)? DataBaseInterface.LESSON_COMPLETE:DataBaseInterface.ACTIVITY_SCORING_URI,
                        DataBaseInterface.LESSON_COMPLETE,
                        JsonUtility.ToJson(record),
                        null,
                        null,
                        UserDataManagerBehavior.GetInstance().currentSelectedKidIDToken
                    );
            }
        });
    }
}

[System.Serializable]
public class ActivityRecord
{
    [SerializeField] public string activity_id = "";
    //[SerializeField] public string lesson_id = "";
    [SerializeField] public string score = "";

    public ActivityRecord(string id, int new_score)
    {
        activity_id = id;
        //lesson_id = id;
        score = new_score.ToString();
    }
}

[System.Serializable]
public class LessonRecord
{
    //[SerializeField] public string activity_id = "";
    [SerializeField] public string lesson_id = "";
    [SerializeField] public string score = "";

    public LessonRecord(string id, int new_score)
    {
        //activity_id = id;
        lesson_id = id;
        score = new_score.ToString();
    }
}
