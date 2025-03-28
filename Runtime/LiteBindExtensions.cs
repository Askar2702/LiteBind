using UnityEngine;

namespace LiteBindDI
{
    public static class LiteBindExtensions
    {
        public static T InstantiateInjected<T>(this T prefab, Vector3 position, Quaternion rotation) where T : MonoBehaviour
        {
            var container = LiteProjectContext.Container;

            if (container == null)
                throw new LiteBindException("[LiteBind] Cannot instantiate — global container is null.");

            var instance = Object.Instantiate(prefab, position, rotation);
            container.InjectInto(instance);
            container.BindSingleton(instance);

            return instance;
        }


    }
}
