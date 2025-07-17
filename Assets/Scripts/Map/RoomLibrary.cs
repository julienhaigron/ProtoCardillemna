using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map/RoomLibrary")]
public class RoomLibrary : ScriptableObject
{
    [System.Serializable]
    public struct RoomPrefabEntry
    {
        public RoomType roomType;
        public GameObject prefab;
    }

    public RoomPrefabEntry[] roomPrefabs;

    private Dictionary<RoomType, GameObject> prefabDict;

    public GameObject GetRoomPrefab ( RoomType type )
    {
        if (prefabDict == null)
        {
            prefabDict = new Dictionary<RoomType, GameObject>();
            foreach (var entry in roomPrefabs)
            {
                if (!prefabDict.ContainsKey(entry.roomType))
                {
                    prefabDict.Add(entry.roomType, entry.prefab);
                }
            }
        }

        prefabDict.TryGetValue(type, out var prefab);
        return prefab;
    }
}