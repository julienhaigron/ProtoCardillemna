using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Entity")]
public class EntityData : ScriptableObject
{
    public bool isPlayer = false;

    public List<ActionCardData> deck;
    public int attackActionPoint = 1;
    public int movementActionPoint = 2;
    public int specialActionPoint = 1;

    public int hp = 2;
}
