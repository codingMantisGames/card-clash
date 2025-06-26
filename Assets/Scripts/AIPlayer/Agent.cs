/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.GPUSort;

namespace CodingMantisGames.UtilityAI
{
    public class Agent : MonoBehaviour
    {
        #region VARIABLES
        [SerializeField] private Deck deck;
        [SerializeField] private DecisionMaker decisionMaker;
        [SerializeField, Space(20)] private Action[] roundOneAction;
        [SerializeField] private Action[] roundTwoAction;
        [SerializeField] private Action[] roundThreeAction;

        private int roundIndex = 0;
        public int RoundIndex => roundIndex;
        Action selectedAction = null;
        public Action SelectedAction => selectedAction;

        [HideInInspector] public List<CardInfo> cards;
        [HideInInspector] public List<CardInfo> discardDeck;
        [HideInInspector] public List<CardInfo> cardInHand;
        [HideInInspector] public bool isActive;
        [HideInInspector] public int cardCounter;
        [HideInInspector] public List<OfflinePlacableItem> enemySpawnables;//to store list of all enemy characters
        [HideInInspector] public List<OfflinePlacableItem> allySpawnables;//to store list of all ally characters
        [SerializeField] private int maxCardsPerRound;
        [SerializeField, Range(0, 2)] private float minDelayBetweenActions;
        [SerializeField, Range(2, 5)] private float maxDelayBetweenActions;

        //All reference needed to do calculations 
        [HideInInspector] public OfflinePlayerTower allyTower;
        [HideInInspector] public OfflinePlayerTower enemyTower;

        [Header("Events")]
        [SerializeField] private UnityEvent OnAIsTurnOver;

        #endregion

        #region UNITY FUNCTIONS
        void Start()
        {
            if (decisionMaker) decisionMaker.Init(this);
            roundIndex = 1;

            cardCounter = maxCardsPerRound;

            discardDeck = new List<CardInfo>();
            cards = new List<CardInfo>();
            enemySpawnables = new List<OfflinePlacableItem>();
            allySpawnables = new List<OfflinePlacableItem>();
        }

        void Update()
        {

        }
        #endregion

        #region FUNCTIONS
        public void TestCode()
        {
            GetNewCard();
        }
        //Start this from gamemanager to start AIs Turn
        public void StartAgentsTurn()
        {
           
        }
        //Call this from SkipRound Action
        public void SkipRound()
        {
          *//*  roundIndex++;

            if (roundIndex >= 4)
            {
                //AIs Turn Over
                SkipTurn();
                return;
            }*//*
        }
        //here the reset happens
        private void SkipTurn()
        {*//*
            Debug.Log("Not its Players Turn!");
            isActive = false;
            OnAIsTurnOver?.Invoke();

            cardCounter = maxCardsPerRound;*//*
        }

        public void PerformAction(int roundID)
        {
           *//* //Select one action based on round.
            if (roundID == 1)
                selectedAction = decisionMaker.DecideAction(roundOneAction);
            else if (roundID == 2)
                selectedAction = decisionMaker.DecideAction(roundTwoAction);
            else
                selectedAction = decisionMaker.DecideAction(roundThreeAction);

            //Perfrom action
            selectedAction.PerformAction(this);*//*
        }

        public void HandlePostAction()
        {
            *//*selectedAction = null;
            Debug.Log("Post Action!");

            if (roundIndex <= 3)
            {
                float delay = Random.Range(minDelayBetweenActions, maxDelayBetweenActions);
                StartCoroutine(PerformActionAfterTime(delay));
            }*//*
        }

        IEnumerator PerformActionAfterTime(float delay)
        {
            yield return new WaitForSeconds(delay);
            PerformAction(roundIndex);
        }

        #region CARD MANAGER
        public void GetNewCard()
        {
            if (cards.Count == 0)
                ShuffleDeck();

            cardCounter--;


            CardInfo data = cards[0];
            cards.Remove(data);

            if (cardInHand == null) cardInHand = new List<CardInfo>();

            cardInHand.Add(data);
        }
        private void ShuffleDeck()
        {
            cards = new List<CardInfo>();

            if (discardDeck.Count == 0)
            {
                foreach (var item in deck.cardInfos)
                {
                    cards.Add(item);
                }
            }
            else
            {
                foreach (var item in discardDeck)
                {
                    cards.Add(item);
                }
            }
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                CardInfo temp = cards[i];
                cards[i] = cards[randomIndex];
                cards[randomIndex] = temp;
            }
        }
        #endregion
        #endregion
    }
    public enum ActionPlanType { NORMAL, PROTECT_TOWER, PROTECT_ALLY, ATTACK_ENEMY };
}*/