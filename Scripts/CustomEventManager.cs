using System;
using UnityEngine;

public class CustomEventManager : MonoBehaviour
{
    #region Singleton
    private static CustomEventManager _Instance;
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
    public static CustomEventManager GetInstance()
    {
        if (_Instance == null)
            Debug.LogError("Error: CustomEventManager instance is null.");

        return _Instance;
    }
    #endregion

    public event Action onParentLoggedIn;
    public void ParentLoggedIn()
    {
        if (onParentLoggedIn != null)
            onParentLoggedIn();
    }   
    
    public void OnEmergencyClose()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public event Action onParentLoggedInWithToken;
    public void ParentLoggedInWithToken()
    {
        onParentLoggedInWithToken?.Invoke();
    }
}
