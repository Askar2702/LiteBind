using System;
using System.Collections.Generic;

namespace LiteBindDI
{
    public class LiteLifecycleRunner
    {
        private readonly List<IInitializable> _initializables = new();
        private readonly List<IUpdatable> _updatables = new();
        private readonly List<IDisposableService> _disposables = new();

        public void CollectFromContainer(LiteBindContainer container)
        {
            foreach (var resolved in container.GetAllBoundInstances())
            {
                if (resolved is IInitializable init)
                    _initializables.Add(init);
                if (resolved is IUpdatable tick)
                    _updatables.Add(tick);
                if (resolved is IDisposableService disp)
                    _disposables.Add(disp);
            }

        }

        public void InitializeAll()
        {
            foreach (var i in _initializables)
                i.Initialize();
        }

        public void TickAll()
        {
            foreach (var u in _updatables)
                u.Tick();
        }

        public void DisposeAll()
        {
            foreach (var d in _disposables)
                d.Dispose();
        }
    }
}
