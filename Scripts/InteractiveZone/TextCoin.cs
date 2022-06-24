using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCoin : MonoBehaviour
{
    public Text txtCoin;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //todo NOBITA get xp from PlayerPrefs (backend not support)
        txtCoin.text = UserDataManagerBehavior.GetInstance().GetCurrentKid().xp + " XP";
    }
}
