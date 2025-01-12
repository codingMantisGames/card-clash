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
    public Sprite topRedCard;
    public Sprite topBlueCard;

    [Space(20)]
    public Sprite bottomRedCard;
    public Sprite bottomBlueCard;
    #endregion

    #region FUNCTIONS

    #endregion
}
