using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class IntelligentHit                                                                                                     //Container class for a previously successful shot fired
{                                                                                                                               //Used to determine the next intelligent shot to be fired by the AI
    public int x, y, n, e, s, w;                                                                                                //x & y denote position on the grid, n, e, s, and w refer to the four cardinal directions
    public IntelligentHit(int xpos, int ypos)                                                                                   //n, e, s, w == 0 represents as invalid direction to fire in,
    {                                                                                                                           // == 1 means first space is next to be fired at and valid
        x = xpos;                                                                                                               // == 2 means second space is next to be fired at and valid.... etc
        y = ypos;
        n = e = s = w = 1;                                                                                                      //Initially all cardinal directions are assumed to be open for fire

        if (x == 0)                                                                                                             //Edge conditions to prevent AI from firing outside of the array
            w = 0;
        else if (x == 9)
            e = 0;

        if (y == 0)
            n = 0;
        else if (y == 9)
            s = 0;
    }

    public override string ToString()                                                                                           //Overrided function used for debugging purposes
    {
        return x + " " + y + " " + n + " " + e + " " + s + " " + w;
    }
}

public class FoundationCode : MonoBehaviour
{
    public int [,] PlayerBoard = new int[10, 10];                                                                               //Memory array for where player has placed ships the status of hits and misses on the playerboard after the computer has played each turn, see CompFoundationCode
    int x, y, ship;
    public int count;
    public int[] ShipsLeft = { 4, 3, 2, 1 };                                                                                    //Number of 2s, 3s, 4s, 5s respectively
    System.Random rnd = new System.Random();

    public Collider shipCollider;
    GameObject ColliderBoard;

    void doQuit()                                                                                                               //Exits the game
    {
        Application.Quit();
    }

    public void Place(Container packet)                                                                                         //Places a Player's ship at specified position
    {
        PlayerBoard[packet.xpos, packet.ypos] = packet.zpos;       
        shipCollider = GameObject.Find((packet.zpos).ToString()).GetComponent<Collider>();
        shipCollider.isTrigger = true;        
    }

    //AI Code Begin

    List<IntelligentHit> ImpHits = new List<IntelligentHit>();                                                                  //A List of all previous successful shots, stored as IntelligentHits
                                                                                                                                //An IntelligentHit remains in the list until the hit ship has been sunk

    int getlength(int ship)                                                                                                     //Gets the length of a ship given its number (1-10)
    {
        if (ship <= 0)
        {
            return 0;
        }
        else if (ship <= 4)
            return 2;
        else if (ship <= 7)
            return 3;
        else if (ship <= 9)
            return 4;
        else
            return 5;
    }

    void EdgeCheck()                                                                                                            //Determines whether an IntelligentHit is on the edge of the board
    {                                                                                                                           //Sets the values of invalid cardinal directions to zero if on the edge of board
        if (ImpHits[0].x - ImpHits[0].w == -1)
            ImpHits[0].w = 0;
        else if (ImpHits[0].x + ImpHits[0].e == 10)
            ImpHits[0].e = 0;

        if (ImpHits[0].y - ImpHits[0].n == -1)
            ImpHits[0].n = 0;
        else if (ImpHits[0].y + ImpHits[0].s == 10)
            ImpHits[0].s = 0;
    }

