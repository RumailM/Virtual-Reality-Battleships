using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleCamera : MonoBehaviour
{
    bool posn;
    public bool isGameStarted;                          


    void Start()
    {
        posn = true;
        isGameStarted = false;                                              //public variable set to true when the start button is hit
    }

  
    void Update()
    {
        if (isGameStarted)                                                  //only has actions if the game has started
        {
            if ((Input.GetKeyDown("r") || Input.GetButtonDown("B")))
            {
                posn = !posn;                                               //toggles between true and false and allows the player to shift between two camera positions based on state of posn
            }
            if (posn)
            {
                transform.position = new Vector3(21, 8, 2);
            }
            else
                transform.position = new Vector3(7, 1, 2);
        }
    }
}
