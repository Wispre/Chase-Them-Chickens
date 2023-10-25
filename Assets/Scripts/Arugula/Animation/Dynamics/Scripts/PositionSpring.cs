using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arugula.Animation
{
    public class PositionSpring : MonoBehaviour
    {
        public Vector3Spring spring;
        public Space space = Space.Self;
        public Vector3 goalPosition;
        public Transform goal;

        Vector3 Goal => goal == null ? goalPosition : (space == Space.Self ? goal.localPosition : goal.position);

        private void Start()
        {
            spring.OnPreUpdate += (x) => spring.goal = Goal;
            if (space == Space.Self)
            {
                spring.OnPreUpdate += (x) => spring.Current = transform.localPosition;
                spring.OnUpdated += (x) => transform.localPosition = spring.Current;
            }
            else
            {
                spring.OnPreUpdate += (x) => spring.Current = transform.position;
                spring.OnUpdated += (x) => transform.position = spring.Current;
            }
            
            spring.Start();
        }

        private void OnDestroy()
        {
            spring.Dispose();
        }
    }

}
