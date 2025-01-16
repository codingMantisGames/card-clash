using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private LineRenderer line;
    [SerializeField] private float offset = 0.05f;
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
    public void SetPosition(Vector3 point1, Vector3 point2)
    {
        point1 += new Vector3(0, offset, 0);
        point2 += new Vector3(0, offset, 0);

        line.SetPosition(0, point1);
        line.SetPosition(1, point2);
    }
    #endregion
}
