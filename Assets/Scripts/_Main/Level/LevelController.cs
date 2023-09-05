﻿using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace sphereGame.level
{
    public class LevelController
    {
        private const float DISTANCE_TO_OPEN_DOOR = 20;
        private const float DISTANCE_TO_WIN_LEVEL = 0.5f;

        private readonly LevelCollectionData _levelCollectionData;
        
        private DoorView _doorView;
        private Transform _playerTransform;
        private readonly Transform _levelPatternPosition;
        private LevelPatternView _currentLevelPatternView;


        public LevelController(LevelCollectionData levelCollectionData, Transform levelPatternPosition)
        {
            _levelCollectionData = levelCollectionData;
            _levelPatternPosition = levelPatternPosition;
        }

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

        public LevelPatternView loadLevel(LevelPatternView levelPatternPrefab)
        {
            if (_currentLevelPatternView != null)
            {
                Object.Destroy(_currentLevelPatternView.gameObject);
                _currentLevelPatternView = null;
            }

            _currentLevelPatternView = Object.Instantiate(levelPatternPrefab, _levelPatternPosition);
            _doorView = _currentLevelPatternView.doorView;
            
            return _currentLevelPatternView;
        }
        
        public void changeDoorSize(float sphereSize)
        {
            if (_doorView != null)
            {
                _doorView.setDoorScale(sphereSize);
            }
        }

        public void dispose()
        {
            if (_currentLevelPatternView != null)
            {
                Object.Destroy(_currentLevelPatternView.gameObject);
                _currentLevelPatternView = null;
            }
        }

        public LevelPatternData getLevelPattern(int currentLevelIndex)
        {
            return _levelCollectionData.levelPatternsCollection[
                    currentLevelIndex % _levelCollectionData.levelPatternsCollection.Count];
        }
    }
}