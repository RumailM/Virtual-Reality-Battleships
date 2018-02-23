using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorCode : MonoBehaviour
{
    Vector3 north = new Vector3(-1, 0, 0);                                                                      //declareation of directions
    Vector3 south = new Vector3(1, 0, 0);
    Vector3 east = new Vector3(0, 0, 1);
    Vector3 west = new Vector3(0, 0, -1);
    private KeyCode lastKeyPressed;                                                                             //declaration of a keycode so that we can store the last button pressed. This allows us to rebound, or undo the move if its invalid, such has pushing the cursor into a wall
    public bool isFocus;

    private bool m_isRightAxisInUse, m_isLeftAxisInUse, m_isUpAxisInUse, m_isDownAxisInUse;                     //variables that allow the dpad of a controller, an axis input by default, to work like a butto input because that was our intended use

    void gotFocus()                     //WE WILL NEVER LOSE FOCUS WAY-WA                                       //function is called when player is done placing all 10 ships, as player cursor or computer board is irrelevent till then
    {                                                                                                           //isfocus appears in multiple scripts and this is either as a condition or changing its value for certain gameObjects so that we can control, and deny input when we need to. For example we do not want the player to place ships while they are still in the start menu.
        isFocus = true;
        this.transform.Translate(0, 2, 0);                                                                      //moves the cursor onto the computerboard so that it is visible to the player
    }

    

   
    void Start ()
    {
        isFocus = false;                                                                                        //default state of isFocus for player cursor on enemy board is false for the above described reason
        m_isRightAxisInUse = m_isLeftAxisInUse = m_isUpAxisInUse = m_isDownAxisInUse = false;                   

    }


    void Update ()                                                                                              
    {
        if (isFocus)                                                                                            //if cursor has focus, allow input and perform the according movement based on input
        {
            if (Input.GetKeyDown("w"))
            {
                lastKeyPressed = KeyCode.W;
                this.transform.Translate(north, Space.World);
            }
            if (Input.GetKeyDown("s"))
            {
                lastKeyPressed = KeyCode.S;
                this.transform.Translate(south, Space.World);
            }
            if (Input.GetKeyDown("a"))
            {
                lastKeyPressed = KeyCode.A;
                this.transform.Translate(west, Space.World);
            }
            if (Input.GetKeyDown("d"))
            {
                lastKeyPressed = KeyCode.D;
                this.transform.Translate(east, Space.World);
            }

            if (Input.GetAxisRaw("DpadRight") > 0)
            {
                if (m_isRightAxisInUse == false)
                {
                    lastKeyPressed = KeyCode.D;
                    this.transform.Translate(east, Space.World);
                    m_isRightAxisInUse = true;
                }
            }
            if (Input.GetAxisRaw("DpadRight") == 0)
            {
                m_isRightAxisInUse = false;
            }
            if (Input.GetAxisRaw("DpadLeft") < 0)
            {
                if (m_isLeftAxisInUse == false)
                {
                    lastKeyPressed = KeyCode.A;
                    this.transform.Translate(west, Space.World);
                    m_isLeftAxisInUse = true;
                }
            }
            if (Input.GetAxisRaw("DpadLeft") == 0)
            {
                m_isLeftAxisInUse = false;
            }
            if (Input.GetAxisRaw("DpadUp") > 0)
            {
                if (m_isUpAxisInUse == false)
                {
                    lastKeyPressed = KeyCode.W;
                    this.transform.Translate(north, Space.World);
                    m_isUpAxisInUse = true;
                }
            }
            if (Input.GetAxisRaw("DpadUp") == 0)
            {
                m_isUpAxisInUse = false;
            }
            if (Input.GetAxisRaw("DpadDown") < 0)
            {
                if (m_isDownAxisInUse == false)
                {
                    lastKeyPressed = KeyCode.S;
                    this.transform.Translate(south, Space.World);
                    m_isDownAxisInUse = true;
                }
            }
            if (Input.GetAxisRaw("DpadDown") == 0)
            {
                m_isDownAxisInUse = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)                                                                    //when the cursor enters a trigger, that is the invisible wall around the gameboard, rebound it in the opposite direction that it entered from. The opposite is in referenced to the  lastKeyPressed keycode
    {
        if (other.gameObject.tag == "Wall")
        {
            if (lastKeyPressed.ToString().Equals("W"))
                this.transform.Translate(south, Space.World);
            if (lastKeyPressed.ToString().Equals("S"))
                this.transform.Translate(north, Space.World);
            if (lastKeyPressed.ToString().Equals("A"))
                this.transform.Translate(east, Space.World);
            if (lastKeyPressed.ToString().Equals("D"))
                this.transform.Translate(west, Space.World);
        }
    }
}
