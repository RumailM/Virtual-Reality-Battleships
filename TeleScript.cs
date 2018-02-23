using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleScript : MonoBehaviour
{
    //private Rigidbody rb;
    public bool isPlaced;                                                                                                                               //variables used to keep track the state of a ship in unity, isPlaced refers to if a ship has succesfully been placed in on the board and its tags at its position are stored in the array
    public bool isCounted;                                                                                                                              //isCounted is false as long as the placed ship remains placed and before the counter increments in the FoundationScriptBoard script. This is used as a reference so that the isFocus variable can be passed to the next ship and also so that the next ship be rendered and teleoported onto the board
    public bool isFocus;                                                                                                                                //variable that denies input if false and allows input if true
    public bool isPlacable;                                                                                                                             //remains false as long as it collides with another ship on the gameboard
    public int health;                                                                                                                                  //declares health of a ship, used to facilitate the color, or health of the ship as it takes shots
    int maxHealth;                                                                                                                                      //declares the max health, used as a reference in calculation when lerping the color of a ship from green to red

    Vector3 north = new Vector3(-1, 0, 0);                                                                                                              //direction vectors
    Vector3 south = new Vector3(1, 0, 0);
    Vector3 east = new Vector3(0, 0, 1);
    Vector3 west = new Vector3(0, 0, -1);
    Vector3 down = new Vector3(0, -1, 0);
    Vector3 origin = new Vector3(0.5f, 1.01f, -9.5f);
    Material startMat;                                                                                                                                  //green color for max health
    Material endMat;                                                                                                                                    //red color for ending health
    public Renderer rend;                                                                                                                               //refernce to a renderer

    private bool m_isRightAxisInUse, m_isLeftAxisInUse, m_isUpAxisInUse, m_isDownAxisInUse;                                                             //used to convert axis input into button inputs, see CursorCode script

    private KeyCode lastKeyPressed;                                                                                                                     //used to rebound a boat from invalid movement/rotation, i.e. undo the the rotation/movement if it collides with a wall

    // Use this for initialization
    string getCount()                                                                                                                                   //returns the current count, i.e. the current ship that is in the process of being placed
    {
        return GameObject.Find("FoundationScriptBoard").GetComponent<FoundationCode>().count.ToString();
    }

    void Start ()
    {
        isCounted = false;
        isPlaced = false;
        isPlacable = true;

        if (this.name.Equals("1") || this.name.Equals("2") || this.name.Equals("3") || this.name.Equals("4"))                                           //gives ships 1-4, max health and current health of 2, 5-7 max and current health of 3, 8-9 get 4 and 10 gets 5, decoding ship tag into health and max health and saving it within each instance of the object 
            maxHealth = health = 2;
        else if (this.name.Equals("5") || this.name.Equals("6") || this.name.Equals("7"))
            maxHealth = health = 3;
        else if (this.name.Equals("8") || this.name.Equals("9"))
            maxHealth = health = 4;
        else
            maxHealth = health = 5;

        startMat = Resources.Load("Materials/Green") as Material;                                                                                       //refrence to start material, green in resource folder and end material, red
        endMat = Resources.Load("Materials/red") as Material;

        rend = this.GetComponent<Renderer>();
        rend.material = startMat;

        m_isRightAxisInUse = m_isLeftAxisInUse = m_isUpAxisInUse = m_isDownAxisInUse = false;                                                           //used to convert axis input into button inputs, see CursorCode script
    }

    void Update ()
    {
        if ((Input.GetButtonDown("START")||Input.GetKeyDown("l")) && this.name.Equals("1") && !isFocus && !isPlaced)                                    //iniates the start sequence based on these conditions
        {
            isFocus = true;
            Debug.Log("Start!");
            GameObject.Find("OVRPlayerController").GetComponent<Transform>().Translate(new Vector3(28, 8, 2), Space.World);                             //teleports playercamera to the boards
            GameObject.Find("OVRPlayerController").GetComponent<TeleCamera>().isGameStarted = true;                                                     //enabled camera teleport script to work
            GameObject.Find("HUDCanvas").GetComponent<Canvas>().enabled = true;                                                                         //eabled player HUD
            FindObjectOfType<AudioManager>().Stop("menu");                                                                                              //stop menu music
            FindObjectOfType<AudioManager>().Play("background");                                                                                        //play background music

        }
        rend.material.Lerp(endMat,startMat,(health / (float)maxHealth));                                                                                //change color of ship based on current health percentage
        if (isPlaced && !isCounted)             //This teleports and renders the next ship when the previous one is placed
        {
            isCounted = true;
            GameObject.Find("FoundationScriptBoard").GetComponent<FoundationCode>().count++;                                                            //Increments count in the board
            isFocus = false;                                                                                                                            //Takes focus off current ship

            if (getCount().Equals("11"))                                                                                                                //when count reaches 11, all boats are placed and player can begin firing on the enemy board, so let the player move the cursor by calling the gotFocus() function of the CursorCode script
            {
                GameObject.Find("PlayerCursor").SendMessage("gotFocus");
            }
            else
            {
                GameObject.Find(getCount()).GetComponent<Renderer>().enabled = true;                                                                        //Enables renderer or now next ship
                GameObject.Find(getCount()).GetComponent<Transform>().parent.Translate(origin - GameObject.Find(getCount()).GetComponent<Transform>().parent.position); //Brings now next ship to board
                GameObject.Find(getCount()).GetComponent<TeleScript>().isFocus = true;                                                                      //Puts focus on now next ship
            }
        }
        if (isFocus)                                                                                                                                        //allows input to be taken on the ship that has the isFocus variable set to true
        {
            //Keyboard Motion
            if (Input.GetKeyDown("w"))
            {
                lastKeyPressed = KeyCode.W;
                this.transform.parent.Translate(north, Space.World);
            }
            if (Input.GetKeyDown("s"))
            {
                lastKeyPressed = KeyCode.S;
                this.transform.parent.Translate(south, Space.World);
            }
            if (Input.GetKeyDown("a"))
            {
                lastKeyPressed = KeyCode.A;
                this.transform.parent.Translate(west, Space.World);
            }
            if (Input.GetKeyDown("d"))
            {
                lastKeyPressed = KeyCode.D;
                this.transform.parent.Translate(east, Space.World);
            }
            if (Input.GetKeyDown("q"))
            {
                lastKeyPressed = KeyCode.Q;
                this.transform.parent.Rotate(0, 90, 0);
            }
            if (Input.GetKeyDown("e"))
            {
                lastKeyPressed = KeyCode.E;
                this.transform.parent.Rotate(0, -90, 0);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.transform.parent.Translate(down, Space.World);
            }

            //Xbox Motion
            if (Input.GetAxisRaw("DpadRight") > 0)
            {
                if (m_isRightAxisInUse == false)
                {
                    lastKeyPressed = KeyCode.D;
                    this.transform.parent.Translate(east, Space.World);
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
                    this.transform.parent.Translate(west, Space.World);
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
                    this.transform.parent.Translate(north, Space.World);
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
                    this.transform.parent.Translate(south, Space.World);
                    m_isDownAxisInUse = true;
                }
            }
            if (Input.GetAxisRaw("DpadDown") == 0)
            {
                m_isDownAxisInUse = false;
            }

            if (Input.GetButtonDown("LB"))
            {
                lastKeyPressed = KeyCode.Q;
                this.transform.parent.Rotate(0, 90, 0);
            }
            if (Input.GetButtonDown("RB"))
            {
                lastKeyPressed = KeyCode.E;
                this.transform.parent.Rotate(0, -90, 0);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" && isFocus)                                                          //rebounds player if they collide into wall when rotating or moving based on the last key they pressed
        {
            if (lastKeyPressed.ToString().Equals("W"))
                this.transform.parent.Translate(south, Space.World);
            if (lastKeyPressed.ToString().Equals("S"))
                this.transform.parent.Translate(north, Space.World);
            if (lastKeyPressed.ToString().Equals("A"))
                this.transform.parent.Translate(east, Space.World);
            if (lastKeyPressed.ToString().Equals("D"))
                this.transform.parent.Translate(west, Space.World);
            if (lastKeyPressed.ToString().Equals("Q"))
                this.transform.parent.Rotate(0, -90, 0);
            if (lastKeyPressed.ToString().Equals("E"))
                this.transform.parent.Rotate(0, 90, 0);
        }
    }

    private void OnTriggerStay(Collider other)                                                                  //checks to see if a ship is currently within another ship, in that case it is not placable, otherwise it is
    {
        if (other.gameObject.tag == "Ship")
        {
            isPlacable = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ship")
        {
            isPlacable = true;
        }
    }

}
