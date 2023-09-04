using System;
using UnityEngine;

namespace sphereGame.level
{
    public class DoorView : MonoBehaviour
    {
        [SerializeField] private Transform _scaleTransform;
        [SerializeField] private Transform _rotateTransform;
        [SerializeField] private float _scaleLerpSpeed;
        [SerializeField] private float _rotationLerpSpeed;
        [SerializeField] private float _openDoorYRotation = -130;


        private Vector3 _targetScale = Vector3.one;
        private Quaternion _targetRotation;
        private bool _isDoorOpened;

        public void setDoorScale(float scale)
        {
            _targetScale = new Vector3(scale, scale, scale);
        }

        public void openTheDoor()
        {
            if (!_isDoorOpened)
            {
                _isDoorOpened = true;
                _targetRotation = Quaternion.Euler(0, _openDoorYRotation, 0);
            }
        }

        private void Update()
        {
            _scaleTransform.localScale = Vector3.Lerp(_scaleTransform.localScale, _targetScale, _scaleLerpSpeed);
            _rotateTransform.rotation = Quaternion.Lerp(_rotateTransform.rotation, _targetRotation, _rotationLerpSpeed);
        }
    }
}