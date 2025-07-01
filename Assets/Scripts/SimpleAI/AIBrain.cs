using System.Collections;
using System.Collections.Generic;
using CodingMantisGames.UtilityAI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace CodingMantisGames.SimpleAI
{
    public class AIBrain : MonoBehaviour
    {
        #region VARIABLES
        [SerializeField] private ActionPlanData[] actionPlans;

        private List<string> messages;

        [HideInInspector] public List<OfflinePlacableItem> allyCharacters;
        [HideInInspector] public List<OfflinePlacableItem> enemyCharacters;
        [HideInInspector] public OfflinePlayerTower allyTower;
        [HideInInspector] public OfflinePlayerTower enemyTower;

        [Header("Info")]
        public RoundStage roundStage;
        private ActionPlanData choosedActionPlan;

        //Hidden
        [HideInInspector] public BotCardManager cardManager;
        [HideInInspector] public CardEvaluator cardEvaluator;
        [HideInInspector] public SpawnLocationChooser spawnLocationChooser;
        [HideInInspector] public TilesEvaluator tileEvaluator;
        private GUIStyle bigFontStyle;
        #endregion

        #region UNITY FUNCTIONS
        void Start()
        {
            messages = new List<string>();

            ShowMessage("Hello 🙋‍");

            cardManager = GetComponentInChildren<BotCardManager>();
            cardEvaluator = GetComponentInChildren<CardEvaluator>();
            spawnLocationChooser = GetComponentInChildren<SpawnLocationChooser>();
            tileEvaluator = GetComponentInChildren<TilesEvaluator>();

            allyCharacters = new List<OfflinePlacableItem>();
            enemyCharacters = new List<OfflinePlacableItem>();

            bigFontStyle = new GUIStyle();
            bigFontStyle.fontSize = 15;
            bigFontStyle.normal.textColor = Color.white;

            BotGameManager.instance.ChangeTurn += HandleGameTurnReset;

            ShowMessage("✅ I am all Set!");
        }
        private void OnDestroy()
        {
            BotGameManager.instance.ChangeTurn -= HandleGameTurnReset;
        }

        void Update()
        {

        }

        void OnGUI()
        {
            string m = "";
            foreach (var mssg in messages)
            {
                m += mssg;
                m += "\n";
            }
            GUI.Label(new Rect(10, 10, 400, 50), m, bigFontStyle);
        }
        #endregion

        #region FUNCTIONS
        public void StartTurn()
        {
            ShowMessage("🚀 Oh Yaa.It's my turn now!");

            roundStage = RoundStage.USING_CARDS;

            //Calculate Score of all Actions
            foreach (ActionPlanData plan in actionPlans)
            {
                plan.scoreCalculator.CalculateScore(this);
            }

            //Choose one with best Score
            float bestScore = 0;
            choosedActionPlan = null;
            foreach (ActionPlanData plan in actionPlans)
            {
                if (plan.chooseThisOne)
                {
                    choosedActionPlan = plan;
                    break;
                }

                if (plan.scoreCalculator.score > bestScore)
                {
                    bestScore = plan.scoreCalculator.score;
                    choosedActionPlan = plan;
                }
            }

            //Perform choosed Action
            ShowMessage("🎯 Lets choose " + choosedActionPlan.name + " this turn!");
            choosedActionPlan.action.PerformAction(this);
        }
        public void UpdateRound()
        {
            if (roundStage == RoundStage.USING_CARDS) roundStage = RoundStage.MOVE_ITEM;
            else if (roundStage == RoundStage.MOVE_ITEM) roundStage = RoundStage.ATTACK;
            else if (roundStage == RoundStage.ATTACK)
            {
                StartCoroutine(SkipTurnProcedure());
            }
        }

        IEnumerator SkipTurnProcedure()
        {
            yield return new WaitForSeconds(Random.Range(1, 2));

            ShowMessage("🔁 My Turn Over");
            roundStage = RoundStage.WAITING;
            BotGameManager.instance.RPC_ChangeTurn();
        }

        public Coroutine StartRoutine(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }
        public void ShowMessage(string message)
        {
            //Debug.Log(message);

            messages.Add(message);
            if (messages.Count > 10)
                messages.RemoveAt(0);
        }
        public void ContinueMovement()
        {
            if (choosedActionPlan != null) choosedActionPlan.action.MoveIfAnyFlagged(this);
        }
        public void ContinueAttack()
        {
            if (choosedActionPlan != null) choosedActionPlan.action.AttackIfAnyFlagged(this);
        }
        private void HandleGameTurnReset()
        {

        }
        #endregion
    }

    [System.Serializable]
    public class ActionPlanData
    {
        public string name;
        public ActionPlanTypes planType;
        public ScoreCalculator scoreCalculator;
        public Action action;
        public bool chooseThisOne;
    }
}

