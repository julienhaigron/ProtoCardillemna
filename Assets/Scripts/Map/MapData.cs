using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/MapData")]
public class MapData : ScriptableObject
{
    public int width;
    public int height;
    public RoomType[] rooms; // Tableau à une dimension pour une grille 2D
    public List<EntityData> enemies;

    public LevelRewardData reward;

    public RoomType GetRoom ( int x, int y )
    {
        if (rooms == null || rooms.Length < y * width + x)
            return RoomType.Void;

        return rooms[y * width + x];
    }

    public void SetRoom ( int x, int y, RoomType roomType )
    {
        rooms[y * width + x] = roomType;
    }
}

public enum RoomType
{
    Void,
    Empty,
    Start,
    Exit,
    Treasure,
    Enemy
}
