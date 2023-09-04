using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace sphereGame.level
{
    public class LevelController
    {
        private const float DISTANCE_TO_OPEN_DOOR = 20;
        private const float DISTANCE_TO_WIN_LEVEL = 0.5f;
        
        private DoorView _doorView;
        private Transform _playerTransform;
        
        public event Action playerReachedDoorPosition;

        public void setPlayerTransform(Transform transform)
        {
            _playerTransform = transform;
        }

        public void tick()
        {
            float distanceToPlayer = Vector3.Distance(_doorView.transform.position, _playerTransform.position);
            
            if (_playerTransform.transform.position.z >= _doorView.transform.position.z)
            {
                playerReachedDoorPosition?.Invoke();
            }
            else if (distanceToPlayer <= DISTANCE_TO_OPEN_DOOR)
            {
                _doorView.openTheDoor();
            }
        }

        public void loadLevel()
        {
            _doorView = Object.FindObjectOfType<DoorView>();
        }
        
        public void changeDoorSize(float sphereSize)
        {
            if (_doorView != null)
            {
                _doorView.setDoorScale(sphereSize);
            }
        }
    }
}