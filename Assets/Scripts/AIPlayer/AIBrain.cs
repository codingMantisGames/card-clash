using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    public class AIBrain : MonoBehaviour
    {
        #region VARIABLES
        [Header("Decision Makers")]
        [SerializeField] private DecisionMaker actionPlanChooser; //Decision Maker #1

        [Header("Other Helpers")]
        [SerializeField] private CharacterEvaluator characterEvaluator; //To get the score of all characters or card

        [Header("Action Plans")]
        [SerializeField] private ActionPlan[] actionPlans;
        //Saved Results
        ActionPlan choosedActionPlan;
        #endregion

        #region UNITY FUNCTIONS
        void Start()
        {

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
            Debug.Log("CHOOSED NORMAL ACTION");
        }
        private void AttackEnemyAction()
        {
            Debug.Log("CHOOSED ATTACK ENEMY ACTION");
        }
        private void ProtectTowerAction()
        {
            Debug.Log("CHOOSED PROTECT TOWER ACTION");
        }
        private void ProtectAllyAction()
        {
            Debug.Log("CHOOSED PROTECT ALLY ACTION");
        }
        #endregion
    }
}
