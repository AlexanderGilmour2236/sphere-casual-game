using UnityEngine;

namespace sphereGame.camera
{
    public class CameraController
    {
        private const float CAMERA_LERP_SPEED = 0.1f;
        
        private readonly Camera _camera;
        private readonly Vector3 _relativeOffset;
        private Transform _target;
        private float _offsetScale;
        private float _minOffsetScale = 3;
        private float _minYPosition = 3;
        
        public CameraController(Camera camera, Vector3 relativeOffset)
        {
            _camera = camera;
            _relativeOffset = relativeOffset;
        }

        public void setOffsetScale(float scale)
        {
            _offsetScale = scale;
        }

        public void setTarget(Transform target)
        {
            _target = target;
        }

        public void tick()
        {
            float offsetScale = Mathf.Clamp(_offsetScale, _minOffsetScale, _offsetScale);
            if (_target != null && _target.gameObject.activeInHierarchy)
            {
                Vector3 cameraPosition = _camera.transform.position;
                Vector3 cameraNextPosition = Vector3.Lerp(cameraPosition, _target.transform.position + _relativeOffset * offsetScale, CAMERA_LERP_SPEED);
                
                cameraNextPosition.y = Mathf.Clamp(cameraNextPosition.y, _minYPosition, float.MaxValue);
                
                _camera.transform.position = cameraNextPosition;
            }
        }
    }
}