using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arugula.Animation
{
    [System.Serializable]
    public struct TransformPose
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformPose(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        public TransformPose(Transform transform)
        {
            this.position = transform.localPosition;
            this.rotation = transform.localRotation;
            this.scale = transform.localScale;
        }

        public void Apply(Transform t)
        {
            t.localPosition = position;
            t.localRotation = rotation;
            t.localScale = scale;
        }

        public static TransformPose Lerp(TransformPose a, TransformPose b, float p)
        {
            return new TransformPose(
                Vector3.Lerp(a.position, b.position, p),
                Quaternion.Slerp(a.rotation, b.rotation, p),
                Vector3.Lerp(a.scale, b.scale, p));
        }

        public static TransformPose LerpUnclamped(TransformPose a, TransformPose b, float p)
        {
            return new TransformPose(
                Vector3.LerpUnclamped(a.position, b.position, p),
                Quaternion.SlerpUnclamped(a.rotation, b.rotation, p),
                Vector3.LerpUnclamped(a.scale, b.scale, p));
        }
    }
}
