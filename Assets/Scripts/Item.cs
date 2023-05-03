using System.Collections.Generic;
using Castle;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable,HideReferenceObjectPicker]
public class Item
{
    [HideInInspector]
    public int itemID;
    [HorizontalGroup("Details",50f),PreviewField(50,ObjectFieldAlignment.Right),HideLabel,OnValueChanged("UpdateSizeFromIcon")]
    public Sprite icon;
    [VerticalGroup("Details/Text"),SuffixLabel("Name",true),HideLabel]
    public string name;
    [VerticalGroup("Details/Text"),TextArea,SuffixLabel("Description",true),HideLabel]
    public string description;
    [HorizontalGroup("Size"),HideLabel,SuffixLabel("Width"),ReadOnly]
    public int width; 
    [HorizontalGroup("Size"),HideLabel,SuffixLabel("Height"),ReadOnly]
    public int height;
[HorizontalGroup("Size",10f),SuffixLabel("Edit"),HideLabel,LabelWidth(5f)]
    public bool editGrids;
    public ItemGib gib;
    public SGrid[] blank;
    public bool nonrollable;
    public SGrid Origin(Orientation orientation)
    {
        if (orientation == Orientation.Right) return new SGrid(0, 0);
        switch (orientation)
        {
            case Orientation.Up:
                return new SGrid(0, width - 1);
            case Orientation.Left:
                return new SGrid(width - 1, height - 1);
            case Orientation.Down:
                return new SGrid(width - 1, 0);
        }
        return new SGrid(0, 0);
    }

    public SGrid[] TransposedBlanks(Orientation orientation)
    {
        if (orientation == Orientation.Right)
        {
            return blank;
        }
        var origin = Origin(orientation);
        var transposed = new SGrid[blank.Length];
        for (var i = 0; i < blank.Length; i++)
        {
            //1,1
            transposed[i] = new SGrid(Mathf.Abs(origin.x - blank[i].x), Mathf.Abs(origin.y - blank[i].y));
        }
        return transposed;
    }
    #if UNITY_EDITOR
[ShowInInspector,ShowIf("editGrids"),TableMatrix(DrawElementMethod = "DrawCell",HideColumnIndices = true,HideRowIndices = true,SquareCells = true),OnValueChanged("UpdateBlankGrids",true)]
    public bool[,] GridView
    {
        get
        {
            var view = new bool[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    view[x, y] = true;
                }
            }

            if (blank.IsSafe())
            {
                foreach (var g in blank)
                {
                    if(g.x >= width || g.y >= height)continue;
                    view[g.x, g.y] = false;
                }
            }
            return view;
        }
        set => UpdateBlankGrids(value);
    }

    void UpdateBlankGrids(bool[,] view)
    {
        var blanks = new List<SGrid>();
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (!view[x, y])
                {
                    blanks.Add(new SGrid(x,y));
                }
            }
        }
        blank = blanks.ToArray();
    }

    void UpdateSizeFromIcon(Sprite icon)
    {
        width = Mathf.RoundToInt(icon.rect.width / 250);
        height = Mathf.RoundToInt(icon.rect.height / 250);
    }
    static bool DrawCell(Rect rect, bool value)
    {
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            value = !value;
            GUI.changed = true;
            Event.current.Use();
        }
        EditorGUI.DrawRect(rect.Padding(1),value ? Color.gray:Color.black);
        return value;
    }
    #endif
    public virtual void UpdateSlot(Inventory.SlotItem slot)
    {
        
    }
}

public interface IStatMod
{
    float Value { get; }
}
public interface ISpeedItem : IStatMod { }

public interface IFireRate : IStatMod { }
public interface IProjectileSpeed : IStatMod { }
public interface IAccuracy : IStatMod { }
public interface IAcceleration : IStatMod{}
public interface IHandling : IStatMod{}
public class Finger : Item, IFireRate
{
    public float Value => 0.1f;
}

public class Gloves : Item, IHandling
{
    public float Value => 0.2f;
}

public class Foot : Item, IAcceleration
{
    public float Value => 0.5f;
}
public class Tire : Item, ISpeedItem
{
public float Value => 0.5f;
}

public interface IHitMultiplier : IStatMod { }

public class Bat : Item, IHitMultiplier
{
    public float Value => 0.25f;
}