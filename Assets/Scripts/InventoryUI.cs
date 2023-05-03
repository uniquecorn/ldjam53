using System.Collections.Generic;
using System.Linq;
using Castle;
using Castle.Core.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryUI : CastlePopup
{
    public DraggableItemUI tempDraggable;
    public List<DraggableItemUI> draggablePool;
    public GridLayoutGroup gridLayoutGroup;
    public Transform itemsParent;
    public Slot slotPrefab;
    public List<Slot> slots;

    protected override void OpenActions()
    {
        base.OpenActions();
        LoadInventory(GameManager.instance.inventory);
    }

    public void LoadInventory(Inventory inventory)
    {
        inventory.ResetOccupiedGrids();
        gridLayoutGroup.constraintCount = inventory.width;
        if (draggablePool == null)
        {
            draggablePool = new List<DraggableItemUI>();
        }
        foreach (var draggable in draggablePool)
        {
            draggable.gameObject.SetActive(false);
        }
        SetSlotCount(inventory.width*inventory.height);
        if (inventory.items.IsSafe())
        {
            for (var i = 0; i < inventory.items.Count; i++)
            {
                var item = inventory.items[i].GetItem;
                var flipped = inventory.items[i].orientation is Orientation.Up or Orientation.Down;
                var width = (flipped ? item.height : item.width) * 110;
                var height = (flipped ? item.width : item.height) * 110;
                var offset = new Vector2(-(width - 110) / 2, -(height - 110) / 2) + Vector2.one * 10;
                var draggable = GetDraggable();
                draggable.Load(inventory.items[i]);
                draggable.rectTransform.position =
                    slots[inventory.PositionIndex(inventory.items[i].position)].transform.position - (Vector3)offset;
                
            }
        }
    }

    public void SetOccupied(Inventory inventory)
    {
        for (var i = 0; i < inventory.occupied.Length; i++)
        {
            slots[i].SetOccupied(inventory.occupied[i]);
            slots[i].transform.SetSiblingIndex(i);
        }
    }
    public void Hover(int gridIndex, Item item, Orientation orientation) =>
        Hover(GameManager.instance.inventory.Position(gridIndex), item, orientation);

    public void Hover(SGrid grid, Item item, Orientation orientation)
    {
        var flipped = orientation is Orientation.Up or Orientation.Down;
        var blanks = item.TransposedBlanks(orientation);
        for (var x = 0; x < (flipped ? item.height : item.width); x++)
        {
            for (var y = 0; y < (flipped ? item.width : item.height); y++)
            {
                var g = new SGrid(x, y);
                if (blanks.IsSafe() && blanks.Contains(g)) continue;
                g = grid.Shift(g);
                if(g.x >= GameManager.instance.inventory.width)continue;
                if(g.y >= GameManager.instance.inventory.height)continue;
                var index = g.x + (g.y * GameManager.instance.inventory.width);
                if(index >= slots.Count)continue;
                slots[index].image.color = GameManager.instance.inventory.occupied[index]? Color.red : Color.cyan;
            }
        }
    }
    public DraggableItemUI GetDraggable()
    {
        foreach (var draggable in draggablePool)
        {
            if(draggable.gameObject.activeSelf)continue;
            draggable.gameObject.SetActive(true);
            return draggable;
        }
        draggablePool.Add(Instantiate(tempDraggable,itemsParent));
        draggablePool[^1].gameObject.SetActive(true);
        return draggablePool[^1];
    }
    public void SetSlotCount(int num)
    {
        if (slots == null)
        {
            slots = new List<Slot>();
        }
        var numToAdd = num - slots.Count;
        for (var i = 0; i < numToAdd; i++)
        {
            slots.Add(Instantiate(slotPrefab,gridLayoutGroup.transform));
            slots[^1].gameObject.SetActive(true);
        }
    }
}