using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(MonoBehaviour), true)]
public class SimpleButtonEditor : Editor
{
    #region FUNCTIONS
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space(10);

        GUILayout.Label("FUNCTIONS", EditorStyles.boldLabel); // Display a bold label for the section

        // Get the target object
        MonoBehaviour monoBehaviour = (MonoBehaviour)target;

        // Get all methods with the SimpleButton attribute
        var methods = monoBehaviour.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttributes(typeof(SimpleButtonAttribute), false).Length > 0);

        // Draw buttons for each method
        foreach (var method in methods)
        {
            var attribute = (SimpleButtonAttribute)method.GetCustomAttributes(typeof(SimpleButtonAttribute), false).FirstOrDefault();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(method.Name)) // Change the button label to the method name
            {
                // Get parameters of the method
                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    // If the method has no parameters, invoke it without arguments
                    method.Invoke(monoBehaviour, null);
                }
                else
                {
                    // Prepare arguments based on parameter types
                    object[] arguments = new object[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        Type parameterType = parameters[i].ParameterType;
                        if (parameterType == typeof(int))
                            arguments[i] = 0; // Provide default value for int
                        else if (parameterType == typeof(float))
                            arguments[i] = 0.0f; // Provide default value for float
                        else if (parameterType == typeof(string))
                            arguments[i] = ""; // Provide default value for string
                        else if (parameterType == typeof(bool))
                            arguments[i] = false; // Provide default value for bool
                        // Add more conditions for other types if needed
                    }

                    // Invoke the method with arguments
                    method.Invoke(monoBehaviour, arguments);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    #endregion
}
