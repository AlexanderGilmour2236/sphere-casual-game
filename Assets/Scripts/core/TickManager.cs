using System;
using UnityEngine;

namespace tuesdayPizza
{
    public class TickManager : MonoBehaviour
    {
        private static TickManager _instance;
        
        public event Action tick; 
        public event Action fixedTick;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
                _instance = this;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            tick?.Invoke();
        }

        private void FixedUpdate()
        {
            fixedTick?.Invoke();
        }

        public static TickManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject(nameof(TickManager)).AddComponent<TickManager>();
                }
                return _instance;
            }
        }
    }
}