using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiteBindDI
{
    public class LiteLifecycleRunner
    {
        private readonly List<IInitializable> _initializables = new();
        private readonly List<IUpdatable> _updatables = new();
        private readonly List<IFixedUpdatable> _fixedUpdatables = new();
        private readonly List<ILateUpdatable> _lateUpdatables = new();
        private readonly List<IDisposableService> _disposables = new();

        public void CollectFromContainer(LiteBindContainer container)
        {
            foreach (var resolved in container.GetAllBoundInstances())
            {
                Debug.Log($"list22233 {resolved}");
                if (resolved is IInitializable init)
                {
                    Debug.Log($"{resolved}  {!_initializables.Contains(init)}");
                    if (!_initializables.Contains(init))
                    {
                        _initializables.Add(init);
                        init.Initialize();
                        Debug.Log($"List Init {resolved}");
                    }
                }


                if (resolved is IUpdatable tick && !_updatables.Contains(tick)) 
                    _updatables.Add(tick);

                if (resolved is IFixedUpdatable fixedTick && !_fixedUpdatables.Contains(fixedTick)) 
                    _fixedUpdatables.Add(fixedTick);

                if (resolved is ILateUpdatable lateTick && !_lateUpdatables.Contains(lateTick)) 
                    _lateUpdatables.Add(lateTick);

                if (resolved is IDisposableService disp && !_disposables.Contains(disp)) 
                    _disposables.Add(disp);
            }
        }

      

        public void TickAll()
        {
            foreach (var u in _updatables)
            {
                try
                {
                    if (u is IConditional cond && !cond.IsActive)
                        continue;

                    u.Tick();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Tick exception in {u.GetType().Name}: {e}");
                }
            }
        }

        public void FixedTickAll()
        {
            foreach (var f in _fixedUpdatables)
            {
                try
                {
                    if (f is IConditional cond && !cond.IsActive)
                        continue;

                    f.FixedTick();
                }
                catch (Exception e)
                {
                    Debug.LogError($"FixedTick exception in {f.GetType().Name}: {e}");
                }
            }
        }

        public void LateTickAll()
        {
            foreach (var l in _lateUpdatables)
            {
                try
                {
                    if (l is IConditional cond && !cond.IsActive)
                        continue;

                    l.LateTick();
                }
                catch (Exception e)
                {
                    Debug.LogError($"LateTick exception in {l.GetType().Name}: {e}");
                }
            }
        }

        public void DisposeAll()
        {
            foreach (var d in _disposables)
            {
                try { d.Dispose(); }
                catch (Exception e)
                {
                    Debug.LogError($"Dispose exception in {d.GetType().Name}: {e}");
                }
            }
        }
    }
}