    void Fire(int x, int y, int c)                                                                                              //Fires at the specified position (x,y) in a cardinal direction c, such that
    {                                                                                                                           //c == 0: n, c == 1: e, c == 2: s, c == 3: w
        if (PlayerBoard[x, y] > 0)                                                                                              //If the fired position resulted in a hit
        {
            GameObject.Find(PlayerBoard[x, y].ToString()).GetComponent<TeleScript>().health--;                                  //Decrements the health of the hit ship             Used for adjusting material of hit ship
            PlayerBoard[x, y] *= -1;                                                                                            //Sets position fired upon as a hit

            ImpHits.Add(new IntelligentHit(x, y));                                                                              //Add a new successful hit to the List of IntelligentHits

            //Logic:
            //Adjusting the cardinal direction of the next Intelligent Hit
            //A ship exists as a straight line in one dimension. If a shot was fired to the east or west and hit, then there is no logical reason to fire north or south in attempting to sink the ship, and vice versa.
            //The if conditions are to determine which cardinal direction the shot was directed towards
            
            if (c == 0)                                                                                                         //For the current IntelligentHit, determine the appropopriate direction of the subsequent hit
            {                                                                                                                   
                ImpHits[0].e = ImpHits[0].w = 0;
                ImpHits[0].n++;
            }
            else if (c == 1)
            {
                ImpHits[0].n = ImpHits[0].s = 0;
                ImpHits[0].e++;
            }
            else if (c == 2)
            {
                ImpHits[0].e = ImpHits[0].w = 0;
                ImpHits[0].s++;
            }
            else if (c == 3)
            {
                ImpHits[0].n = ImpHits[0].s = 0;
                ImpHits[0].w++;
            }

            if (CheckSink(-1 * PlayerBoard[x, y]))                                                                              //After each successful hit, check if a ship has been sunk, If yes:
            {
                Debug.Log("Player's ship #" + (-1 * PlayerBoard[x, y]) + " sank");
                Sink(-1 * PlayerBoard[x, y]);                                                                                   //Sink the ship

                int length = System.Math.Abs(PlayerBoard[x, y]);
                for (int i = ImpHits.Count - 1; i >= 0 && length > 0; i--)                                                      //Removes all IntelligentHits associated with the sunk ship
                {                                                                                                               //It is counterintuitive to attempt to predict the subsequent shot against a ship that has already been sunk
                    if (PlayerBoard[ImpHits[i].x, ImpHits[i].y] == PlayerBoard[x, y])
                    {
                        ImpHits.RemoveAt(i);
                        length--;
                    }
                }
            }                                                                                                                   //By now all IntelligentHits relating to the sunk ship have been removed


            if (ImpHits.Count != 0)                                                                                             //If any IntelligentHits remain:
                EdgeCheck();                                                                                                    //Ensure the next IntelligentHit is not an edge case

            ColliderBoard.BroadcastMessage("FiredUpon");                                                                        //Used to update the material of the hit position
        }
        else                                                                                                                    //If the fired position resulted in a miss
        {
            PlayerBoard[x, y] = -99;                                                                                            //Our chosen sentinel value to represent a miss in the array (-99)

            //Logic:
            //Ships exist as straight lines in one dimension within our array
            //If the AI attempted to fire in a specific direction based off an IntelligentHit, and it was a miss, then there is no logical reason to fire again in the same direction from the same IntelligentHit in attempting to sink the ship.
            //The if conditions are to determine which cardinal direction the shot was directed towards

            if (c == 0)
            {
                ImpHits[0].n = 0;
            }
            else if (c == 1)
            {
                ImpHits[0].e = 0;
            }
            else if (c == 2)
            {
                ImpHits[0].s = 0;
            }
            else if (c == 3)
            {
                ImpHits[0].w = 0;
            }

            ColliderBoard.BroadcastMessage("FiredUpon");                                                                        //Used to update the material of the missed position
        }
    }

    void RandomFire()                                                                                                           //Used to fire at a random position on the board that has not been fired upon yet
    {                                                                                                                           //Only called when no IntelligentHits exist on record
                                                                                                                                //(No previous successful hits, nothing to base next hit on)
        do                                                                                                                      //Generating random positions within the bounds of our array
        {
            x = rnd.Next(0, 10);
            y = rnd.Next(0, 10);
        } while (PlayerBoard[x, y] < 0);                                                                                        //Checks that the random position has not been fired upon yet
        if (PlayerBoard[x, y] > 0)                                                                                              //If a hit
        {
            GameObject.Find(PlayerBoard[x, y].ToString()).GetComponent<TeleScript>().health--;                                  //Decrement the health of the hit ship              Used for color
            PlayerBoard[x, y] *= -1;                                                                                            //Update the array storing the ships
            if (CheckSink(-1 * PlayerBoard[x, y]))                                                                              //Check if the affected ship was sunk
                Sink(-1 * PlayerBoard[x, y]);

            ImpHits.Add(new IntelligentHit(x, y));                                                                              //Add the successful hit to IntelligentHits for further targeting against hit ship
            ColliderBoard.BroadcastMessage("FiredUpon");                                                                        //Used to update the material of the hit position
        }
        else
        {
            PlayerBoard[x, y] = -99;                                                                                            //Update the array storing the ships to a miss
            ColliderBoard.BroadcastMessage("FiredUpon");                                                                        //Used to update the material of the missed position
        }
    }


    void AutoFire()                                                                                                             //The Master Fire Function called at the beginning of each of the AI's turns
    {     
        if (ImpHits.Count == 0)                                                                                                 //Shoots randomly if no previous IntelligentHits exist on record
        {
            RandomFire();
        }
        else
        {
            if (ImpHits[0].n == 0 && ImpHits[0].e == 0 && ImpHits[0].s == 0 && ImpHits[0].w == 0)                               //Resets the cardinal directions if an IntelligentHit has no direction to shoot in
            {
                ImpHits[0].n = ImpHits[0].s = ImpHits[0].e = ImpHits[0].w = 1;

                EdgeCheck();                                                                                                    //Calls EdgeCheck to ensure the IntelligentHit is not an edge case
            }
            
            //Chooses the next cardinal direction to fire towards from the IntelligentHit record
            //Cycles through the four directions: North, East, South, West
            //Checks if the IntelligentHit record allows for the chosen cardinal direction
            //Ensures that the chosen direction has not been fired upon already

            if (ImpHits[0].n != 0 && PlayerBoard[ImpHits[0].x, ImpHits[0].y - ImpHits[0].n] >= 0)
                Fire(ImpHits[0].x, ImpHits[0].y - ImpHits[0].n, 0);
            else if (ImpHits[0].e != 0 && PlayerBoard[ImpHits[0].x + ImpHits[0].e, ImpHits[0].y] >= 0)
                Fire(ImpHits[0].x + ImpHits[0].e, ImpHits[0].y, 1);
            else if (ImpHits[0].s != 0 && PlayerBoard[ImpHits[0].x, ImpHits[0].y + ImpHits[0].s] >= 0)
                Fire(ImpHits[0].x, ImpHits[0].y + ImpHits[0].s, 2);
            else if (ImpHits[0].w != 0 && PlayerBoard[ImpHits[0].x - ImpHits[0].w, ImpHits[0].y] >= 0)
                Fire(ImpHits[0].x - ImpHits[0].w, ImpHits[0].y, 3);
            else                                                                                                                //If all cardinal directions are invalid, clear the List of IntelligentHits, and fire randomly
            {
                ImpHits.Clear();
                RandomFire();
            }
        }
    }


