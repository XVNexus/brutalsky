using Brutalsky;
using Brutalsky.Object;
using Core;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Start()
    {
        var map = BsMap.Load("Brutalsky");
        MapSystem.current.Build(map);
        PlayerSystem.current.Spawn(map, new[]
        {
            new BsPlayer("player-1", "Player 1", 100f, new BsColor(1f, .5f, 0f)),
            new BsPlayer("player-2", "Player 2", 100f, new BsColor(0f, .5f, 1f), true)
        });
    }
}
