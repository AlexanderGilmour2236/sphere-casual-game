using UnityEngine;

namespace sphereGame
{
    public class InflateComponent : MonoBehaviour
    {
        [SerializeField] private float _currentTargetScale = 11.5f;
        [SerializeField] private Transform _inflateTransform;
        [SerializeField] private float _inflateLerpSpeed;

        private void Update()
        {
            Vector3 targetScale = getVectorFromFloat(_currentTargetScale);

            if (!Mathf.Approximately(_inflateTransform.localScale.magnitude, targetScale.magnitude))
            {
                _inflateTransform.localScale = Vector3.Lerp(_inflateTransform.localScale, targetScale, _inflateLerpSpeed);
            }
        }

        public void addSize(float addedSize)
        {
            _currentTargetScale += addedSize;
        }

        public void setSize(float scale)
        {
            _currentTargetScale = scale;
            _inflateTransform.localScale = getVectorFromFloat(_currentTargetScale);
        }

        private Vector3 getVectorFromFloat(float scaleMagnitude)
        {
            return new Vector3(scaleMagnitude, scaleMagnitude, scaleMagnitude);
        }
    }
}