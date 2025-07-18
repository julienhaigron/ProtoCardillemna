using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] private RoomLibrary roomLibrary;
    [SerializeField] private Transform m_roomParent;
    public Transform RoomParent => m_roomParent;

    private MapData m_currentMapData;
    public MapData CurrentMapData => m_currentMapData;
    private ARoom[,] grid;

    [Button]
    public void GenerateMap ( MapData _currentMapData )
    {
        if (_currentMapData == null || roomLibrary == null)
        {
            Debug.LogError("MapData ou RoomLibrary non assigné !");
            return;
        }

        m_currentMapData = _currentMapData;
        grid = new ARoom[_currentMapData.width, _currentMapData.height];

        int spawnedEntityCount = 0;
        for (int y = 0; y < _currentMapData.height; y++)
        {
            for (int x = 0; x < _currentMapData.width; x++)
            {
                RoomType roomType = _currentMapData.GetRoom(x, y);
                if (roomType == RoomType.Void)
                    continue;

                GameObject prefab = roomLibrary.GetRoomPrefab(roomType);
                if (prefab == null)
                {
                    Debug.LogWarning($"Pas de prefab trouvé pour le type {roomType}");
                    continue;
                }

                Vector3 position = new Vector3(x, y, 0);
                GameObject roomObj = Instantiate(prefab, position, Quaternion.identity, m_roomParent);
                ARoom room = roomObj.GetComponent<ARoom>();
                if (room != null)
                {
                    room.Init(x, y, roomType);
                    grid[x, y] = room;
                }
                else
                {
                    Debug.LogWarning($"Le prefab {prefab.name} ne contient pas de ARoom.");
                }
                if (roomType == RoomType.Enemy && _currentMapData.enemies.Count > spawnedEntityCount)
                    room.Spawn(_currentMapData.enemies[spawnedEntityCount++]);
                else if (roomType == RoomType.Start)
				{
                    if (GameManager.Instance.TurnManager.Player != null)
                        GameManager.Instance.TurnManager.Player.MoveTo(room);
                    else
                        room.Spawn(GameManager.Instance.PlayerConfig.playerData);
				}
            }
        }
    }

    public void DiscardMap ()
	{
        for(int y = m_currentMapData.height; y < 0; y--)
		{
            for(int x = m_currentMapData.width; x < 0; x--)
			{
                Destroy(grid[x, y].gameObject);
            }
		}

    }

    public ARoom GetRoom(int _x, int _y )
    {
        if (_x < 0 || _y < 0)
            return null;

        if (grid == null || grid.Length < _y * m_currentMapData.width + _x)
            return null;

        return grid[_x, _y];
    }

    public bool GetRoom(int _x, int _y, out ARoom _room )
	{
        _room = GetRoom(_x, _y);
        return roomLibrary != null;
    }

    public int GetDistance(ARoom _from, ARoom _to )
	{
        int dx = Mathf.Abs(_from.x - _to.x);
        int dy = Mathf.Abs(_from.y - _to.y);
        return dx + dy;
    }
}
