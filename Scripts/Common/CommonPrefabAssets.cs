using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonPrefabAssets : MonoBehaviour
{
    #region Singleton
    private static CommonPrefabAssets _Instance;
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
    public static CommonPrefabAssets GetInstance()
    {
        if (_Instance == null)
            Debug.LogError("Error: AuthorizationManager instance is null.");

        return _Instance;
    }
    #endregion

    [Header("Prefabs")]
    public GameObject m_ActivityDisplay;

    [Header("Images")]
    public Sprite m_LabelSprite;

    //[Header("Audio")]
}
