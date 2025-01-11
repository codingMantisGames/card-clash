using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    #region VARIABLES
    public static Gamemanager instance;
    public bool isLeft = false;
    public RoundStage currentRoundStage;
    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        currentRoundStage = RoundStage.USING_CARDS;
    }
    void Update()
    {

    }
    #endregion

    #region FUNCTIONS

    #endregion
}
public enum RoundStage
{
    USING_CARDS, MOVE_ITEM, ATTACK
}