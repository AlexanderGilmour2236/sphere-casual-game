using System;
using UnityEngine;

namespace sphereGame.sphere
{
    [Serializable]
    public class SphereData
    {
        [SerializeField] private float _startScale = 11.5f;
        [SerializeField] private float _inflateMultiplier = 1.0f;

        public float startScale
        {
            get { return _startScale; }
        }

        public float inflateMultiplier
        {
            get { return _inflateMultiplier; }
        }
    }
}