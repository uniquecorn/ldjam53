using System.Collections.Generic;
using Castle;
using UnityEngine;

public class Cluster : MonoBehaviour
{
    public SGrid grid;
    public const int PropsCount = 50;
    public const int EnemyCount = 5;
    public void Generate()
    {
        var pos = new Vector2(grid.x * 100, grid.y * 100);
        List<Vector2> points = new List<Vector2>(PropsCount);
        for (var i = 0; i < PropsCount; i++)
        {
            points.Add(new Vector2(Random.Range(-50f,50f),Random.Range(-50f,50f)));
        }

        for (var i = points.Count-1; i >= 0; i--)
        {
            for (var j = i; j >= 0; j--)
            {
                if(j==i)continue;
                if ((points[j] - points[i]).sqrMagnitude < 25)
                {
                    points.RemoveAt(i);
                    break;
                }
            }
        }

        var carsToSpawn = 1;
        for(var i = 0; i < points.Count; i++)
        {
            if (i < carsToSpawn)
            {
                Instantiate(ItemData.Instance.car,pos + points[i],Quaternion.identity);
            }
            else if (Random.value > 0.5f)
            {
                Instantiate(ItemData.Instance.props.RandomValue(), pos + points[i],Quaternion.identity);
            }
            else
            {
                Instantiate(ItemData.Instance.enemies.RandomValue(), pos + points[i],Quaternion.identity);
            }
        }
    }
}