using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggedItem : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetIcon(string name)
    {
        image.sprite = SpriteManager.GetIcon(name);
    }
}
