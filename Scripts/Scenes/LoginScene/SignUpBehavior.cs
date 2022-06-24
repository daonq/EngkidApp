using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignUpBehavior : MonoBehaviour
{
    public GameObject m_ParentActivationWindow;

    public void OnSignUpComplete()
    {
        m_ParentActivationWindow.SetActive(true);
    }    
}
