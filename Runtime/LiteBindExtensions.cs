using UnityEngine;

namespace LiteBindDI
{
    public static class LiteBindExtensions
    {
        public static T InstantiateInjected<T>(this T prefab, Vector3 position, Quaternion rotation) where T : Object
        {
            var container = LiteProjectContext.Container;

            if (container == null)
                throw new LiteBindException("[LiteBind] Cannot instantiate — global container is null.");

            var instance = Object.Instantiate(prefab, position, rotation);

            if (instance is GameObject go)
            {
                foreach (var mono in go.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    container.InjectInto(mono);
                    container.BindSingleton(mono); // <=== ÄÎÁÀÂÈË
                }
            }
            else if (instance is MonoBehaviour mono)
            {
                container.InjectInto(mono);
                container.BindSingleton(mono); // <=== ÄÎÁÀÂÈË
            }

            return instance;
        }
    }
}
