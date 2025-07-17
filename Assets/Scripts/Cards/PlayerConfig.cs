using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    public EntityData playerData;

    public int equipmentSlot = 1;
}
