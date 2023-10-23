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

#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(-500)]
public class UnityEventDispatcher : MonoBehaviour
{
    //automatically create UnityEventDispatcher when invoked in any way
    static UnityEventDispatcher()
    {
        //Clean up our instance while inside the Editor to prevent runtime errors
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += (x) =>
        {
            if (x == PlayModeStateChange.ExitingPlayMode)
            {
                if (instance != null)
                    Destroy(instance);
            }
        };
#endif

        GameObject go = new GameObject("UnityEventDispatcher", typeof(UnityEventDispatcher));
        //hide our shameful disregard of Unity best practices
        go.hideFlags = HideFlags.HideAndDontSave;
    }

    static UnityEventDispatcher instance;

    //main event pumps
    public static event Action OnUpdate = delegate { };
    public static event Action OnFixedUpdate = delegate { };
    public static event Action OnLateUpdate = delegate { };

    //global coroutine helper function



    public static Coroutine StartGlobalCoroutine(IEnumerator coroutine) => instance.StartCoroutine(coroutine);

    private void Update() => OnUpdate();
    private void FixedUpdate() => OnFixedUpdate();
    private void LateUpdate() => OnLateUpdate();

    private void Awake()
    {
        //ensure only one instance exists
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}


