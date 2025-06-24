using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OfflinePlayerTower : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private List<Renderer> meshRenderers;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material blueMaterial;
    [SerializeField] private TMP_Text lifeLabel;

    [Space(20)]
    public bool isBot = false;
    public int life;
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
    public void SetTower(bool bot)
    {
        isBot = bot;
        foreach (var item in meshRenderers)
        {
            if (isBot)
                item.material = redMaterial;
            else
                item.material = blueMaterial;
        }

        if (isBot)
        {
            transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 180, 0);
        }


        life = 7;
    }
    #endregion
}