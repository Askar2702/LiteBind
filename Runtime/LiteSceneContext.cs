using System.Collections.Generic;
using System.ComponentModel;
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


            OnContainerReady(Container);
            InjectAllSceneMonoBehaviours();

            _lifecycle = new LiteLifecycleRunner();
            _lifecycle.CollectFromContainer(Container);
            _lifecycle.InitializeAll();

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
            foreach (var instance in Container.GetAllBoundInstances())
            {
                Container.InjectInto(instance); // и вот тут поле с атрибутом точно будет
            }
        }



    }
}
