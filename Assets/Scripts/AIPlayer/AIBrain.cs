using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class AIBrain : MonoBehaviour
    {
        #region VARIABLES
        [SerializeField] private RoundStage roundStage;
        [SerializeField] private bool showAIsThought;
        [Header("Decision Makers")]
        [SerializeField] private DecisionMaker actionPlanChooser; //Decision Maker #1
        [SerializeField] private DecisionMaker utilityAI;

        [Header("Other Helpers")]
        [SerializeField] private CharacterEvaluator characterEvaluator; //To get the score of all characters or card
        [SerializeField] private TileEvaluator tileEvaluator;
        [SerializeField] private BotCardManager botCardManager;
        [SerializeField] private SpawnLocationChooser spawnLocationChooser;
        [SerializeField] private OfflineHexagonManager offlineHexagonManager;

        [Header("Action Plans")]
        [SerializeField] private ActionPlan[] actionPlans;
        [Header("Normal")]
        [SerializeField] private Action[] normalRoundOneActions;
        [SerializeField] private Action[] normalRoundTwoActions;
        [SerializeField] private Action[] normalRoundThreeActions;

        //Private Variables
        [HideInInspector] public BotCardManager cardManager;//Script that manages all cards for Bot Player
        public List<OfflinePlacableItem> allyCharacters;
        public List<OfflinePlacableItem> enemyCharacters;

        //Saved Results
        ActionPlan choosedActionPlan;
        Action choosedAction;
        List<string> messages;
        private GUIStyle bigFontStyle;
        #endregion

        #region UNITY FUNCTIONS
        void Start()
        {
            actionPlanChooser.Init(this);
            utilityAI.Init(this);

            cardManager = GetComponentInChildren<BotCardManager>();
            allyCharacters = new List<OfflinePlacableItem>();
            enemyCharacters = new List<OfflinePlacableItem>();

            messages = new List<string>();

            bigFontStyle = new GUIStyle();
            bigFontStyle.fontSize = 15; // Set desired font size
            bigFontStyle.normal.textColor = Color.white; // Optional: set text color

            messages.Add("Hello 🙋‍");
        }

        void Update()
        {

        }
        void OnGUI()
        {
            if (showAIsThought)
            {
                string m = "";
                foreach (var mssg in messages)
                {
                    m += mssg;
                    m += "\n";
                }
                GUI.Label(new Rect(10, 10, 400, 50), m, bigFontStyle);
            }


            if (GUI.Button(new Rect(500, 10, 200, 40), showAIsThought ? "Hide Thought" : "Show Thought"))
            {
                showAIsThought = !showAIsThought;
            }
        }
        #endregion

        #region FUNCTIONS
        [SimpleButton]
        public void TestCode()
        {
            StartTurn();
        }

        //Start this from gamemanager to start AIs Turn
        public void StartTurn()
        {
            ShowMessage("🚀 Oh Yaa.It's my turn now!");

            roundStage = RoundStage.USING_CARDS;

            choosedActionPlan = actionPlanChooser.DecideActionPlan(actionPlans); //Choose one action plan for this round.

            switch (choosedActionPlan.planType)
            {
                case ActionPlanType.NORMAL:
                    NormalAction();
                    break;
                case ActionPlanType.PROTECT_TOWER:
                    ProtectTowerAction();
                    break;
                case ActionPlanType.PROTECT_ALLY:
                    ProtectAllyAction();
                    break;
                case ActionPlanType.ATTACK_ENEMY:
                    AttackEnemyAction();
                    break;
            }
        }

        private void NormalAction()
        {
            ShowMessage("🧠 Choosed Normal Action Plan.");

            //Decision Maker #2
            choosedAction = utilityAI.DecideAction(normalRoundOneActions);

            ShowMessage("🎯 " + choosedAction.gameObject.name + " Action is choosed!!");

            if (choosedAction) choosedAction.PerformAction(this);
        }
        private void AttackEnemyAction()
        {
            ShowMessage("🧠 Choosed Attack Enemy Action Plan.");
        }
        private void ProtectTowerAction()
        {
            ShowMessage("🧠 Choosed Protect Tower Action Plan.");
        }
        private void ProtectAllyAction()
        {
            ShowMessage("🧠 Choosed Protect Ally Action Plan.");
        }

        public void NextRound()
        {
            ShowMessage("⚠️ Let's Skip this round.");

            IncrementRound();
            if (choosedActionPlan.planType == ActionPlanType.NORMAL)
            {
                if (roundStage == RoundStage.MOVE_ITEM) DecideMoveCharactersOrNot_NormalAction();
            }
        }
        private void IncrementRound()
        {
            if (roundStage == RoundStage.USING_CARDS) roundStage = RoundStage.MOVE_ITEM;
            else if (roundStage == RoundStage.MOVE_ITEM) roundStage = RoundStage.ATTACK;
            else if (roundStage == RoundStage.ATTACK)
            {
                ShowMessage("🔁 My Turn Over");
                roundStage = RoundStage.WAITING;
                BotGameManager.instance.RPC_ChangeTurn();
            }
        }
        public void HandleOnDrawCardsComplete_NormalAction()
        {
            ShowMessage("✅ Successfully Card Drawn from the deck!");
            //Decision Maker #3
            StartCoroutine(DropCardsProcedure());
        }
        IEnumerator DropCardsProcedure()
        {
            yield return null;

            int cardToDrop = Random.Range(enemyCharacters.Count, cardManager.cardInHand.Count + 1);
            cardToDrop = Mathf.Clamp(cardToDrop, 0, cardManager.cardInHand.Count);

            if (cardToDrop == 0) cardToDrop = 1;

            ShowMessage("💭 Enemy has " + enemyCharacters.Count + " Characters so I should drop " + cardToDrop + " Cards this round!");

            for (int i = 0; i < cardToDrop; i++)
            {
                CardInfo selectedCard = characterEvaluator.EvaluateCard(botCardManager.cardInHand.ToArray());

                if (selectedCard != null && botCardManager.cardInHand.Contains(selectedCard))
                {
                    botCardManager.cardInHand.Remove(selectedCard);
                }

                OfflineHexagon spawnLocation = spawnLocationChooser.GetASpawnLocation();
                offlineHexagonManager.SpawnItem(selectedCard.topCard.cardID, spawnLocation);

                ShowMessage("🧙 " + selectedCard.topCard.name + " Character Spawned");

                yield return new WaitForSeconds(Random.Range(1, 2));
            }
            ShowMessage("✅ Successfully Droped all needed cards!");
            IncrementRound();
            yield return new WaitForSeconds(1);
            DecideMoveCharactersOrNot_NormalAction();
        }

        public void DecideMoveCharactersOrNot_NormalAction()
        {
            //Decision Maker #4
            choosedAction = utilityAI.DecideAction(normalRoundTwoActions);

            choosedAction.PerformAction(this);
        }

        public void HandleOnMoveCharacterChoosed_NormalAction()
        {
            ShowMessage("💭 We have to move characters now.");

            foreach (OfflinePlacableItem item in allyCharacters)
            {
                if (item.isFlaggedCharacter)
                {
                    OfflineHexagon[] result = item.GetAllTilesInRange(RoundStage.MOVE_ITEM);

                    OfflineHexagon hexgonToMove = tileEvaluator.ChooseBestTileToMove(ActionPlanType.NORMAL, result, item.transform);

                    hexgonToMove.isMarkedByAI = true;
                    item.hexagonFlagged = hexgonToMove;
                }
            }

            MoveCharacterIfAny();
        }

        public void MoveCharacterIfAny()
        {
            bool flag = false;
            foreach (var item in allyCharacters)
            {
                if (item.isFlaggedCharacter)
                {
                    StartCoroutine(MoveItemProcedure(item));
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                //means we go to next round
                IncrementRound();
                ShowMessage("✅ Successfully Completed move Actions.");

                DecideMoveAttack_NormalAction();
            }
        }
        IEnumerator MoveItemProcedure(OfflinePlacableItem item)
        {
            yield return new WaitForSeconds(Random.Range(1, 2));

            BotGameManager.instance.MoveCurentItem(item.hexagonFlagged, OfflineHexagonManager.instance.GetIndex(item.hexagonFlagged), item);
            item.isFlaggedCharacter = false;
            ShowMessage("🧙 Started Moving " + item.gameObject.name);
        }

        private void DecideMoveAttack_NormalAction()
        {
            //Decision Maker #5
            choosedAction = utilityAI.DecideAction(normalRoundThreeActions);

            choosedAction.PerformAction(this);
        }

        public void AttackEnemey_NormalAction()
        {
            ShowMessage("🗡️ Now we have to attack them.");

            foreach (var item in allyCharacters)
            {
                OfflineHexagon[] result = item.GetAllTilesInRange(RoundStage.ATTACK);
                foreach (var tile in result)
                {
                    if (tile.isUsed && tile.isUsedByEnemy)
                    {
                        OfflinePlacableItem enemy = tile.GetPlayer();
                        enemy.isFlaggedCharacter = true;
                        item.enemyToAttack = enemy;
                    }
                }
            }

            AttackEnemyIfAny();
        }

        public void AttackEnemyIfAny()
        {
            bool flag = false;
            foreach (var item in allyCharacters)
            {
                if (item.enemyToAttack != null)
                {
                    StartCoroutine(AttackProcedure(item));
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                //means we go to next round
                IncrementRound();
                ShowMessage("✅ Successfully Completed attack Actions.");
            }
        }
        IEnumerator AttackProcedure(OfflinePlacableItem item)
        {
            yield return new WaitForSeconds(Random.Range(1, 2));
            if (item.enemyToAttack != null)
            {
                item.Attack(item.enemyToAttack.tileIndex);
                item.enemyToAttack = null;
            }
            else
            {
                AttackEnemyIfAny();
            }
        }

        private void ShowMessage(string message)
        {
            if (showAIsThought)
            {
                Debug.Log(message);

                messages.Add(message);
                if (messages.Count > 10)
                    messages.RemoveAt(0);
            }
        }
        #endregion
    }
}
