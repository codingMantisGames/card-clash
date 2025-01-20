using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Fusion;
using TMPro;

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
    public Action CheckPlayerPosition;
    public Action ResetRound;
    [SerializeField] private TMP_Text yourTurnLabel;
    [SerializeField] private TMP_Text roundMessageLabel;
    [SerializeField] private GameObject nextRoundButton;
    [SerializeField] private GameObject endTurnButton;
    [Networked] public int ID { set; get; }
    public bool isPlayerTurn;
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
            SwitchToAttackMode();
        }
        try
        {
            if (Runner.IsServer && GUI.Button(new Rect(640, 0, 200, 40), "ResetRound"))
            {
                ResetRound.Invoke();
            }
        }
        catch
        {
            Debug.LogWarning("issue");
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
    public void SwitchToAttackMode()
    {
        currentRoundStage = RoundStage.ATTACK;

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
    public void StartGame()
    {
        RPC_SetPlayerTurn(1);
        ID = 1;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetPlayerTurn(int id)
    {
        if (Runner.LocalPlayer.PlayerId == id)
        {
            ShowMessage("Your Turn");

            SwitchToCardMode();

            nextRoundButton.SetActive(true);
            endTurnButton.SetActive(false);

            isPlayerTurn = true;
            roundMessageLabel.text = "Round 1";
        }
        else
        {
            nextRoundButton.SetActive(false);
            endTurnButton.SetActive(false);

            isPlayerTurn = false;
            roundMessageLabel.text = "";
        }
    }
    public void ShowMessage(string message)
    {
        yourTurnLabel.text = message;
        yourTurnLabel.DOFade(1, 0.5f).SetDelay(0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            yourTurnLabel.DOFade(0, 0.4f).SetDelay(1).SetEase(Ease.Linear);
        });
    }
    [SimpleButton]
    public void EndTurn()
    {
        RPC_ChangeTurn();
        HexagonManager.instance.HideAllHex();
        ResetRound.Invoke();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ChangeTurn()
    {
        if (Runner.IsServer)
        {
            ID++;
            if (ID > Runner.SessionInfo.PlayerCount)
            {
                ID = 1;
                RPC_SetPlayerTurn(ID);
            }
        }
    }
    public void NextRound()
    {
        HexagonManager.instance.HideAllHex();

        ResetRound.Invoke();

        if (currentRoundStage == RoundStage.USING_CARDS)
        {
            SwitchToMoveMode();
            roundMessageLabel.text = "Round 2";
        }
        else if (currentRoundStage == RoundStage.MOVE_ITEM)
        {
            SwitchToAttackMode();
            endTurnButton.SetActive(true);
            nextRoundButton.SetActive(false);
            roundMessageLabel.text = "Round 3";
        }
    }
    #endregion
}
public enum RoundStage
{
    USING_CARDS, MOVE_ITEM, ATTACK, WAITING
}