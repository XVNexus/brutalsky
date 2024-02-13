using System.Collections.Generic;
using System.Linq;
using Random = Unity.Mathematics.Random;

namespace Brutalsky
{
    public class BsMap
    {
        public List<BsShape> shapes;
        public List<BsJoint> joints;
        public List<BsSpawn> spawns;

        private Random random = Random.CreateFromIndex(0);

        public void Create()
        {
            foreach (var shape in shapes)
            {
                shape.Create();
            }
            foreach (var joint in joints)
            {
                joint.Create();
            }
        }

        public void Destroy()
        {
            foreach (var joint in joints)
            {
                joint.Destroy();
            }
            foreach (var shape in shapes)
            {
                shape.Destroy();
            }
        }

        public void Spawn(BsPlayer player)
        {
            var leastUsages = spawns.Select(spawn => spawn.usages).Prepend(int.MaxValue).Min();
            var possibleSpawns = spawns.Where(spawn => spawn.usages == leastUsages).ToList();
            var spawnChoice = possibleSpawns[random.NextInt(possibleSpawns.Count)];
            player.Create();
            player.instance.transform.position = spawnChoice.Use();
        }

        public void Despawn(BsPlayer player)
        {
            player.Destroy();
        }

        public void SpawnAll(IEnumerable<BsPlayer> players)
        {
            foreach (var player in players)
            {
                Spawn(player);
            }
        }

        public void DespawnAll(IEnumerable<BsPlayer> players)
        {
            foreach (var player in players)
            {
                Despawn(player);
            }
        }

        public void ResetSpawns()
        {
            foreach (var spawn in spawns)
            {
                spawn.Reset();
            }
        }
    }
}
