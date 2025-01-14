using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Fusion;

public class Gamemanager : NetworkBehaviour
{
    #region VARIABLES
    public List<CardData> cardDatas;
    public static Gamemanager instance;
    public bool isLeft = false;
    public RoundStage currentRoundStage;
    [SerializeField] private Transform cameraHolder;

    [SerializeField] private Transform playerOnePosition;
    [SerializeField] private Transform playerTowPosition;
    public Action OnItemSelected;

    [SerializeField] private Transform uiCamera;
    [SerializeField] private RectTransform drawCardsButton;
    [SerializeField] private float cardAppearTime = 2;
    [SerializeField] private Ease cardAppearEase;
    [HideInInspector] public PlaceableItem currentItemToMove;
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
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 40), "Card Mode"))
        {
            SwitchToCardMode();
        }
        if (GUI.Button(new Rect(210, 0, 200, 40), "Move Mode"))
        {
            SwitchToMoveMode();
        }
        if (GUI.Button(new Rect(420, 0, 200, 40), "Attack Mode"))
        {
            currentRoundStage = RoundStage.ATTACK;
        }
    }
    public void SetTargetCamera(float rot)
    {
        cameraHolder.rotation = Quaternion.Euler(0, rot, 0);
    }
    public void SwitchToCardMode()
    {
        currentRoundStage = RoundStage.USING_CARDS;

        drawCardsButton.DOAnchorPos3DY(30, cardAppearTime).SetEase(cardAppearEase);
        uiCamera.DOMoveY(0, cardAppearTime).SetEase(cardAppearEase);
    }
    public void SwitchToMoveMode()
    {
        currentRoundStage = RoundStage.MOVE_ITEM;

        drawCardsButton.DOAnchorPos3DY(-150, cardAppearTime).SetEase(cardAppearEase);
        uiCamera.DOMoveY(4, cardAppearTime).SetEase(cardAppearEase);

        Gamemanager.instance.OnItemSelected?.Invoke();
    }

    public void MoveCuurentItem(HexagonTile target, int index)
    {
        if (currentItemToMove == null)
            return;
        HexagonTile currentTile = HexagonManager.instance.GetHexagon(currentItemToMove.tileIndex);
        
        List<Vector3> locations = AStarPathFinding.FindPath(currentTile, target);

        currentItemToMove.MoveToPosition(locations.ToArray(), index, isLeft, currentItemToMove.tileIndex);
    }
    #endregion
}
public enum RoundStage
{
    USING_CARDS, MOVE_ITEM, ATTACK, WAITING
}