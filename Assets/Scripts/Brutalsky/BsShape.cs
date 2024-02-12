using UnityEngine;

namespace Brutalsky
{
    public class BsShape : BsObject
    {
        public BsPath path { get; set; }
        public BsMaterial material { get; set; }

        protected override GameObject _Create()
        {
            throw new System.NotImplementedException();
        }
    }
}
