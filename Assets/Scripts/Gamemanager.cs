using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    #region VARIABLES
    public List<CardData> cardDatas;
    public static Gamemanager instance;
    public bool isLeft = false;
    public RoundStage currentRoundStage;
    [SerializeField] private Transform cameraHolder;

    [SerializeField] private Transform playerOnePosition;
    [SerializeField] private Transform playerTowPosition;
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
    public  void SetTargetCamera(float rot)
    {
        cameraHolder.rotation = Quaternion.Euler(0, rot, 0);
    }
    #endregion
}
public enum RoundStage
{
    USING_CARDS, MOVE_ITEM, ATTACK
}