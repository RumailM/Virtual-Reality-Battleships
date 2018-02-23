using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompFoundationCode : MonoBehaviour
{                                                                             //Memory array for the placement of the computer ships, and where the player has hit the ship, or missed a ship. Ships are tagged from 1 to 10, where 1-4 are 2 length, 5-7 are 3 length, 8-9 are 4 length, and 10 is 5 length
    public int[,] CompBoard = new int[10, 10];                                //A hit is stored as negative of the ship tag, so a hit on 1st ship is stored as -1, a miss is a -99, an empty cell that has not been hit is a 0, and the cells where ships are placed are marked with the tag, the first 2 length ship has 2 cells with a '1' in it
    public int x, y;
    public int[] ShipsLeft = { 4, 3, 2, 1 };                                  //Number of 2s, 3s, 4s, 5s respectively
    System.Random rnd = new System.Random();                                  //allows generation of random numbers
    int rand;



    void doQuit()                                                             //quits the game when called
    {
        Application.Quit();
    }

    void PlayerFire(Coordinate coord)                                         //is called when the player fires. The TriggerZoneComputerScript listens for the correct conditions for when a player fire, if the firing position is valid, it calls PlayerFire and checks to see where the player fired and update the array accordingly based on the key above
    {
        x = coord.xpos;
        y = coord.ypos;
        if (CompBoard[x, y] > 0)
        {
            CompBoard[x, y] *= -1;
            if (CheckSink(-1 * CompBoard[x, y]))                              //calls the CheckSink function which checks to see if the boat that a player fired has been completely destroyed, i.e. all its remaining shiptags in the array have become negative
                Sink(-1 * CompBoard[x, y]);                                     //sinks the ship if it has sunk
        }
        else
            CompBoard[x, y] = -99;
        GameObject.Find("FoundationScriptBoard").SendMessage("AutoFire");     //player turn is complete so call the computer to play its turn based on its AI.
    }

    private void Sink(int ship)                                               //sinks the inputted ship. Calls required actions when a ship is sunk. This include decrememnting ships left array so that player how many of each kind of ship is left. It plays a sinking audio track as well. It tells the HUDCanvas on the player camera to update its values because they have changed.
    {
        Debug.Log("Player sank ship #" + ship);

        if (ship <= 4)
            ShipsLeft[0]--;
        else if (ship <= 7)
            ShipsLeft[1]--;
        else if (ship <= 9)
            ShipsLeft[2]--;
        else
            ShipsLeft[3]--;
        GameObject.Find("HUDCanvas").SendMessage("updateHUD");
        FindObjectOfType<AudioManager>().Play("sink");

        string sl = "";
        for (int i = 0; i < 4; i++)
        {
            sl = sl + " " + ShipsLeft[i];
        }
        Debug.Log("Ships Left: " + sl);
        if (CheckWin())                                                       //checks to see if a player has won and if so call the win function.
            Win();
    }
    private bool CheckSink(int ship)                                         //input is the ship tag and it scans through the 2d array to find any other cell of that tag, if so it returns false because the ship did not sink, if it false to find another it returns true
    {
        bool sunk = true;
        for (int col = 0; col < 10; col++)
        {
            for (int row = 0; row < 10; row++)
            {
                if (CompBoard[col, row] == ship)
                {
                    sunk = false;
                    goto OUT;
                }
            }
        }
    OUT:
        return sunk;
    }
    bool CheckWin()                                                          //checks if the player one by sifting through the computerboard 2d array and seeing if there are any positive numbers left, if there are none, the player has sunken all ships and has won
    {
        bool win = true;
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                if (CompBoard[i, j] > 0)
                    win = false;
        return win;
    }

    void Win()                                                                //this calls the sequence of things that need to occur when a player has won
    {
        FindObjectOfType<AudioManager>().Stop("sink");                        //disables sinking sound so victory sound sounds clearer
        Debug.Log("Player Won!");
        FindObjectOfType<CursorCode>().isFocus = false;                       //disable the cursor from accepting input  
        GameObject.Find("HUDCanvas").GetComponent<Canvas>().enabled = false;  //disables ingame HUDCanvas 
        GameObject.Find("Victory").GetComponent<Canvas>().enabled = true;     //enables a victory canvas over player camera showing victory message
        FindObjectOfType<AudioManager>().Stop("background");                  //stops background music
        FindObjectOfType<AudioManager>().Play("win");                         //plays victory music


        Vector3 explode;                                                      //the rest of the win function generates a random vector3 under certain circumstances that cause the players boats to fly out of the water when the game is over, for comedic effect
        float magnitude = 100;

        for (int i = 1; i <= 10; i++)
        {
            GameObject.Find(i.ToString()).GetComponent<Collider>().isTrigger = false;
            explode = new Vector3((float)(rnd.NextDouble() * 2 - 1), (float)rnd.NextDouble() * 10, (float)(rnd.NextDouble() * 2 - 1));
            //magnitude = (float)rnd.NextDouble() * 9 + 1;
            GameObject.Find(i.ToString()).GetComponent<Rigidbody>().AddForce(magnitude * explode);
            GameObject.Find(i.ToString()).GetComponent<Rigidbody>().AddTorque(transform.up * 150f);
            GameObject.Find(i.ToString()).GetComponent<Rigidbody>().AddTorque(transform.right * 150f);
            GameObject.Find(i.ToString()).GetComponent<Rigidbody>().AddTorque(transform.forward * 150f);
        }
        Invoke("doQuit", 15);                                                 //quits the game after 15 seconds
    }

    bool ScanForShipsY(int x, int y, int length)                              //scanning functions are created so that when randomly placing ships computer can find out if their placement is valid, if not it will try to place elsewhere
    {
        for (int i = 0; i < length; i++)
        {
            if (CompBoard[x, y + i] != 0)
                return true;
        }
        return false;
    }

    bool ScanForShipsX(int x, int y, int length)
    {
        for (int i = 0; i < length; i++)
        {
            if (CompBoard[x + i, y] != 0)
                return true;
        }
        return false;
    }

    void CompPlace()                                                           //this function places the computer ships in the board
    {
        int length;                                                            //loop from ship 1 to 10 
        for (int ship = 1; ship <= 10; ship++)                                 //decoder so computer knows the length of a ship
        {
            if (ship <= 4)
                length = 2;
            else if (ship <= 7)
                length = 3;
            else if (ship <= 9)
                length = 4;
            else
                length = 5;
            rand = rnd.Next(0, 2);                                             //random number between 0 and 1 to determine orientation
                                                                               //checks the corresponding direction based on orientation for any ships already in the place in which the ship is trying to be placed, if so loop until valid
            if (rand == 0)
            {
                do
                {
                    x = rnd.Next(0, 10);
                    y = rnd.Next(0, 11 - length);
                } while (ScanForShipsY(x, y, length));

                for (int i = 0; i < length; i++)
                {
                    CompBoard[x, y + i] = ship;
                }

            }
            else
            {
                do
                {
                    x = rnd.Next(0, 11 - length);
                    y = rnd.Next(0, 10);
                } while (ScanForShipsX(x, y, length));

                for (int i = 0; i < length; i++)
                {
                    CompBoard[x + i, y] = ship;
                }

            }
        }
    }


    void Start()
    {
        CompPlace();                                                            //tells the computer to place its ships
    }
}
	
