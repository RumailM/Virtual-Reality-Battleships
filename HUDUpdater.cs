using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;                                                //using imported assets for better text rendering
using UnityEngine.UI;

public class HUDUpdater : MonoBehaviour                     //placed on HUDCanvas
{
    void updateHUD()
    {
        for(int i = 2; i <= 5; i++)                         //loops through all the counters for all lengths of ships and tells them to update whenever a ship has sunk, this function is called whenever ship has sunk
        {
            GameObject.Find(i + "ComputerCount").GetComponent<TextMeshProUGUI>().text = GameObject.Find("ComputerScriptBoard").GetComponent<CompFoundationCode>().ShipsLeft[i - 2].ToString();
        }                                                   //updates each text element counter based on the according ShipsLeft arrays in CompFoundationCode and FoundationCode
        for (int i = 2; i <= 5; i++)
        {
            GameObject.Find(i + "PlayerCount").GetComponent<TextMeshProUGUI>().text = GameObject.Find("FoundationScriptBoard").GetComponent<FoundationCode>().ShipsLeft[i - 2].ToString();
        }
    }
}
