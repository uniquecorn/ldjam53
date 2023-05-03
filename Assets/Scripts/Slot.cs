using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int slotID;
    public Image image;
    public Color color;
    public Sprite occupied, unoccupied;
    public InventoryUI inventoryUI;
    public void SetOccupied(bool value)
    {
        image.sprite = value ? occupied : unoccupied;
    }
    private void Update()
    {
        image.color = Color.Lerp(image.color, color, Time.deltaTime * 5);
    }
}
