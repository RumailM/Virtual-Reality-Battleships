using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container
{
    public int xpos, ypos, zpos;                                                                                       //container class used so one variable can be passed into the .SendMessage Command with multiple properties
    public Container(int x, int y, int z)                                                                               
    {
        xpos = x;
        ypos = y;
        zpos = z;
    }
}

public class TriggerZone : MonoBehaviour
{
    public int xpos;                                                                                                   //
    public int ypos;
    public int shipInsideMe;                                                                                           //stores the tag of the ship currently within the triggerzone gameobject by getiing its name, i.e. 1-10, and casting that to an int 
    public bool shipPlacedInMe;                                                                                        //stores the state of if there is a shipped placed inside

    Renderer rend;
    Material miss;
    Material hit;

    void FiredUpon()                                                                                                    //when a playerboard cell or zone is hit, check to see if it was a miss, if so make it red, otherwise make it green
    {
        if (GameObject.Find("FoundationScriptBoard").GetComponent<FoundationCode>().PlayerBoard[xpos, ypos] == -99)
        {
            this.GetComponent<Renderer>().material = miss;
        }
        else if (GameObject.Find("FoundationScriptBoard").GetComponent<FoundationCode>().PlayerBoard[xpos, ypos] < 0)
        {
            this.GetComponent<Renderer>().material = hit;
        }
    }

    void Start()
    {
        miss = Resources.Load("Materials/Green_Miss") as Material;                                                      //misses should be marked with red
        hit = Resources.Load("Materials/Red_Hit") as Material;                                                          //hits should be marked with green
        shipPlacedInMe = false;                             
    }

    private void OnTriggerEnter(Collider other)                                                                         //gets the name of the ship that is currently within the triggerzone
    {
        //rend.material.SetColor ("_Color", Color.red);

        shipInsideMe = Int32.Parse(other.gameObject.name);

    }

	private void OnTriggerExit(Collider other)                                                                          //resets the shipinsideme to 0 if a ship leaves the triggerzone
    {
        //rend.material.SetColor ("_Color", Color.white);

        shipInsideMe = 0;

    }

    private void Update()
    {
        if ((Input.GetKeyDown("space") || Input.GetButtonDown("A")) && shipInsideMe != 0 && shipPlacedInMe == false && GameObject.Find(shipInsideMe.ToString()).GetComponent<TeleScript>().isPlacable == true) //when the player tries to place, and if there isn't already a ship placed in the triggerzone, and there currently is an unplaced ship in the zone, and the ship is placebale, do these commands
        {
            GameObject.Find("FoundationScriptBoard").SendMessage("Place", new Container(xpos, ypos, shipInsideMe));                                     //place the ship, send a message to the FoundatioNScriptBoard and tell it to place the ship within the memory array
            shipPlacedInMe = true;                                                                                                                      //since a valid placement has occured, block this triggerzone out from being able to have more ships placed within it by saying shipPlacedInMe is true

            GameObject.Find(shipInsideMe.ToString()).GetComponent<TeleScript>().isPlaced = true;                                                        //updates ship state from isPlaced is false to is true

        }
    }
}
