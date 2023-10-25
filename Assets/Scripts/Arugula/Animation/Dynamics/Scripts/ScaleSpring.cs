using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arugula.Animation
{
    public class ScaleSpring : MonoBehaviour
    {
        public Vector3Spring spring;
        public Vector3 goalScale;
        public Transform goal;

        Vector3 Goal => goal == null ? goalScale : goal.localScale;

        private void Start()
        {
            spring.OnPreUpdate += (x) => spring.goal = Goal;
            spring.OnPreUpdate += (x) => spring.Current = transform.localScale;
            spring.OnUpdated += (x) => transform.localScale = spring.Current;
            spring.Start();
        }

        private void OnDestroy()
        {
            spring.Dispose();
        }
    }

}
