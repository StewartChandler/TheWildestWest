using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;
    public int[] itemsThrown = new int[4];
    public int[] hatsLost = new int[4];
    public int[] hatsPickedUp = new int[4];
    public int[] timesFallen = new int[4];

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    public void ItemThrown(int playerIndex)
    {
        itemsThrown[playerIndex]++;
    }
    public void HatLost(int playerIndex)
    {
        hatsLost[playerIndex]++;
    }
    public void HatPickedUp(int playerIndex)
    {
        hatsPickedUp[playerIndex]++;
    }
    public void TimesFallen(int playerIndex)
    {
        timesFallen[playerIndex]++;
    }

    public void PrintStats()
    {
        for (int i = 0; i < 4; i++)
        {
            Debug.Log("Player " + i + " has thrown " + itemsThrown[i] + " items.");
            Debug.Log("Player " + i + " has lost " + hatsLost[i] + " hats.");
            Debug.Log("Player " + i + " has picked up " + hatsPickedUp[i] + " hats.");
            Debug.Log("Player " + i + " has fallen " + timesFallen[i] + " times.");
        }
    }

}