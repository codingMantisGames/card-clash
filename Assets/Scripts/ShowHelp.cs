using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHelp : MonoBehaviour
{
    #region VARIABLES
    public List<GameObject> messages;
    int index;
    #endregion

    #region UNITY FUNCTIONS
    private void OnEnable()
    {
        index = 0;
        ShowMessgae(0);
    }
    private void OnDisable()
    {
        Gamemanager.instance.HideHelp(false);
    }
    #endregion

    #region FUNCTIONS
    public void Close()
    {
        Gamemanager.instance.HideHelp(true);
    }
    public void Next()
    {
        index++;
        ShowMessgae(index);
    }
    public void Prev()
    {
        index--;
        ShowMessgae(index);
    }
    public void ShowMessgae(int num)
    {
        for (int i = 0; i < messages.Count; i++)
        {
            if (i == num)
                messages[i].SetActive(true);
            else
                messages[i].SetActive(false);
        }
    }
    #endregion
}
