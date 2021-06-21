using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public Transform TextPopUp;

    [Space]
    [Header("Characters:")]
    public GameObject Character;

    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null)
            {
                _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            }
            return _i;
        }
    }

    public void SetUp()
    {
        Debug.Log("GameAssets.SetUp()");
    }
}
