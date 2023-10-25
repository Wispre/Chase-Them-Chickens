using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arugula.Animation
{
    [Serializable]
    public class Vector3Spring : Spring, ISpring<Vector3>
    {
        public Vector3 goal;
        public Vector3 Current
        {
            get
            {
                return current;
            }
            set
            {
                current = value;
            }
        }

        public Vector3 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }

        Vector3 direction;
        Vector3 velocity;
        Vector3 current;

        public override void Step(float deltaTime, int subSteps = 1)
        {
            if (disposed)
                return;

            deltaTime /= subSteps;

            for (int i = 0; i < subSteps; i++)
            {
                direction = (current - goal).normalized;

                Vector3 f = -direction * strength - damper * velocity;

                if (Vector3.Distance(goal, current + (velocity * deltaTime)) > threshold)
                {
                    velocity += f * deltaTime;
                }
                else
                {
                    velocity = Vector3.MoveTowards(velocity, Vector3.zero, deltaTime * 5);
                    if (velocity.sqrMagnitude == 0)
                        current = goal;
                }
                current += velocity * deltaTime;
            }
        }
    }
}

