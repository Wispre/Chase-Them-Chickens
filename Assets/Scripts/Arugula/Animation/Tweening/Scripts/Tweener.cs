/*
MIT License

Copyright(c) 2019 Mitchel Thompson
www.angryarugula.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

//#define TWEENER_DEBUG

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace Arugula.Animation
{

    public static class Defaults
    {
        public const float duration = 1;
        public const float delay = 0;
        public const Easing easing = Easing.easeInOutSine;
        public const LoopMode loop = LoopMode.None;
        public const TimeMode timeMode = TimeMode.Scaled;
        public const UpdateMode updateMode = UpdateMode.Update;
        public const Space space = Space.Self;

        public static Tween.Options options = new Tween.Options();
    }

    public enum Easing
    {
        linear,
        easeInBack,
        easeInBounce,
        easeInOutBack,
        easeInOutSine,
        easeInSine,
        easeInQuadratic,
        easeInElastic,
        easeOutBack,
        easeOutBounce,
        easeOutSine,
        easeOutQuadratic,
        easeOutElastic
    }

    public enum LoopMode
    {
        None,
        Repeat,
        PingPong
    }

    public interface ITween
    {
        void Update();
        void Update(float deltaTime);
        void Finish(bool complete = true);
        float Duration { get; set; }
        float Delay { get; set; }
        float Speed { get; set; }
        float Time { get; set; }
        float NormalizedTime { get; set; }
        LoopMode Loop { get; set; }
        TimeMode TimeMode { get; set; }
        Object Context { get; set; }
        bool Paused { get; set; }
        bool Active { get; }
        bool Finished { get; }
        int LoopCount { get; }
        event Action OnStarted;
        event Action OnUpdated;
        event Action<int> OnLooped;
        event Action<bool> OnFinished;
        event Action OnContextLost;

    }
    public interface ITween<T> : ITween
    {
        T Origin { get; set; }
        T Target { get; set; }
        T Current { get; }
        event Action<T> OnUpdatedValue;
    }



    //[Serializable]  //TODO:  Can't serialize natively this until Unity 2020 sad face.
    /*
    public class Options<T> : Options
    {
        public T origin;
        public T target;
    }
    */

    public class Tween : ITween
    {
        [Serializable]
        public class Options : ICloneable
        {
            public float duration = Defaults.duration;
            public float delay = Defaults.delay;
            public Easing easing = Defaults.easing;
            public AnimationCurve curve = null;
            public LoopMode loop = Defaults.loop;
            public TimeMode timeMode = Defaults.timeMode;

            public Options(
                float duration = Defaults.duration,
                Easing easing = Defaults.easing,
                float delay = Defaults.delay,
                AnimationCurve curve = null,
                LoopMode loop = Defaults.loop,
                TimeMode timeMode = Defaults.timeMode)
            {
                this.duration = duration;
                this.easing = easing;
                this.delay = delay;
                this.curve = curve;
                this.loop = loop;
                this.timeMode = timeMode;
            }

            public bool UsesCustomCurve => curve != null && curve.keys.Length >= 2;

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        public float Duration
        {
            get { return duration; }
            set
            {
                if (value <= 0)
                    return;

                if (duration != value)
                {
                    if (Time > 0)
                    {
                        float p = Time / (Delay + Duration);
                        Time = Mathf.Lerp(0, Delay + value, p);
                    }
                }

                duration = value;
            }

        }

        float speed = 1;
        public float Speed
        {
            get => speed;
            set
            {
                speed = Mathf.Clamp(value, 0, float.MaxValue);
            }
        }

        public float Time
        {
            get { return time; }
            set
            {
                time = value;
                if (Finished && time < Delay + Duration)
                    Finished = false;

                Update(0);
            }
        }
        public float NormalizedTime
        {
            get { return time / (Delay + Duration); }
            set
            {
                Time = Mathf.Lerp(0, Delay + Duration, Mathf.Clamp01(value));
            }
        }
        public LoopMode Loop { get; set; }
        public TimeMode TimeMode { get; set; }
        public float Delay { get; set; }
        public Object Context
        {
            get
            {
                return context;
            }
            set
            {
                context = value;
                useContext = context != null;
            }
        }

        public bool Paused { get; set; }
        public bool Active { get; private set; }
        public bool Finished { get; private set; }
        public int LoopCount { get; private set; }

        public event Action OnStarted = delegate { };
        public event Action OnUpdated = delegate { };
        public event Action<bool> OnFinished = delegate { };
        public event Action OnContextLost = delegate { };
        public event Action<int> OnLooped = delegate { };

        protected AnimationCurve curve;
        protected bool useContext;
        protected Object context;
        protected float duration;
        protected float time;
        private int direction = 1;

        public Tween(Options opts, Object context = null)
        {
            Duration = opts.duration;
            Delay = opts.delay;
            Loop = opts.loop;
            Context = context;
            TimeMode = opts.timeMode;
            curve = opts.UsesCustomCurve ? opts.curve : Tweener.GetCurve(opts.easing);
        }

        public virtual void UpdateCurrent(float p)
        {
            //IE: for vector3 tweens
            //Current = Vector3.LerpUnclamped(Origin, Target, curve.Evaluate(p));
        }

        public void Update() => Update(TimeMode == TimeMode.Scaled ? UnityEngine.Time.deltaTime : UnityEngine.Time.unscaledDeltaTime);

        public void Update(float deltaTime)
        {
            if (Paused || Finished)
                return;

            if (useContext && Context == null)
            {
                OnContextLost();
                Finish(false);
                return;
            }

            time += deltaTime * direction * Speed;

            if (Time >= Delay)
            {
                if (!Active)
                {
                    OnStarted();
                    Active = true;
                }

                float p = Mathf.Clamp01((Time - Delay) / Duration);
                UpdateCurrent(p);
                OnUpdated();

                if (p == 1)
                {
                    switch (Loop)
                    {
                        case LoopMode.None:
                            Finish();
                            break;
                        case LoopMode.Repeat:
                            Time = 0;
                            Active = false;
                            LoopCount++;
                            OnLooped(LoopCount);
                            break;
                        case LoopMode.PingPong:
                            if (direction == 1)
                                time = duration;
                            else
                                time = 0;
                            direction *= -1;
                            LoopCount++;
                            OnLooped(LoopCount);
                            break;
                    }
                }
            }
            else
            {
                Active = false;

                //TODO: Why did this get added?
                //UpdateCurrent(0);

                if (direction == -1 && Time <= 0)
                {
                    Time = 0;
                    direction = 1;
                }
            }
        }

        public virtual void Finish(bool complete = true)
        {
            if (complete && Loop == LoopMode.None && NormalizedTime < 1)
                UpdateCurrent(1);

            Active = false;
            Finished = true;
            OnFinished(complete);
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}/{2}s {3}", GetType(), (Time - Delay).ToString("f2"), Duration.ToString("f2"), Context);
        }
    }

    public class FloatTween : Tween, ITween<float>
    {
        public FloatTween(float origin, float target, Options opts, Object context = null) : base(opts, context)
        {
            Origin = Current = origin;
            Target = target;
        }

        public float Origin { get; set; }
        public float Target { get; set; }
        public float Current { get; private set; }

        public event Action<float> OnUpdatedValue = delegate { };

        public override void UpdateCurrent(float p)
        {
            Current = Mathf.LerpUnclamped(Origin, Target, curve.Evaluate(p));
            OnUpdatedValue(Current);
        }
    }

    public class Vector2Tween : Tween, ITween<Vector2>
    {
        public Vector2Tween(Vector2 origin, Vector2 target, Options opts, Object context = null) : base(opts, context)
        {
            Origin = Current = origin;
            Target = target;
        }

        public Vector2 Origin { get; set; }
        public Vector2 Target { get; set; }
        public Vector2 Current { get; private set; }

        public event Action<Vector2> OnUpdatedValue = delegate { };

        public override void UpdateCurrent(float p)
        {
            Current = Vector2.LerpUnclamped(Origin, Target, curve.Evaluate(p));
            OnUpdatedValue(Current);
        }
    }

    public class Vector3Tween : Tween, ITween<Vector3>
    {
        public Vector3Tween(Vector3 origin, Vector3 target, Options opts, Object context = null) : base(opts, context)
        {
            Origin = Current = origin;
            Target = target;
        }

        public Vector3 Origin { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Current { get; private set; }

        public event Action<Vector3> OnUpdatedValue = delegate { };

        public override void UpdateCurrent(float p)
        {
            Current = Vector3.LerpUnclamped(Origin, Target, curve.Evaluate(p));
            OnUpdatedValue(Current);
        }
    }

    public class ColorTween : Tween, ITween<Color>
    {
        public ColorTween(Color origin, Color target, Options opts, Object context = null) : base(opts, context)
        {
            Origin = Current = origin;
            Target = target;
        }

        public Color Origin { get; set; }
        public Color Target { get; set; }
        public Color Current { get; private set; }

        public event Action<Color> OnUpdatedValue = delegate { };

        public override void UpdateCurrent(float p)
        {
            Current = Color.LerpUnclamped(Origin, Target, curve.Evaluate(p));
            OnUpdatedValue(Current);
        }
    }

    public class Vector3Punch : Tween, ITween<Vector3>
    {
        public Vector3Punch(Vector3 origin, Vector3 target, Options opts, Object context = null) : base(opts, context)
        {
            Origin = origin;
            Target = target;
            Current = origin + new Vector3(Random.Range(-Target.x, Target.x), Random.Range(-Target.y, Target.y), Random.Range(-Target.z, Target.z));
        }

        public override void UpdateCurrent(float p)
        {
            float intensity = curve.Evaluate(1f - p);

            Vector3 offset = Vector3.zero;
            offset.x = Random.Range(-Target.x, Target.x) * intensity;
            offset.y = Random.Range(-Target.y, Target.y) * intensity;
            offset.z = Random.Range(-Target.z, Target.z) * intensity;

            Current = Origin + offset;

            OnUpdatedValue(Current);
        }

        public Vector3 Origin { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Current { get; private set; }

        public event Action<Vector3> OnUpdatedValue = delegate { };
    }

    public class QuaternionTween : Tween, ITween<Quaternion>
    {
        public QuaternionTween(Quaternion origin, Quaternion target, Options opts, Object context = null) : base(opts, context)
        {
            Origin = Current = origin;
            Target = target;
        }

        public Quaternion Origin { get; set; }
        public Quaternion Target { get; set; }
        public Quaternion Current { get; private set; }

        public event Action<Quaternion> OnUpdatedValue = delegate { };

        public override void UpdateCurrent(float p)
        {
            Current = Quaternion.SlerpUnclamped(Origin, Target, curve.Evaluate(p));
            OnUpdatedValue(Current);
        }
    }

    public class TransformPoseTween : Tween, ITween<TransformPose>
    {
        public TransformPoseTween(TransformPose origin, TransformPose target, Options opts, Object context = null) : base(opts, context)
        {
            Origin = Current = origin;
            Target = target;
        }

        public TransformPose Origin { get; set; }
        public TransformPose Target { get; set; }
        public TransformPose Current { get; private set; }

        public event Action<TransformPose> OnUpdatedValue = delegate { };

        public override void UpdateCurrent(float p)
        {
            Current = TransformPose.LerpUnclamped(Origin, Target, curve.Evaluate(p));
            OnUpdatedValue(Current);
        }
    }

    public class Tweener : MonoBehaviour
    {
        static Queue<ITween> tweenQueue = new Queue<ITween>();
        static List<ITween> tweens = new List<ITween>();

        static Queue<ITween> lateTweenQueue = new Queue<ITween>();
        static List<ITween> lateTweens = new List<ITween>();

        static Queue<ITween> fixedTweenQueue = new Queue<ITween>();
        static List<ITween> fixedTweens = new List<ITween>();

        static Queue<ITween> finishedTweens = new Queue<ITween>();

        static Dictionary<string, AnimationCurve> curveCache = new Dictionary<string, AnimationCurve>();

        static Tweener inst;
        static Tweener()
        {
            if (inst == null)
            {
                GameObject go = new GameObject("Tweener", typeof(Tweener));
                go.hideFlags = HideFlags.HideInHierarchy;
                DontDestroyOnLoad(go);
                inst = go.GetComponent<Tweener>();
            }
        }

        public static AnimationCurve GetCurve(string name)
        {
            if (curveCache.ContainsKey(name))
                return curveCache[name];

            curveCache.Add(name, Resources.Load<TweenCurve>(name).curve);
            return curveCache[name];
        }

        public static AnimationCurve GetCurve(Easing easing)
        {
            return GetCurve(easing.ToString());
        }

        public static void Enqueue(ITween tween, UpdateMode updateMode = UpdateMode.Update)
        {
            switch (updateMode)
            {
                case UpdateMode.Update:
                    tweenQueue.Enqueue(tween);
                    break;
                case UpdateMode.LateUpdate:
                    lateTweenQueue.Enqueue(tween);
                    break;
                case UpdateMode.FixedUpdate:
                    fixedTweenQueue.Enqueue(tween);
                    break;
            }

            tween.OnFinished += (complete) => finishedTweens.Enqueue(tween);
            tween.OnContextLost += () => finishedTweens.Enqueue(tween);
        }

#if TWEENER_DEBUG
        private void OnGUI()
        {
            foreach (var t in tweens)
                GUILayout.Label(t.ToString());
        }
#endif

        private void Update()
        {
            while (tweenQueue.Count > 0) tweens.Add(tweenQueue.Dequeue());
            foreach (var t in tweens) t.Update();
            while (finishedTweens.Count > 0) tweens.Remove(finishedTweens.Dequeue());
        }

        private void FixedUpdate()
        {
            while (fixedTweenQueue.Count > 0) fixedTweens.Add(fixedTweenQueue.Dequeue());
            foreach (var t in fixedTweens) t.Update();
            while (finishedTweens.Count > 0) fixedTweens.Remove(finishedTweens.Dequeue());
        }

        private void LateUpdate()
        {
            while (lateTweenQueue.Count > 0) lateTweens.Add(lateTweenQueue.Dequeue());
            foreach (var t in lateTweens) t.Update();
            while (finishedTweens.Count > 0) lateTweens.Remove(finishedTweens.Dequeue());
        }

    }
}
