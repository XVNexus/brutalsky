using Brutalsky;
using Brutalsky.Object;
using Core;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Start()
    {
        TestLoad();
    }

    private static void TestLoad()
    {
        var map = BsMap.Load("Brutalsky");
        MapSystem.current.LoadLevel(map, new[]
        {
            new BsPlayer("player-1", "Player 1", 100f, new BsColor(1f, .5f, 0f)),
            new BsPlayer("player-2", "Player 2", 100f, new BsColor(0f, .5f, 1f), true)
        });
    }

    private static void TestChange()
    {
        var map = BsMap.Load("Brutalsky");
        MapSystem.current.ChangeLevel(map);
    }

    private static void TestUnload()
    {
        MapSystem.current.UnloadLevel();
    }
}
