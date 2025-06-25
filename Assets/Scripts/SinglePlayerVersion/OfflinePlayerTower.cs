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
    [SerializeField] private GameObject brokenBuilding;

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
            if (!isBot)
                item.material = redMaterial;
            else
                item.material = blueMaterial;
        }

        transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 180, 0);

        life = 7;
        lifeLabel.text = life.ToString();
    }

    public void Damage(int damage)
    {
        RPC_ShakeCamera();
        RPC_Damage(damage);
    }

    public void RPC_Damage(int damage)
    {
        life -= damage;

        if (life <= 0)
        {
            RPC_ShowBrokenBuilding();

            BotGameManager.instance.GameWin(!isBot);
        }
    }
    public void RPC_ShakeCamera()
    {
        CameraShake.instance.ShakeCamera(1, 0.5f);
    }
    public void RPC_ShowBrokenBuilding()
    {
        brokenBuilding.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion
}