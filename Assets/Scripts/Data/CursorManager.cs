using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

public static class CursorManager
{
    private static Dictionary<string, Texture2D> cursors;

    public static void Initialize()
    {
        cursors = new Dictionary<string, Texture2D>();

        List<Texture2D> list = DataManager.GetObjects<Texture2D>(DataManager.CursorPath);

        foreach (Texture2D cursor in list)
        {
            string name = cursor.name.ToUpper();
            cursors.Add(name, cursor);
        }
        Debug.Log("[SYSTEM] LOAD CURSOR DATA");
    }

    public static Texture2D GetCursor(string name)
    {
        name = name.ToUpper();

        if (!cursors.ContainsKey(name)) return null;
        return cursors[name];
    }
}
