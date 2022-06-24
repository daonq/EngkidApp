using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationManagerBehavior : MonoBehaviour
{
    #region Singleton
    private static LocalizationManagerBehavior _Instance;
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public static LocalizationManagerBehavior GetInstance()
    {
        if (_Instance == null)
            Debug.LogError("Error: LocalizationManagerBehavior instance is null.");

        return _Instance;
    }
    #endregion

    public string m_CurrentLanguageCode = "vi";
    public Font m_CurrentFont;

    public Font GetCurrentFont()
    {
        return m_CurrentFont;
    }

    public string GetLanguageCode()
    {
        return m_CurrentLanguageCode;
    }
}
