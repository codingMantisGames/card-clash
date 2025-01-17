using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deck", menuName = "ScriptableObject/New Deck")]
public class Deck : ScriptableObject
{
    #region VARIABLES
    public List<CardInfo> cardInfos;
    #endregion

    #region UNITY FUNCTIONS
    #endregion

    #region FUNCTIONS
    private void OnValidate()
    {
        foreach (var item in cardInfos)
        {
            string name = "";
            if (item.topCard)
                name += item.topCard.name;
            if (item.bottomCard)
                name += (" X " + item.bottomCard.name);

            item.name = name;
        }
    }
    #endregion
}
[System.Serializable]
public class CardInfo
{
    [HideInInspector] public string name;
    public CardData topCard;
    public CardData bottomCard;
}