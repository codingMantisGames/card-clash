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

        [Header("In Game Data")]
        public List<OfflinePlacableItem> allyCharacters;
        public List<OfflinePlacableItem> enemyCharacters;
        public OfflinePlayerTower allyTower;
        public OfflinePlayerTower enemyTower;

        [Header("Info")]
        [SerializeField] private RoundStage roundStage;
        [SerializeField] private ActionPlanData choosedActionPlan;

        //Hidden
        [HideInInspector] public BotCardManager cardManager;
        [HideInInspector] public CardEvaluator cardEvaluator;
        private GUIStyle bigFontStyle;
        #endregion

        #region UNITY FUNCTIONS
        void Start()
        {
            messages = new List<string>();

            ShowMessage("Hello 🙋‍");

            cardManager = GetComponentInChildren<BotCardManager>();
            cardEvaluator = GetComponentInChildren<CardEvaluator>();

            allyCharacters = new List<OfflinePlacableItem>();
            enemyCharacters = new List<OfflinePlacableItem>();

            bigFontStyle = new GUIStyle();
            bigFontStyle.fontSize = 15;
            bigFontStyle.normal.textColor = Color.white;

            ShowMessage("✅I am all Set!");
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

        public Coroutine StartRoutine(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }
        public void ShowMessage(string message)
        {
            Debug.Log(message);

            messages.Add(message);
            if (messages.Count > 10)
                messages.RemoveAt(0);
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
    }
}

