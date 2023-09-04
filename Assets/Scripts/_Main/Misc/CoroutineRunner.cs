using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace misc
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;
        
        public static CoroutineRunner instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }

        public Coroutine startCoroutine(IEnumerator coroutineMethod)
        {
            return StartCoroutine(coroutineMethod);
        }

        public void stopCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
    }
}