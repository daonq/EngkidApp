using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillProgress : MonoBehaviour
{
    bool lockTime;
    float timeLock;

    public Image countDown;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckTime();
    }

    private void CheckTime()
    {
        //if (lockTime == true)
        //{
        //    countDown.fillAmount -= 1.0f / timeLock * Time.deltaTime;
        //}
        //if (countDown.fillAmount <= 0)
        //{
        //    lockTime = false;
        //}

        if (lockTime == true)
        {
            countDown.fillAmount += 1.0f / timeLock * Time.deltaTime;
        }
        if (countDown.fillAmount > 0)
        {
            lockTime = true;
        }
    }

    public void LockTime(float duration)
    {
        //countDown.fillAmount = 1.0f;
        //timeLock = duration;
        //lockTime = true;

        countDown.fillAmount = 0.0f;
        timeLock = duration;
        lockTime = true;
    }

    public void FillEmpty()
    {
        countDown.fillAmount = 0.0f;
        lockTime = false;
    }
}
