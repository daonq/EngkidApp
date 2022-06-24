using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodModeManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var currentResultPopup = Resources.FindObjectsOfTypeAll<ResultPopupBehavior>();

        if (Input.GetKeyUp(KeyCode.F12) == true &&
            currentResultPopup.Length > 0)
        {
            Time.timeScale = 1.0f;
            currentResultPopup[0].gameObject.SetActive(true);
            currentResultPopup[0].OnTriggerResultPopUp();
        }    
    }

    public void OnSkipActivity()
    {
        Time.timeScale = 1.0f;
        var currentResultPopup = Resources.FindObjectsOfTypeAll<ResultPopupBehavior>();
        if (currentResultPopup.Length > 0)
        {
            currentResultPopup[0].gameObject.SetActive(true);
            currentResultPopup[0].OnTriggerResultPopUp();
        }
    }
}
