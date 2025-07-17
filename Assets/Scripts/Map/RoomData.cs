using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : ScriptableObject
{
    public enum RoomType { Empty, Exit, PlayerSpawn, EnemySpawn, Treasure}
    public RoomType type = RoomType.Empty;


}
