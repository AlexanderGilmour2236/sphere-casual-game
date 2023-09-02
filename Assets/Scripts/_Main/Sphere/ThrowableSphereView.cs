using System;
using UnityEngine;

namespace sphereGame
{
    public class ThrowableSphereView : MonoBehaviour
    {
        [SerializeField] private InflateComponent _inflateComponent;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _sphereSpeed;
        [SerializeField] private float _maxVelocity = 1;

        private bool _isThrowed;

        private void FixedUpdate()
        {
            if (_isThrowed)
            {
                _rigidbody.AddForce(Vector3.forward * _sphereSpeed, ForceMode.VelocityChange);
                _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _maxVelocity);
            }
        }

        public InflateComponent inflateComponent
        {
            get { return _inflateComponent; }
        }

        public void throwSphere()
        {
            _isThrowed = true;
        }
    }
}