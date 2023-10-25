using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arugula.Animation
{
    //TODO: Deprecate this interface for 2020 when you can serialize generics
    public interface ISpring<T>
    {
        event Action<Spring> OnPreUpdate;
        event Action<Spring> OnUpdated;
        void Start();
        void Stop();
        void Update();
        void Step(float deltaTime, int subSteps);
        void ClearEvents();
        T Current { get; set; }
        T Velocity { get; set; }
    }

    public abstract class Spring : IDisposable
    {
        //TODO: Include T when 2020 can serialize generics
        public event Action<Spring> OnPreUpdate = delegate { };
        public event Action<Spring> OnUpdated = delegate { };

        public UpdateMode updateMode;
        public TimeMode timeMode;

        public float strength = 50;
        public float damper = 3f;
        public float threshold = 0.01f;
        public int subSteps = 5;
        protected bool disposed;

        public void Start()
        {
            if (disposed)
                return;

            switch (updateMode)
            {
                case UpdateMode.FixedUpdate:
                    UnityEventDispatcher.OnFixedUpdate += Update;
                    break;
                case UpdateMode.LateUpdate:
                    UnityEventDispatcher.OnLateUpdate += Update;
                    break;
                case UpdateMode.Update:
                    UnityEventDispatcher.OnUpdate += Update;
                    break;
            }
        }

        public void Stop()
        {
            switch (updateMode)
            {
                case UpdateMode.FixedUpdate:
                    UnityEventDispatcher.OnFixedUpdate -= Update;
                    break;
                case UpdateMode.LateUpdate:
                    UnityEventDispatcher.OnLateUpdate -= Update;
                    break;
                case UpdateMode.Update:
                    UnityEventDispatcher.OnUpdate -= Update;
                    break;
            }
        }

        public void Dispose()
        {
            disposed = true;
            ClearEvents();
            Stop();
        }

        public virtual void Update()
        {
            if (OnPreUpdate != null)
                OnPreUpdate(this);
            Step(timeMode == TimeMode.Scaled ? Time.deltaTime : Time.unscaledDeltaTime, subSteps);
            if (OnUpdated != null)
                OnUpdated(this);
        }

        public abstract void Step(float deltaTime, int subSteps = 1);

        public void ClearEvents()
        {
            OnUpdated = null;
            OnPreUpdate = null;
        }
    }

}