    //AI CODE END

    private void Sink(int ship)                             //sinks the inputted ship. Calls required actions when a ship is sunk. This include decrememnting ships left array so that player how many of each kind of ship is left. It plays a sinking audio track as well. It tells the HUDCanvas on the player camera to update its values because they have changed.
    {
        if (ship <= 4)
            ShipsLeft[0]--;
        else if (ship <= 7)
            ShipsLeft[1]--;
        else if (ship <= 9)
            ShipsLeft[2]--;
        else
            ShipsLeft[3]--;

        GameObject.Find("HUDCanvas").SendMessage("updateHUD");

        if (CheckWin())                                     //checks to see if a player has won and if so call the win function.
            Win();
    }
    private bool CheckSink(int ship)                        //input is the ship tag and it scans through the 2d array to find any other cell of that tag, if so it returns false because the ship did not sink, if it false to find another it returns true
    {
        bool sunk = true;
        for (int col = 0; col < 10; col++)
        {
            for (int row = 0; row < 10; row++)
            {
                if (PlayerBoard[col, row] == ship)
                {
                    sunk = false;
                    goto OUT;
                }
            }
        }
        OUT:
        return sunk;
    }
    bool CheckWin()                                         //checks if the player one by sifting through the computerboard 2d array and seeing if there are any positive numbers left, if there are none, the player has sunken all ships and has won
    {
        bool win = true;
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                if (PlayerBoard[i, j] > 0)
                    win = false;
        return win;
    }
                                                                                //runs through the win sequence if the computer wins and the player loses
    void Win()
    {
        FindObjectOfType<AudioManager>().Stop("sink");                          //disable sink sound so that "game over" can be heard in full glory
        Debug.Log("Computer Won!");                     
        FindObjectOfType<CursorCode>().isFocus = false;                         //disbales player input on the cursor
        GameObject.Find("HUDCanvas").GetComponent<Canvas>().enabled = false;    //disables in game HUD Canvas
        GameObject.Find("Defeat").GetComponent<Canvas>().enabled = true;        //enables defeat HUD canvas showing defeat screen
        FindObjectOfType<AudioManager>().Stop("background");                    //stop background music
        FindObjectOfType<AudioManager>().Play("defeat");                        //play defeat sound


                                                                                //the rest of the win function generates a random vector3 under certain circumstances that cause the players boats to fly out of the water when the game is over, for comedic effect
        float magnitude = 100;
        Vector3 explode;

        for (int i = 1; i <= 10; i++)
        {
            GameObject.Find(i.ToString()).GetComponent<Collider>().isTrigger = false;
            explode = new Vector3((float)(rnd.NextDouble() * 2 - 1), (float)rnd.NextDouble()*10, (float)(rnd.NextDouble() * 2 - 1));
            //magnitude = (float)rnd.NextDouble() * 9 + 1;
            GameObject.Find(i.ToString()).GetComponent<Rigidbody>().AddForce(magnitude * explode);
            GameObject.Find(i.ToString()).GetComponent<Rigidbody>().AddTorque(transform.up * 150f);
            GameObject.Find(i.ToString()).GetComponent<Rigidbody>().AddTorque(transform.right * 150f);
            GameObject.Find(i.ToString()).GetComponent<Rigidbody>().AddTorque(transform.forward * 150f);
        }
        Invoke("doQuit", 15);                                                   //quits the game after 15 seconds


    }


    /*bool ScanForShipsY(int x, int y, int length)
    {
        for (int i = 0; i < length; i++)
        {
            if (PlayerBoard[x, y + i] != 0)
                return true;
        }
        return false;
    }

    bool ScanForShipsX(int x, int y, int length)
    {
        for (int i = 0; i < length; i++)
        {
            if (PlayerBoard[x + i, y] != 0)
                return true;
        }
        return false;
    }*/

    
    void Start ()
    {
        count = 1;
        ColliderBoard = GameObject.Find("ColliderBoard");
    }
	
	
}
