using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interact
{
    public override void RunInteraction(Player player)
    {
        Debug.Log("Opening Door!");
    }
}
