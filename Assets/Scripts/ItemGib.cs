using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ItemGib : Gib
{
    [ValueDropdown("@ItemData.GetItems()")]
    public int itemID;
}
