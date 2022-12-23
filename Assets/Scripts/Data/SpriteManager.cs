using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

public static class SpriteManager
{
    private static Dictionary<string, Sprite> images;
    private static Dictionary<string, Sprite> icons;

    public static void Initialize()
    {
        images = new Dictionary<string, Sprite>();
        icons = new Dictionary<string, Sprite>();

        List<Sprite> list = DataManager.GetObjects<Sprite>(DataManager.ImagePath);

        foreach (Sprite sprite in list)
        {
            string name = sprite.name.ToUpper();
            images.Add(name, sprite);
        }
        Debug.Log("[SYSTEM] LOAD IMAGE DATA");
        list.Clear();

        list = DataManager.GetObjects<Sprite>(DataManager.IconPath);

        foreach (Sprite sprite in list)
        {
            string name = sprite.name.ToUpper();
            icons.Add(name, sprite);
        }
        Debug.Log("[SYSTEM] LOAD ICON DATA");
    }

    public static Sprite GetImage(string name)
    {
        name = name.ToUpper();

        if (!images.ContainsKey(name)) return null;
        return images[name];
    }

    public static Sprite GetIcon(string name)
    {
        name = name.ToUpper();

        if (!icons.ContainsKey(name)) return icons["NONE"];
        return icons[name];
    }
}
