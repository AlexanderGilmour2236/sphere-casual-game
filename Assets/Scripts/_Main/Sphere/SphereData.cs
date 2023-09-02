using System;
using UnityEngine;

namespace sphereGame
{
    [Serializable]
    public class SphereData
    {
        [SerializeField] private float _startScale = 11.5f;
        [SerializeField] private float _inflateMultiplier = 0.3f;
    }
}