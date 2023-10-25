using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arugula.Animation
{
    [Serializable]
    public class ValueSpring : Spring, ISpring<float>
    {
        public float goal;
        public float Current
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

        public float Velocity
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

        float direction;
        float velocity;
        float current;

        public override void Step(float deltaTime, int subSteps = 1)
        {
            if (disposed)
                return;

            deltaTime /= subSteps;

            for (int i = 0; i < subSteps; i++)
            {
                direction = (current - goal) > 0 ? 1 : -1;

                float f = -direction * strength - damper * velocity;

                if (Mathf.Abs(goal - (current + (velocity * deltaTime))) > threshold)
                {
                    velocity += f * deltaTime;
                }
                else
                {
                    velocity = Mathf.MoveTowards(velocity, 0, deltaTime * 5);
                }
                current += velocity * deltaTime;
            }
        }
    }
}
