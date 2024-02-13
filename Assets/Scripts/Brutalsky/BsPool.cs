using UnityEngine;

namespace Brutalsky
{
    public class BsPool : BsObject
    {
        public Vector2 size { get; set; }
        public BsChemical chemical { get; set; }
        public BsColor color { get; set; }

        public BsPool(string id, Vector2 size, BsChemical chemical, BsColor color) : base(id)
        {
            this.size = size;
            this.chemical = chemical;
            this.color = color;
        }

        protected override GameObject _Create()
        {
            throw new System.NotImplementedException();
        }
    }
}
