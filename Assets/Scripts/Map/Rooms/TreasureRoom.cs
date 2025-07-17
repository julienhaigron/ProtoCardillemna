using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoom : ARoom
{
    private bool opened = false;

    public override void OnEnter ( EntityCard entity )
    {
        if (!opened)
        {
            opened = true;
            Debug.Log("Tr�sor r�cup�r� !");
            // Logique de loot ici
        }
    }
}
