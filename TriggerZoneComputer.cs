using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinate                                                                                                             //Container class used to store two ints in one variable, useful for passing in arguments to a function in another script
{
    public int xpos, ypos;
    public Coordinate(int x, int y)
    {
        xpos = x;
        ypos = y;
    }
}

public class TriggerZoneComputer : MonoBehaviour
{

    public int xpos;                                                                                                                //Stores the xpos of the zone along the grid
    public int ypos;                                                                                                                //Stores the ypos of the zone along the grid
    public bool isFiredUpon;                                                                                                        //Stores whether the zone has been fired upon yet
    public bool isCursorInsideMe;                                                                                                   //Stores whther the cursor is currently highlighting the zone

    Material miss;                                                                                                                  //Stores the designated material for a missed shot
    Material hit;                                                                                                                   //Stores the designated material for a hit shot
    Material selected;                                                                                                              //Stores the designated material for a highlighted cursor
    Material temp;                                                                                                                  //Temporarily stores the previous material before the cursor enters the zone
    Renderer rend;                                                                                                                  //Used to initialize the Renderer

    bool getFocus()                                                                                                                 //Shortcut function to get the isFocus variable From PlayerCursor
    {
        return GameObject.Find("PlayerCursor").GetComponent<CursorCode>().isFocus;
    }

    // Use this for initialization
    void Start ()
    {
        miss = Resources.Load("Materials/Green_Miss") as Material;                                                                  //Initialize miss material
        hit = Resources.Load("Materials/Red_Hit") as Material;                                                                      //Initialize hit material
        selected = Resources.Load("Materials/Yellow_Cursor") as Material;                                                           //Initialize selected material
        isFiredUpon = false;
        isCursorInsideMe = false;

        rend = this.GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        /*
		if (Input.GetKeyDown("p"))                                                                                                  //Shows positions of enemy ships
        {                                                                                                                           //Useful during debugging
            if (GameObject.Find("ComputerScriptBoard").GetComponent<CompFoundationCode>().CompBoard[xpos, ypos] > 0)
            {
                if (GameObject.Find("ComputerScriptBoard").GetComponent<CompFoundationCode>().CompBoard[xpos, ypos] <= 4)
                    this.GetComponent<Renderer>().material.color = Color.cyan;
                else if (GameObject.Find("ComputerScriptBoard").GetComponent<CompFoundationCode>().CompBoard[xpos, ypos] <= 7)
                    this.GetComponent<Renderer>().material.color = Color.black;
                else if (GameObject.Find("ComputerScriptBoard").GetComponent<CompFoundationCode>().CompBoard[xpos, ypos] <= 9)
                    this.GetComponent<Renderer>().material.color = Color.green;
                else if (GameObject.Find("ComputerScriptBoard").GetComponent<CompFoundationCode>().CompBoard[xpos, ypos] == 10)
                    this.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }
        */
        if ((Input.GetKeyDown("space") || Input.GetButtonDown("A")) && !isFiredUpon && isCursorInsideMe && getFocus())              //Shoots if currently highlighted by PlayerCursor and not shot at before
        {
            isFiredUpon = true;
            Fire();
            FindObjectOfType<AudioManager>().Play("fired");
        }
    }

    void Fire()                                                                                                                     //Fires at designated position by PlayerCursor
    {
        if (GameObject.Find("ComputerScriptBoard").GetComponent<CompFoundationCode>().CompBoard[xpos, ypos] > 0)                    //Checks if unfired ship exists in zone
        {
            rend.material = temp = hit;
            FindObjectOfType<AudioManager>().Play("hit");
        }
        else
        {
            rend.material = temp = miss;
            FindObjectOfType<AudioManager>().Play("miss");
        }
        GameObject.Find("ComputerScriptBoard").SendMessage("PlayerFire", new Coordinate(xpos, ypos));
    }

    private void OnTriggerEnter(Collider other)                                                                                     //Changes material of zone highlighted by PlayerCursor when PLayerCursor enters the zone
    {
        if (!isFiredUpon)                                                                                                           //PlayerCursor is not specified as it is the only object meant to enter the zone
        {
            temp = rend.material;
        }
        rend.material = selected;
        isCursorInsideMe = true;
    }

    private void OnTriggerExit(Collider other)                                                                                      //Resets material to previous value after PlayerCursor exits the zone
    {
         rend.material = temp;
        isCursorInsideMe = false;
    }
}
