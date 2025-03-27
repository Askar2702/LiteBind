using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LiteBindDI
{
    [DefaultExecutionOrder(-1000)]
    public abstract class LiteSceneContext : MonoBehaviour
    {
        public static LiteBindContainer Container { get;private set; }
        private LiteLifecycleRunner _lifecycle;

        protected virtual void Awake()
        {
            Container = new LiteBindContainer(LiteProjectContext.Container);
            InstallBindings(Container);

            _lifecycle = new LiteLifecycleRunner();
            _lifecycle.CollectFromContainer(Container);
            _lifecycle.InitializeAll();

            OnContainerReady(Container);

            InjectAllSceneMonoBehaviours();

        }

        private void Update()
        {
            _lifecycle?.TickAll();
        }

        private void OnDestroy()
        {
            _lifecycle?.DisposeAll();
        }

        protected abstract void InstallBindings(LiteBindContainer container);
        protected virtual void OnContainerReady(LiteBindContainer container) { }

        private void InjectAllSceneMonoBehaviours()
        {
            var sceneObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID)
                 .Where(m => m.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                 .Any(f => f.GetCustomAttribute<LiteInjectAttribute>() != null));

          
            foreach (var obj in sceneObjects)
            {
                Container.InjectInto(obj);
            }
        }


    }
}
