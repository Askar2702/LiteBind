using LiteBindDI.Services.Localization;
using UnityEngine;

namespace LiteBindDI
{
    [DefaultExecutionOrder(-1000)]
    public abstract class LiteProjectContext : MonoBehaviour
    {
        public static LiteBindContainer Container { get; private set; }
        private static LiteLifecycleRunner _lifecycle;
        private void Awake()
        {
            if (Container != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Container = new LiteBindContainer();
            InstallBindings();
            InstallBindings(Container);
            CollectFromContainer(Container);
        }
      
        protected abstract void InstallBindings(LiteBindContainer container);

        private void InstallBindings()
        {
            Container.BindSingletonInterfaceAndSelf<ILocalizationService, LocalizationService>(new LocalizationService());
        }

        public static void CollectFromContainer(LiteBindContainer container)
        {
            _lifecycle ??= new LiteLifecycleRunner();
            _lifecycle.CollectFromContainer(container);
        }

        private void Start()
        {
            _lifecycle?.StartAll();
        }

        private void Update()
        {
            _lifecycle?.TickAll();
        }
        private void FixedUpdate()
        {
            _lifecycle?.FixedTickAll();
        }

        private void LateUpdate()
        {
            _lifecycle?.LateTickAll();
        }
        private void OnDestroy()
        {
            _lifecycle?.DisposeAll();
        }
    }
}
