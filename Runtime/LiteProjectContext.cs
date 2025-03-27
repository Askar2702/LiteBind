using UnityEngine;

namespace LiteBindDI
{
    [DefaultExecutionOrder(-1000)]
    public abstract class LiteProjectContext : MonoBehaviour
    {
        public static LiteBindContainer Container { get; private set; }

        private void Awake()
        {
            if (Container != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            Container = new LiteBindContainer();
            InstallBindings(Container);
        }

        protected abstract void InstallBindings(LiteBindContainer container);
    }
}
