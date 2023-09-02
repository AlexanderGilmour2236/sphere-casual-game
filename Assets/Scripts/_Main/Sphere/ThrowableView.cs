﻿using System;
using UnityEngine;

namespace sphereGame
{
    public class ThrowableView : MonoBehaviour
    {
        [SerializeField] private InflateComponent _inflateComponent;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _sphereSpeed;
        [SerializeField] private float _maxVelocity = 1;

        private bool _isThrowed;
        private bool _isHitObstacle;

        private void FixedUpdate()
        {
            if (_isThrowed && !_isHitObstacle)
            {
                _rigidbody.AddForce(Vector3.forward * _sphereSpeed, ForceMode.VelocityChange);
                _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _maxVelocity);
            }
        }

        public void throwObject()
        {
            _isThrowed = true;
        }

        public InflateComponent inflateComponent
        {
            get { return _inflateComponent; }
        }

        public bool isThrowed
        {
            get { return _isThrowed; }
        }

        public void onHitObstacle()
        {
            _isHitObstacle = true;
        }
    }
}