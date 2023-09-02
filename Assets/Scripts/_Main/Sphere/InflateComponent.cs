using System;
using UnityEngine;

namespace sphereGame
{
    public class InflateComponent : MonoBehaviour
    {
        [SerializeField] private float _currentTargetScale = 11.5f;
        [SerializeField] private Transform _sphereTransform;
        [SerializeField] private float _inflateLerpSpeed;

        private void Update()
        {
            Vector3 targetScale = new Vector3(_currentTargetScale, _currentTargetScale, _currentTargetScale);
            Debug.Log(Mathf.Approximately(_sphereTransform.localScale.magnitude, targetScale.magnitude));
            if (!Mathf.Approximately(_sphereTransform.localScale.magnitude, targetScale.magnitude))
            {
                _sphereTransform.localScale = Vector3.Lerp(_sphereTransform.localScale, targetScale, _inflateLerpSpeed);
            }
            
            
        }

        public void addSphereSize(float addedSphereSize)
        {
            _currentTargetScale += addedSphereSize;
        }
    }
}