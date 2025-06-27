using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BotGameManager : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private CodingMantisGames.UtilityAI.AIBrain brain;

    public List<CardData> cardDatas;
    public static BotGameManager instance;
    [Header("UI")]
    [SerializeField] private TMP_Text yourTurnLabel;
    [SerializeField, Space(20)] private RectTransform drawCardsButton;
    [SerializeField] private float cardAppearTime = 2;
    [SerializeField] private Ease cardAppearEase;
    [SerializeField] private Transform uiCamera;

    [SerializeField, Space(20)] private GameObject nextRoundButton;
    [SerializeField] private GameObject endTurnButton;
    [SerializeField] private TMP_Text roundMessageLabel;

    [SerializeField, Space(20)] private GameObject drawAndDeployHelp;
    [SerializeField] private GameObject movementHelp;
    [SerializeField] private GameObject attackHelp;
    [SerializeField] private Button helpButton;

    public bool isBotsTurn = false;
    [HideInInspector] public RoundStage currentRoundStage;
    private bool isGameOver = false;

    [SerializeField, Space(20)] private GameObject offlineTower;
    [SerializeField] private Transform playerTowerSpawnPosition;
    [SerializeField] private Transform botTowerSpawnPosition;

    public Action ChangeTurn;
    public Action ResetRound;
    public Action OnItemSelected;
    public Action CheckPlayerPosition;
    [HideInInspector] public bool isBotGamePlay = false;
    public OfflinePlacableItem currentItemToMove;

    [SerializeField, Space(20)] private CanvasGroup gameWinPanel;
    [SerializeField] private CanvasGroup gameLosePanel;
    [SerializeField] private CanvasGroup connectionIssuePanel;
    [SerializeField] private GameObject inGamePanel;
    [HideInInspector] public bool canInteract = false;
    [SerializeField] private TMP_Text characterInfo;
    [SerializeField] private TMP_Text noMovesPending;
    private Tween fadeTween;

    Button _nextRoundButton;
    Button _endTurnButton;
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
    public void StartBotGame()
    {
        isBotGamePlay = true;

        currentRoundStage = RoundStage.USING_CARDS;

        SetPlayerTurn(false);

        isGameOver = false;

        LobbyUI.instance.HideUIForBotGame();
        LobbyUI.instance.GameStart();

        SpawnTowers();

        CardManager.instance.StartGame();
        LobbyNetworkManager.instance.SwitchToOfflineMode();
    }

    private void SpawnTowers()
    {
        OfflinePlayerTower tower = Instantiate(offlineTower, playerTowerSpawnPosition.position, Quaternion.identity).GetComponent<OfflinePlayerTower>();
        tower.SetTower(false);
        //agent.enemyTower = tower;

        tower = Instantiate(offlineTower, botTowerSpawnPosition.position, Quaternion.identity).GetComponent<OfflinePlayerTower>();
        tower.SetTower(true);
        //agent.allyTower = tower;
    }
    public void HandleAITurnComplete()
    {
        RPC_ChangeTurn();
    }

    private void SetPlayerTurn(bool isBot)
    {
        isBotsTurn = isBot;

        if (isBot)
        {
            //Logic for bots is set here
            nextRoundButton.SetActive(false);
            endTurnButton.SetActive(false);

            roundMessageLabel.text = "Bot's Turn";
            helpButton.gameObject.SetActive(false);

            //agent.StartAgentsTurn();//We ask AI to perform his move
            brain.StartTurn();
        }
        else
        {
            //Logic for player is set here
            SwitchToCardMode();

            nextRoundButton.SetActive(true);
            endTurnButton.SetActive(false);

            roundMessageLabel.text = "</b>Round 1</b>\nDraw & Deploy";

            CheckShowHelpCondition();
            helpButton.gameObject.SetActive(true);
        }

        ChangeTurn?.Invoke();
    }



    //Other usefull functions
    public void SwitchToCardMode()
    {
        currentRoundStage = RoundStage.USING_CARDS;

        drawCardsButton.DOAnchorPos3DY(30, cardAppearTime).SetEase(cardAppearEase);
        uiCamera.DOMoveY(0, cardAppearTime).SetEase(cardAppearEase);
    }
    public void ShowMessage(string message)
    {
        yourTurnLabel.text = message;
        yourTurnLabel.DOFade(1, 0.5f).SetDelay(0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            yourTurnLabel.DOFade(0, 0.4f).SetDelay(1).SetEase(Ease.Linear);
        });
    }

    public void SwitchToMoveMode()
    {
        currentRoundStage = RoundStage.MOVE_ITEM;

        drawCardsButton.DOAnchorPos3DY(-150, cardAppearTime).SetEase(cardAppearEase);
        uiCamera.DOMoveY(4, cardAppearTime).SetEase(cardAppearEase);

        OnItemSelected?.Invoke();
    }
    public void SwitchToAttackMode()
    {
        currentRoundStage = RoundStage.ATTACK;

        drawCardsButton.DOAnchorPos3DY(-150, cardAppearTime).SetEase(cardAppearEase);
        uiCamera.DOMoveY(4, cardAppearTime).SetEase(cardAppearEase);

        OnItemSelected?.Invoke();
    }
    public void MoveCuurentItem(OfflineHexagon target, int index)
    {
        if (currentItemToMove == null)
            return;
        OfflineHexagon currentTile = OfflineHexagonManager.instance.GetHexagon(currentItemToMove.tileIndex);

        List<Vector3> locations = AStarPathFinding.FindPath(currentTile, target);
        currentItemToMove.MoveToPosition(locations.ToArray(), index, isBotsTurn, currentItemToMove.tileIndex);
    }
    public void EndTurn()
    {
        if (!isBotGamePlay) return;

        RPC_ChangeTurn();
        OfflineHexagonManager.instance.HideAllHex();
        ResetRound.Invoke();

        drawAndDeployHelp.SetActive(false);
        movementHelp.SetActive(false);
        attackHelp.SetActive(false);
        endTurnButton.SetActive(false);
    }

    public void RPC_ChangeTurn()
    {
        SetPlayerTurn(!isBotsTurn);
    }

    public void NextRound()
    {
        if (!isBotGamePlay) return;

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
    private void CheckShowHelpCondition()
    {
        /*if (currentRoundStage == RoundStage.USING_CARDS && PlayerPrefs.GetInt("round1") == 0)
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
        }*/
    }
    public void GameWin(bool flag)
    {
        inGamePanel.SetActive(false);
        isGameOver = true;

        if (isBotsTurn == flag)
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