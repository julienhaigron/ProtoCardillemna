using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapEditorTool : EditorWindow
{
    private MapData currentMap;
    private RoomType selectedRoomType = RoomType.Empty;

    [MenuItem("Tools/Map Editor")]
    public static void ShowWindow ()
    {
        GetWindow<MapEditorTool>("Map Editor");
    }

    private void OnGUI ()
    {
        currentMap = (MapData)EditorGUILayout.ObjectField("Map Data", currentMap, typeof(MapData), false);

        if (currentMap != null)
        {
            currentMap.width = EditorGUILayout.IntField("Width", currentMap.width);
            currentMap.height = EditorGUILayout.IntField("Height", currentMap.height);

            if (GUILayout.Button("Generate New Map"))
            {
                currentMap.rooms = new RoomType[currentMap.width * currentMap.height];
            }

            GUILayout.Space(10);
            selectedRoomType = (RoomType)EditorGUILayout.EnumPopup("Selected Room Type", selectedRoomType);

            if (currentMap.rooms != null)
            {
                for (int y = 0; y < currentMap.height; y++)
                {
                    GUILayout.BeginHorizontal();
                    for (int x = 0; x < currentMap.width; x++)
                    {
                        RoomType current = currentMap.GetRoom(x, y);
                        GUI.color = GetColorForRoom(current);
                        if (GUILayout.Button(current.ToString().Substring(0, 1), GUILayout.Width(30), GUILayout.Height(30)))
                        {
                            currentMap.SetRoom(x, y, selectedRoomType);
                            EditorUtility.SetDirty(currentMap);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }

    private Color GetColorForRoom ( RoomType roomType )
    {
        return roomType switch
        {
            RoomType.Void => Color.black,
            RoomType.Empty => Color.gray,
            RoomType.Start => Color.green,
            RoomType.Exit => Color.red,
            RoomType.Treasure => Color.yellow,
            RoomType.Enemy => Color.magenta,
            _ => Color.white,
        };
    }
}
