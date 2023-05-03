using System.Collections.Generic;
using System.Linq;
using Castle;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public int width, height;
    public bool[] occupied;
    public List<SlotItem> items;

    public bool CanFit(SGrid position, Item item)
    {
        for (var x = 0; x < item.width; x++)
        {
            for (var y = 0; y < item.height; y++)
            {
                var g = position.Shift(x, y);
                if (Occupied(position.Shift(x, y)))
                {
                    return true;
                }
            }
        }

        return false;   
    }

    public bool Occupied(SGrid grid)
    {
        if(grid.x >= width)return true;
        if(grid.y >= height)return true;
        var index = grid.x + (grid.y * width);
        if (occupied.Length <= index)
        {
            return true;
        }
        return occupied[index];
    }

    public void SetOccupied(SGrid grid)
    {
        var index = grid.x + (grid.y * width);
        occupied[index] = true;
    }
    public void SetOccupied(SGrid grid,Item item, Orientation orientation)
    {
        var flipped = orientation is Orientation.Up or Orientation.Down;
        var blanks = item.TransposedBlanks(orientation);
        for (var x = 0; x < (flipped ? item.height : item.width); x++)
        {
            for (var y = 0; y < (flipped ? item.width : item.height); y++)
            {
                var g = new SGrid(x, y);
                if (blanks.IsSafe() && blanks.Contains(g)) continue;
                SetOccupied(grid.Shift(g));
            }
        }
    }
    [Button]
    public void ResetOccupiedGrids()
    {
        occupied = new bool[width * height];
        if (items.IsSafe())
        {
            for (var i = 0; i < items.Count; i++)
            {
                SetOccupied(items[i].position,items[i].GetItem,items[i].orientation);
            }
        }
    }
    public bool CanInsert(int gridIndex, Item item, Orientation orientation) => CanInsert(Position(gridIndex), item, orientation);
    public bool CanInsert(SGrid grid, Item item, Orientation orientation)
    {
        var flipped = orientation is Orientation.Up or Orientation.Down;
        var blanks = item.TransposedBlanks(orientation);
        for (var x = 0; x < (flipped ? item.height : item.width); x++)
        {
            for (var y = 0; y < (flipped ? item.width : item.height); y++)
            {
                var g = new SGrid(x, y);
                if (blanks.IsSafe() && blanks.Contains(g)) continue;
                if (Occupied(grid.Shift(g)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public SGrid Position(int gridIndex) => new(gridIndex % width, Mathf.FloorToInt((float) gridIndex / width));
public int PositionIndex(SGrid grid) => grid.x + (grid.y * width);
[System.Serializable]
    public class SlotItem
    {
        public SGrid position;
        public Orientation orientation;
        public int itemID;
        public Item GetItem => ItemData.Instance.items[itemID];
        public float timer;
        
    }
}