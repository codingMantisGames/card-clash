using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    #region VARIABLES
    public static CursorChanger instance;
    public Texture2D normalCursor;
    public Texture2D normalCursor_dropCard;
    public Texture2D moveCursor; 
    public Texture2D attackCursor;
    public Texture2D attackCursor_Focused;
    public Vector2 hotSpot = Vector2.zero;
    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        Cursor.SetCursor(normalCursor, hotSpot, CursorMode.Auto);
    }
    void Update()
    {
	
    }
    #endregion

    #region FUNCTIONS
    public void SetDropCursor()
    {
        Cursor.SetCursor(normalCursor_dropCard, hotSpot, CursorMode.Auto);
    }
    public void SetMoveCursor()
    {
        Cursor.SetCursor(moveCursor, hotSpot, CursorMode.Auto);
    }
    public void SetNormalCursor()
    {
        Cursor.SetCursor(normalCursor, hotSpot, CursorMode.Auto);
    }
    public void SetAttackCursor()
    {
        Cursor.SetCursor(attackCursor, hotSpot, CursorMode.Auto);
    }
    public void SetAttackCursorFocued()
    {
        Cursor.SetCursor(attackCursor_Focused, hotSpot, CursorMode.Auto);
    }
    #endregion
}
