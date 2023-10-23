using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arugula.Animation
{
    public static class TweenExtensions
    {
        #region Value Types
        public static ITween<float> To(this float x, float target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null)
        {
            FloatTween tween = new FloatTween(x, target, opts, context);
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }
        #endregion


        #region Transform
        public static ITween<Vector3> MoveTo(this Transform t, Vector3 target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null, Space space = Defaults.space)
        {
            Vector3Tween tween = new Vector3Tween(Vector3.zero, target, opts, context ? context : t);
            tween.OnStarted += () => tween.Origin = space == Space.Self ? t.localPosition : t.position;
            if (space == Space.Self) tween.OnUpdatedValue += (x) => t.localPosition = x;
            else tween.OnUpdatedValue += (x) => t.position = x;
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }

        public static ITween<Vector3> RotateTo(this Transform t, Vector3 target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null, Space space = Defaults.space)
        {
            Vector3Tween tween = new Vector3Tween(Vector3.zero, target, opts, context ? context : t);
            tween.OnStarted += () => tween.Origin = space == Space.Self ? t.localEulerAngles : t.eulerAngles;
            if (space == Space.Self) tween.OnUpdatedValue += (x) => t.localEulerAngles = x;
            else tween.OnUpdatedValue += (x) => t.eulerAngles = x;
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }

        public static ITween<Quaternion> RotateTo(this Transform t, Quaternion target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null, Space space = Defaults.space)
        {
            QuaternionTween tween = new QuaternionTween(Quaternion.identity, target, opts, context ? context : t);
            tween.OnStarted += () => tween.Origin = space == Space.Self ? t.localRotation : t.rotation;
            if (space == Space.Self) tween.OnUpdatedValue += (x) => t.localRotation = x;
            else tween.OnUpdatedValue += (x) => t.rotation = x;
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }

        public static ITween<Vector3> ScaleTo(this Transform t, Vector3 target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null)
        {
            Vector3Tween tween = new Vector3Tween(Vector3.zero, target, opts, context ? context : t);
            tween.OnStarted += () => tween.Origin = t.localScale;
            tween.OnUpdatedValue += (x) => t.localScale = x;
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }

        public static ITween<Vector3> PunchPosition(this Transform t, Vector3 amount, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null)
        {
            Vector3Punch tween = new Vector3Punch(Vector3.zero, amount, opts, context ? context : t);
            tween.OnStarted += () => tween.Origin = t.localPosition;
            tween.OnUpdatedValue += (x) => t.localPosition = x;
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }

        public static ITween<TransformPose> PoseTo(this Transform t, TransformPose target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null)
        {
            TransformPoseTween tween = new TransformPoseTween(target, target, opts, context ? context : t);
            tween.OnStarted += () => tween.Origin = new TransformPose(t);

            tween.OnUpdatedValue += (x) => x.Apply(t);

            Tweener.Enqueue(tween, updateMode);
            return tween;
        }
        #endregion

        #region Rigidbody
        public static ITween<Vector3> MoveTo(this Rigidbody rb, Vector3 target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null)
        {
            Vector3Tween tween = new Vector3Tween(Vector3.zero, target, opts, context ? context : rb);
            tween.OnStarted += () => tween.Origin = rb.position;
            tween.OnUpdatedValue += (x) => rb.MovePosition(x);
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }

        public static ITween<Quaternion> RotateTo(this Rigidbody rb, Quaternion target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null, Space space = Defaults.space)
        {
            QuaternionTween tween = new QuaternionTween(Quaternion.identity, target, opts, context ? context : rb);
            tween.OnStarted += () => tween.Origin = rb.rotation;
            tween.OnUpdatedValue += (x) => rb.MoveRotation(x);
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }

        public static ITween<Vector3> MoveTo(this Rigidbody2D rb, Vector3 target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null)
        {
            Vector3Tween tween = new Vector3Tween(Vector3.zero, target, opts, context ? context : rb);
            tween.OnStarted += () => tween.Origin = rb.position;
            tween.OnUpdatedValue += (x) => rb.MovePosition(x);
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }

        public static ITween<float> RotateTo(this Rigidbody2D rb, float target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null, Space space = Defaults.space)
        {
            FloatTween tween = new FloatTween(0, target, opts, context ? context : rb);
            tween.OnStarted += () => tween.Origin = rb.rotation;
            tween.OnUpdatedValue += (x) => rb.MoveRotation(x);
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }
        #endregion

        #region Color
        public static ITween<Color> FadeTo(this Color color, Color target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null)
        {
            ColorTween tween = new ColorTween(color, target, opts, context);
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }

        public static ITween<Color> FadeTo(this SpriteRenderer sprite, Color target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null)
        {
            ITween<Color> tween = sprite.color.FadeTo(target, opts, updateMode, context ? context : sprite);
            tween.OnStarted += () => tween.Origin = sprite.color;
            tween.OnUpdatedValue += (x) => sprite.color = x;
            return tween;
        }
        #endregion

        #region Audio
        public static ITween<float> VolumeTo(this AudioSource src, float target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null)
        {
            FloatTween tween = new FloatTween(0, target, opts, context ? context : src);
            tween.OnStarted += () => tween.Origin = src.volume;
            tween.OnUpdatedValue += (x) => src.volume = x;
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }

        public static ITween<float> CrossFade(this AudioSource a, AudioSource b, float target, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null)
        {
            FloatTween tween = new FloatTween(0, 0, opts, context ? context : a);
            tween.OnStarted += () => tween.Origin = a.volume;
            tween.OnUpdatedValue += (x) => { a.volume = x; b.volume = tween.NormalizedTime * target; };
            Tweener.Enqueue(tween, updateMode);
            return tween;
        }
        #endregion

        #region Sprites
        public static ITween<float> Cycle(this SpriteRenderer rend, Sprite[] sprites, Tween.Options opts, UpdateMode updateMode = Defaults.updateMode, Object context = null)
        {
            FloatTween tween = new FloatTween(0, sprites.Length - 1, opts, context ? context : rend);
            tween.OnUpdatedValue += (x) => rend.sprite = sprites[Mathf.FloorToInt(x)];
            Tweener.Enqueue(tween);
            return tween;
        }
        #endregion
    }
}

