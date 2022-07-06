using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UISettings", menuName = "ScriptableObjects/Settings/UISettings")]
public class UISettings : ScriptableObject
{
    
    #region Singleton
    public static UISettings instance;
        
    private void OnEnable()
    {
        if (instance == null)
            instance = this;
    }

    private void OnDisable()
    {
        instance = null;
    }
    #endregion

    [Header("Input Controller")] 
    #region Input Controller
    public List<Sprite> controllerTypeImages;
    [Header("Players")] 
    public List<Color> colors;
    #endregion
}
