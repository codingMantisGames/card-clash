using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class AIBrain : MonoBehaviour
    {
        #region VARIABLES
        [SerializeField] private bool showAIsThought;
        [Header("Decision Makers")]
        [SerializeField] private DecisionMaker actionPlanChooser; //Decision Maker #1
        [SerializeField] private DecisionMaker utilityAI;

        [Header("Other Helpers")]
        [SerializeField] private CharacterEvaluator characterEvaluator; //To get the score of all characters or card
        [SerializeField] private BotCardManager botCardManager;
        [SerializeField] private SpawnLocationChooser spawnLocationChooser;
        [SerializeField] private OfflineHexagonManager offlineHexagonManager;

        [Header("Action Plans")]
        [SerializeField] private ActionPlan[] actionPlans;
        [Header("Normal")]
        [SerializeField] private Action[] normalRoundOneActions;
        [SerializeField] private Action[] normalRoundTwoActions;

        //Private Variables
        [HideInInspector] public BotCardManager cardManager;//Script that manages all cards for Bot Player
        public List<OfflinePlacableItem> allyCharacters;
        public List<OfflinePlacableItem> enemyCharacters;
        private int roundNumber;

        //Saved Results
        ActionPlan choosedActionPlan;
        Action choosedAction;
        #endregion

        #region UNITY FUNCTIONS
        void Start()
        {
            actionPlanChooser.Init(this);
            utilityAI.Init(this);

            cardManager = GetComponentInChildren<BotCardManager>();
            allyCharacters = new List<OfflinePlacableItem>();
            enemyCharacters = new List<OfflinePlacableItem>();
        }

        void Update()
        {

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

            roundNumber = 1;

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

            roundNumber++;
            if (choosedActionPlan.planType == ActionPlanType.NORMAL)
            {
                if (roundNumber == 2) DecideMoveCharactersOrNot_NormalAction();
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
            roundNumber++;
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
            ShowMessage("🧙 We have to move characters now.");
        }

        private void ShowMessage(string message)
        {
            if (showAIsThought)
            {
                Debug.Log(message);
            }
        }
        #endregion
    }
}
