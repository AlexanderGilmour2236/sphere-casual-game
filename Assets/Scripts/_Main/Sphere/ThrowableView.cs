using System;
using misc;
using UnityEngine;

namespace sphereGame.sphere
{
    public class ThrowableView : MonoBehaviour, IPoolObject
    {
        [SerializeField] private InflateComponent _inflateComponent;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _sphereSpeed;
        [SerializeField] private float _maxVelocity = 1;

        private bool _isThrown;
        private bool _isHitObstacle;

        private void FixedUpdate()
        {
            if (_isThrown && !_isHitObstacle)
            {
                _rigidbody.AddForce(Vector3.forward * _sphereSpeed, ForceMode.VelocityChange);
                _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _maxVelocity);
            }
        }

        public void throwForward()
        {
            _isThrown = true;
        }

        public void throwAtObstacle(Transform obstacleTransform)
        {
            _isHitObstacle = true;
            Vector3 forceToObstacle = Vector3.down * _rigidbody.velocity.magnitude * 0.5f;
            Vector3 direction = obstacleTransform.position - transform.position;
            forceToObstacle += direction * _sphereSpeed;

            _rigidbody.AddForce(forceToObstacle, ForceMode.VelocityChange);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isHitObstacle && other.CompareTag(SphereGameTags.OBSTACLE_TAG))
            {
                ThrowableView throwableView = other.GetComponentInParent<ThrowableView>();
                if (!throwableView.isHitObstacle)
                {
                    _isHitObstacle = true;
                }
            }
        }

        public InflateComponent inflateComponent
        {
            get { return _inflateComponent; }
        }

        public bool isThrown
        {
            get { return _isThrown; }
        }

        public bool isHitObstacle
        {
            get { return _isHitObstacle; }
        }

        public void onSpawn()
        {
        }

        public void onRelease()
        {
            _rigidbody.velocity = Vector3.zero;
            _isThrown = false;
            _isHitObstacle = false;
        }
    }
}