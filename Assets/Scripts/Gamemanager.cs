using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Fusion;
using TMPro;
using UnityEngine.UI;

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
    public Action ChnageTurn;
    [SerializeField] private TMP_Text yourTurnLabel;
    [SerializeField] private TMP_Text roundMessageLabel;
    [SerializeField] private GameObject nextRoundButton;
    [SerializeField] private GameObject endTurnButton;
    [Networked] public int ID { set; get; }
    public bool isPlayerTurn;
    [SerializeField, Space(20)] private CanvasGroup gameWinPanel;
    [SerializeField] private CanvasGroup gameLosePanel;
    [SerializeField] private CanvasGroup connectionIssuePanel;
    [SerializeField] private GameObject inGamePanel;
    [HideInInspector] public bool canInteract = false;
    [SerializeField] private TMP_Text characterInfo;
    [SerializeField] private TMP_Text noMovesPending;
    private Tween fadeTween;

    [SerializeField, Space(20)] private GameObject drawAndDeployHelp;
    [SerializeField] private GameObject movementHelp;
    [SerializeField] private GameObject attackHelp;
    [SerializeField] private Button helpButton;

    Button _nextRoundButton;
    Button _endTurnButton;
    public bool isGameOver = false;
    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        _nextRoundButton = nextRoundButton.GetComponent<Button>();
        _endTurnButton = endTurnButton.GetComponent<Button>();

        canInteract = true;
        isGameOver = true;
    }
    void Update()
    {

    }
    #endregion

    #region FUNCTIONS
    public void Disconnect()
    {
        Runner.Shutdown();
    }
    public void ExitSession()
    {
        try
        {
            Runner.Shutdown();
        }
        catch
        {
            Debug.Log("No Runner");
        }

        LobbyUI.instance.ResetAll();
        gameLosePanel.interactable = false;
        gameWinPanel.interactable = false;
        gameLosePanel.alpha = 0;
        gameWinPanel.alpha = 0;

        connectionIssuePanel.alpha = 0;
        connectionIssuePanel.interactable = false;

        //Instantiate(networkManger, null).gameObject.SetActive(true);
    }
    public void PlayerExit()
    {
        Vector3 pos = uiCamera.position;
        pos.y = 4;
        uiCamera.position = pos;
        currentRoundStage = RoundStage.WAITING;
        isPlayerTurn = false;
        canInteract = false;
        inGamePanel.SetActive(false);
        connectionIssuePanel.gameObject.SetActive(true);
        connectionIssuePanel.DOFade(1, 0.5f).SetDelay(0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            connectionIssuePanel.interactable = true;
        });
        LobbyUI.instance.PlayerExit();
    }

   /* private void OnGUI()
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
           // Debug.LogWarning("issue");
        }
    }*/
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
        currentRoundStage = RoundStage.USING_CARDS;

        RPC_SetPlayerTurn(1);
        ID = 1;

        isGameOver = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetPlayerTurn(int id)
    {
        isGameOver = false;

        if (Runner.LocalPlayer.PlayerId == id)
        {
            ShowMessage("Your Turn");

            SwitchToCardMode();

            nextRoundButton.SetActive(true);
            endTurnButton.SetActive(false);

            isPlayerTurn = true;
            roundMessageLabel.text = "</b>Round 1</b>\nDraw & Deploy";

            CheckShowHelpCondition();
            helpButton.gameObject.SetActive(true);


        }
        else
        {
            nextRoundButton.SetActive(false);
            endTurnButton.SetActive(false);

            isPlayerTurn = false;
            roundMessageLabel.text = "Enemys Turn";
            helpButton.gameObject.SetActive(false);
        }

        ChnageTurn?.Invoke();
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
        if (BotGameManager.instance.isBotGamePlay) return;

        RPC_ChangeTurn();
        HexagonManager.instance.HideAllHex();
        ResetRound.Invoke();

        drawAndDeployHelp.SetActive(false);
        movementHelp.SetActive(false);
        attackHelp.SetActive(false);
        endTurnButton.SetActive(false);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ChangeTurn()
    {
        if (Runner.IsServer)
        {
            ID += 1;
            if (ID > Runner.SessionInfo.PlayerCount)
            {
                ID = 1;
            }
            RPC_SetPlayerTurn(ID);
        }
    }
    public void NextRound()
    {
        if (BotGameManager.instance.isBotGamePlay) return;

        drawAndDeployHelp.SetActive(false);
        movementHelp.SetActive(false);
        attackHelp.SetActive(false);


        HexagonManager.instance.HideAllHex();

        ResetRound?.Invoke();

        if (currentRoundStage == RoundStage.USING_CARDS)
        {
            SwitchToMoveMode();
            roundMessageLabel.text = "<b>Round 2</b>\nMovement";
        }
        else if (currentRoundStage == RoundStage.MOVE_ITEM)
        {
            SwitchToAttackMode();
            endTurnButton.SetActive(true);
            nextRoundButton.SetActive(false);
            roundMessageLabel.text = "<b>Round 3</b>\nAttack";
        }

        CheckShowHelpCondition();
    }
    public void GameWin(bool flag)
    {
        RPC_GameWin(flag);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_GameWin(bool flag)
    {
        inGamePanel.SetActive(false);
        isGameOver = true;

        if (isLeft == flag)
        {
            gameWinPanel.interactable = true;
            gameWinPanel.gameObject.SetActive(true);
            gameWinPanel.DOFade(1, 0.5f).SetEase(Ease.Linear);
        }
        else
        {
            gameLosePanel.interactable = true;
            gameLosePanel.gameObject.SetActive(true);
            gameLosePanel.DOFade(1, 0.5f).SetEase(Ease.Linear);
        }

        /* if (Runner.IsServer)
             Runner.Shutdown();*/
    }
    public void EnableButtons()
    {
        _endTurnButton.interactable = true;
        _nextRoundButton.interactable = true;
        helpButton.interactable = true;

        canInteract = true;
    }
    public void DisableButtons()
    {
        _endTurnButton.interactable = false;
        _nextRoundButton.interactable = false;
        helpButton.interactable = false;

        canInteract = false;
    }
    public void ShowCharacterDetails(string name, string movementRange, string attackRange, string health, string attackValue, string chancePending)
    {
        string str = "<b>Character Details</b>";

        str += "\nName : " + name;
        str += "\nHealth : " + health;
        str += "\nMovement Range : " + movementRange;
        str += "\nAttack Range : " + attackRange;
        str += "\nAttack Value : " + attackValue;

        if (currentRoundStage == RoundStage.MOVE_ITEM)
            str += "\nMovements Remainig : " + chancePending;
        else if (currentRoundStage == RoundStage.ATTACK)
            str += "\nAttacks Remainig : " + chancePending;

        characterInfo.text = str;
    }
    public void HideCharacterDetails()
    {
        characterInfo.text = "";
    }
    public void ShowNoMovesPending(Vector3 pos, string txt)
    {
        if (fadeTween != null && fadeTween.IsActive())
        {
            fadeTween.Kill();
        }

        noMovesPending.text = txt;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(pos);
        noMovesPending.transform.position = screenPosition;

        noMovesPending.gameObject.SetActive(true);

        Color c = noMovesPending.color;
        c.a = 1;
        noMovesPending.color = c;

        fadeTween = noMovesPending.DOFade(0, 0.5f).SetDelay(1).OnComplete(() =>
         {
             noMovesPending.gameObject.SetActive(false);
         });
    }
    public void ShowHelp()
    {
        helpButton.gameObject.SetActive(false);

        if (currentRoundStage == RoundStage.USING_CARDS)
        {
            drawAndDeployHelp.SetActive(true);
        }
        else if (currentRoundStage == RoundStage.MOVE_ITEM)
        {
            movementHelp.SetActive(true);
        }
        else if (currentRoundStage == RoundStage.ATTACK)
        {
            attackHelp.SetActive(true);
        }
    }
    void CheckShowHelpCondition()
    {
        if (currentRoundStage == RoundStage.USING_CARDS && PlayerPrefs.GetInt("round1") == 0)
        {
            ShowHelp();
        }
        else if (currentRoundStage == RoundStage.MOVE_ITEM && PlayerPrefs.GetInt("round2") == 0)
        {
            ShowHelp();
        }
        else if (currentRoundStage == RoundStage.ATTACK && PlayerPrefs.GetInt("round3") == 0)
        {
            ShowHelp();
        }
    }
    public void HideHelp(bool flag)
    {
        helpButton.gameObject.SetActive(true);


        if (!flag)
            return;

        if (currentRoundStage == RoundStage.USING_CARDS)
        {
            PlayerPrefs.SetInt("round1", 1);
        }
        else if (currentRoundStage == RoundStage.MOVE_ITEM)
        {
            PlayerPrefs.SetInt("round2", 1);
        }
        else if (currentRoundStage == RoundStage.ATTACK)
        {
            PlayerPrefs.SetInt("round3", 1);
        }
        PlayerPrefs.Save();
    }
    #endregion
}
public enum RoundStage
{
    USING_CARDS, MOVE_ITEM, ATTACK, WAITING
}