using System;
using Castle;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

[GlobalConfig("Assets/Resources/")]
public class ItemData : GlobalConfig<ItemData>
{
    [SerializeReference,ListDrawerSettings(DraggableItems = false)]
    public Item[] items;

    public GameObject bloodExplosion;
    public Blood[] bloodPrefabs;
    public Blood bigBlood;
    public Bullet bullet;
    public GameObject[] props;
    public GameObject[] enemies;
    public Car car;
    private void OnValidate()
    {
        for (var i = 0; i < items.Length; i++)
        {
            items[i].itemID = i;
        }
    }

    public void SpawnBlood(Vector3 pos)
    {
        Instantiate(bloodPrefabs.RandomValue(), pos, Quaternion.identity);
    }

    public void SpawnBigBlood(Vector3 pos)
    {
        Instantiate(bigBlood, pos, Quaternion.identity);
    }
    
    #if UNITY_EDITOR
    public static ValueDropdownList<int> GetItems()
    {
        var dropdown = new ValueDropdownList<int>();
        for (var i = 0; i < Instance.items.Length; i++)
        {
            dropdown.Add(Instance.items[i].name,i);
        }

        return dropdown;
    }
    #endif
}