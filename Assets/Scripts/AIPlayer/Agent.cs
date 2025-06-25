using UnityEngine;
using UnityEngine.Events;

namespace CodingMantisGames.UtilityAI
{
    public class Agent : MonoBehaviour
    {
        #region VARIABLES
        [SerializeField] private DecisionMaker decisionMaker;
        [SerializeField, Space(20)] private Action[] roundOneAction;
        [SerializeField] private Action[] roundTwoAction;
        [SerializeField] private Action[] roundThreeAction;

        private int roundIndex = 0;
        public int RoundIndex => roundIndex;
        Action selectedAction = null;
        public Action SelectedAction => selectedAction;

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
        }

        void Update()
        {

        }
        #endregion

        #region FUNCTIONS
        public void TestCode()
        {
            PerformAction(1);
        }
        //Start this from gamemanager to start AIs Turn
        public void StartAgentsTurn()
        {
            roundIndex = 1;

            PerformAction(roundIndex);
        }
        //Call this from SkipRound Action
        public void SkipRound()
        {
            roundIndex++;

            if (roundIndex >= 4)
            {
                //AIs Turn Over
                SkipTurn();
                return;
            }

            PerformAction(roundIndex);
        }

        private void SkipTurn()
        {
            Debug.Log("Not its Players Turn!");
            OnAIsTurnOver?.Invoke();    
        }

        public void PerformAction(int roundID)
        {
            //Select one action based on round.
            if (roundID == 1)
                selectedAction = decisionMaker.DecideAction(roundOneAction);
            else if (roundID == 2)
                selectedAction = decisionMaker.DecideAction(roundTwoAction);
            else
                selectedAction = decisionMaker.DecideAction(roundThreeAction);

            Debug.Log(selectedAction.gameObject.name + " SELECTED. With Score " + selectedAction.score);

            //Perfrom action
            selectedAction.PerformAction(this);
        }

        public void HandlePostAction()
        {
            selectedAction = null;
            Debug.Log("Post Action!");
        }
        #endregion
    }
}