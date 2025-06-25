using UnityEditor;
using UnityEngine;

namespace CodingMantisGames.UtilityAI
{
    [CustomEditor(typeof(Agent))]
    public class AgentEditor : Editor
    {
        #region VARIABLES
        #endregion

        #region UNITY FUNCTIONS
        public override void OnInspectorGUI()
        {
            Agent agent = (Agent)target;

            if (Application.isPlaying)
            {
                string message = "";
                message += "Round Index : " + agent.RoundIndex;
                if (agent.SelectedAction != null)
                {
                    message += "\nSelected Action : " + agent.SelectedAction.gameObject.name;
                    message += "\nSelected Action Score : " + agent.SelectedAction.score;
                }
                EditorGUILayout.HelpBox(message, MessageType.Info);
            }

            base.OnInspectorGUI();

            if (GUILayout.Button("Test Sample Code"))
                agent.TestCode();
        }
        #endregion

        #region FUNCTIONS
        #endregion
    }
}
