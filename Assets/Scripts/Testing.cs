using Brutalsky;
using Brutalsky.Object;
using Core;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public void Start()
    {
        MapSystem.current.Load(BsMap.Load("Brutalsky"));
        MapSystem.current.Spawn(new BsPlayer("player-1", "Player 1", 100f, new BsColor(1f, .5f, 0f)));
        MapSystem.current.Spawn(new BsPlayer("player-2", "Player 2", 100f, new BsColor(0f, .5f, 1f), true));
    }
}
