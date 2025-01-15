using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CardData",menuName = "ScriptableObject/New Card Data")]
public class CardData : ScriptableObject
{
    #region VARIABLES
    public string cardID;
    public GameObject prefab; 

    [Space(20)]
    public Sprite red;
    public Sprite blue;
    #endregion

    #region FUNCTIONS

    #endregion
}
