using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitRoom : ARoom
{
    public override void OnEnter ( EntityCard entity )
    {
        if (entity.IsPlayer)
        {
            Debug.Log("Le joueur a atteint la sortie");
            // Tu pourrais déclencher ici un changement de niveau
        }
    }
}
