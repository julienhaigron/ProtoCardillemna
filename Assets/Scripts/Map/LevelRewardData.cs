using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Scriptables/LevelReward")]
public class LevelRewardData : ScriptableObject
{
    public List<ActionCardData> cards;
}
