using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoom : ARoom
{
    private bool spawned = false;

    public override void OnEnter ( EntityCard entity )
    {
        if (!spawned)
        {
            spawned = true;
            Debug.Log("Des ennemis apparaissent !");
            // Logique de spawn d’ennemis ici
        }
    }
}
