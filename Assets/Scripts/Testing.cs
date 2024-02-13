using Brutalsky;
using Core;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public void Start()
    {
        var map = new BsMap();
        map.pools.Add(new BsPool(new BsTransform(0f, -7.5f), new Vector2(10f, 5f),
            BsChemical.Water(), BsColor.Water(BsLayer.Midground)));
        MapSystem.current.Load(map);
    }
}
