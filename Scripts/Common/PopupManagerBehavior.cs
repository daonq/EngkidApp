using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManagerBehavior : MonoBehaviour
{
    #region Singleton
    private static PopupManagerBehavior _Instance;
    private void Awake()
    {
        _Instance = this;
        if (Debug.isDebugBuild) Debug.Log("PopupManagerBehavior Loaded");
    }
    public static PopupManagerBehavior GetInstance()
    {
        if (_Instance == null)
            Debug.LogError("Error: PopupManagerBehavior instance is null.");

        return _Instance;
    }
    #endregion

    public GameObject m_GeneralConnectionPopup;
    public GameObject m_ServerConnectionPopup;
    public GameObject m_ConstructionPopup;
    public GameObject m_PremiumPopup;
    public GameObject m_UpdatePopup;

    // Start is called before the first frame update
    void Start()
    {
        m_GeneralConnectionPopup.SetActive(false);
        m_ConstructionPopup.SetActive(false);
        m_ServerConnectionPopup.SetActive(false);
        m_UpdatePopup.SetActive(false);
        m_PremiumPopup.SetActive(false);
    }

    public void TriggerUpdatePopUp()
    {
        m_UpdatePopup.SetActive(true);
    }    

    public void TriggerConnectionPopUp()
    {
        m_GeneralConnectionPopup.SetActive(true);
    }    

    public void TriggerConstructionPopUp()
    {
        m_ConstructionPopup.SetActive(true);
    }    

    public void TriggerServerConnectionPopUp()
    {
        m_ServerConnectionPopup.SetActive(true);
    }    

    public void TriggerPremiumPopUp()
    {
        m_PremiumPopup.SetActive(true);
    }

    public void HidePremiumPopUp()
    {
        m_PremiumPopup.SetActive(false);
    }
    
    public void OnMoveToPremiumShop()
    {
        SceneManagerBehavior.GetInstance().OpenParentCheckToIAP(null);
    }    
}
