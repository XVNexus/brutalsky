using UnityEngine;

namespace Extensions
{
    public static class GameObjectExt
    {
        public static void SetChildrenActive(this GameObject _, bool value)
        {
            for (var i = 0; i < _.transform.childCount; i++)
            {
                _.transform.GetChild(i).gameObject.SetActive(value);
            }
        }
    }
}
