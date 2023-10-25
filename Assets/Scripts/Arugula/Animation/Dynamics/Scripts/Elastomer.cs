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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arugula.Animation
{
    [DefaultExecutionOrder(-800)]
    public class Elastomer : MonoBehaviour
    {
        [ContextMenu("Build Chain From Children")]
        void BuildChainFromChildren()
        {
            chain.Clear();
            Transform t = transform;
            while (true)
            {
                chain.Add(t);
                if (t.childCount > 0)
                    t = t.GetChild(0);
                else
                    break;
            }
        }

        class Link
        {
            public Transform transform = null;
            public Link parent;
            public float length = 0;

            public Vector3 pos = Vector3.zero;
            public Vector3 prevPos = Vector3.zero;
            public Vector3 offset = Vector3.zero;
            public Vector3 initialLocalPosition = Vector3.zero;
            public Quaternion initialLocalRotation = Quaternion.identity;
        }

        public List<Transform> chain;

        [Range(0, 1)]
        public float Damping = 0.1f;
        [Range(0, 1)]
        public float Elasticity = 0.1f;
        [Range(0, 1)]
        public float PositionWeight = 1;

        public float UpdateRate = 90;

        public Vector3 ConstantForce;

        public Elastomer Parent;
        public bool attemptTimeNormalization;

        public event Action OnPreProcess = delegate { };
        public event Action OnUpdateLinks = delegate { };

        public void ArtificialMove(Vector3 offset)
        {
            artificialMove += offset;
        }

        public void ZeroOut()
        {
            deltaMove = Vector3.zero;
            prevPos = transform.position;

            foreach (var l in _chain)
            {
                l.prevPos = l.pos;
            }
        }

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
            ZeroOut();
        }


        List<Link> _chain = new List<Link>();
        Vector3 deltaMove = Vector3.zero;
        Vector3 artificialMove = Vector3.zero;
        Vector3 prevPos;
        float totalLength = 0;
        float nextUpdateTime;
        bool shouldUpdate = false;
        bool hasInitialized = false;

        private void OnEnable()
        {
            if (!hasInitialized)
                Init();

            prevPos = transform.position;
            deltaMove = Vector3.zero;

            if (Parent)
            {
                Parent.OnPreProcess += PreProcess;
                Parent.OnUpdateLinks += UpdateLinks;
            }
        }

        private void OnDisable()
        {
            if (Parent)
            {
                Parent.OnPreProcess -= PreProcess;
                Parent.OnUpdateLinks -= UpdateLinks;
            }
        }

        void Init()
        {
            hasInitialized = true;
            _chain.Clear();
            totalLength = 0;

            for (int i = 0; i < chain.Count; i++)
            {
                AddLink(chain[i], _chain.Count == 0 ? null : _chain[_chain.Count - 1], ref totalLength);
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying == false)
                return;

            for (int i = 0; i < _chain.Count; i++)
            {
                Gizmos.DrawWireSphere(_chain[i].pos, 0.1f);
                if (i < _chain.Count - 1)
                    Gizmos.DrawLine(_chain[i].pos, _chain[i + 1].pos);
            }
        }

        Link AddLink(Transform t, Link parent, ref float length)
        {
            Link l = new Link();
            l.transform = t;
            l.pos = l.transform.position;
            l.prevPos = l.pos;
            l.initialLocalPosition = t.localPosition;
            l.initialLocalRotation = t.localRotation;
            l.parent = parent;

            if (parent != null)
                length += (parent.pos - l.pos).magnitude;

            _chain.Add(l);
            return l;
        }

        private void Update()
        {
            if (Parent == null)
                PreProcess();
        }

        void PreProcess()
        {
            if (UpdateRate > 0)
            {
                if (Time.time >= nextUpdateTime)
                {
                    shouldUpdate = true;
                    ResetTransforms();
                    nextUpdateTime = Time.time + (1f / UpdateRate);
                }
            }
            else
            {
                ResetTransforms();
                shouldUpdate = true;
            }

            OnPreProcess();
        }

        void ResetTransforms()
        {
            foreach (var l in _chain)
            {
                l.transform.localPosition = l.initialLocalPosition;
                l.transform.localRotation = l.initialLocalRotation;
            }
        }

        private void LateUpdate()
        {
            if (Parent == null)
                UpdateLinks();
        }

        void UpdateLinks()
        {
            if (!shouldUpdate)
                return;

            shouldUpdate = false;

            deltaMove = transform.position - prevPos;


            prevPos = transform.position;

            float posWeight = (1 - PositionWeight);
            if (attemptTimeNormalization)
            {
                float norm = UpdateRate == 0 ? 90 : UpdateRate;
                float old = posWeight;
                //posWeight = (1f - (Mathf.InverseLerp(0, UpdateRate, 1f/Time.smoothDeltaTime) * PositionWeight));
                posWeight = (1f - (InverseLerpUnclamped(0, norm, 1f / Time.smoothDeltaTime) * PositionWeight));
                if (posWeight < 0)
                    posWeight = 0;
                if (posWeight > 10)
                    posWeight = 10;
            }

            Vector3 move = deltaMove * posWeight;

            move += artificialMove;
            artificialMove = Vector3.zero;

            for (int i = 0; i < _chain.Count; i++)
            {
                Link link = _chain[i];
                if (link.parent == null)
                {
                    link.prevPos = link.pos;
                    link.pos = link.transform.position;
                    continue;
                }

                Vector3 vec = link.pos - link.prevPos;

                link.prevPos = link.pos + move + ConstantForce;
                link.pos += vec * (1 - Damping) + move + ConstantForce;
            }


            for (int i = 1; i < _chain.Count; i++)
            {
                Link link = _chain[i];
                Link parent = link.parent;

                float restLen = (parent.transform.position - link.transform.position).magnitude;

                Matrix4x4 matrix = parent.transform.localToWorldMatrix;
                matrix.SetColumn(3, parent.pos);

                Vector3 restPos = matrix.MultiplyPoint3x4(link.transform.localPosition);

                Vector3 deltaPos = restPos - link.pos;
                link.pos += deltaPos * Elasticity;

                deltaPos = parent.pos - link.pos;
                float leng = deltaPos.magnitude;
                link.pos += deltaPos * ((leng - restLen) / leng);
            }

            deltaMove = Vector3.zero;

            for (int i = 1; i < _chain.Count; i++)
            {
                var link = _chain[i];
                var parent = link.parent;

                Vector3 vec = link.transform.localPosition;
                Vector3 vec2 = link.pos - parent.pos;

                Quaternion rot = Quaternion.FromToRotation(parent.transform.TransformDirection(vec), vec2);

                parent.transform.rotation = rot * parent.transform.rotation;
                link.transform.position = link.pos;
            }

            OnUpdateLinks();
        }

        //excerpt from mathemagic for slim anim package
        static float InverseLerpUnclamped(float from, float to, float value)
        {
            return (value - from) / (to - from);
        }
    }
}