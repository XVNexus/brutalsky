using UnityEngine;

namespace Brutalsky
{
    public class BsPool : BsObject
    {
        public Vector2 size { get; set; }
        public BsChemical chemical { get; set; }

        protected override GameObject _Create()
        {
            throw new System.NotImplementedException();
        }
    }
}
