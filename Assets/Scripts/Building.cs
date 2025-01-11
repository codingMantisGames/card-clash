using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private List<MeshRenderer> meshRenderers;
    [SerializeField] private Material redMat;
    [SerializeField] private Material blueMat;

    [SerializeField, Space(20)] private Outline outline;
    [SerializeField] private bool isMainBuilding = false;
    private bool isLeft;
    bool isSelected;
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
        if (!isMainBuilding)
            SetBuilding(Gamemanager.instance.isLeft);
    }
    void Update()
    {

    }
    #endregion

    #region FUNCTIONS
    public void SetBuilding(bool flag = false)
    {
        isLeft = flag;

        foreach (var item in meshRenderers)
        {
            if (flag)
                item.material = redMat;
            else
                item.material = blueMat;
        }
    }
    #endregion
}
