﻿using System.Collections.Generic;
using UnityEngine;

namespace sphereGame
{
    public class SphereController
    {
        private const float MIN_THROW_SIZE = 1;
        
        private SphereViewFactory _sphereViewFactory;

        private bool _isTapDown;
        private SphereData _currentSphereData;
        private PlayerSphereView _currentPlayerSphereView;

        private SphereControllerState _currentSphereControllerState;
        private ThrowableView _currentThrowableView;
        private float _currentThrowableScale = 0;
        private float _playerSphereScale;
        
        private List<ObstacleView> _currentObstacles;
        private Transform _levelStartPoint;

        public SphereController(SphereViewFactory sphereViewFactory)
        {
            _sphereViewFactory = sphereViewFactory;
        }

        public void init(SphereData sphereData, Transform levelStartPoint, List<ObstacleView> currentObstacles)
        {
            _currentSphereData = sphereData;
            _playerSphereScale = _currentSphereData.startScale;
            
            _currentPlayerSphereView = _sphereViewFactory.getPlayerSphereView(_currentSphereData);
            _currentPlayerSphereView.transform.position = levelStartPoint.position;
            
            _currentObstacles = currentObstacles;
            _levelStartPoint = levelStartPoint;
        }

        public void start()
        {
            _currentSphereControllerState = SphereControllerState.IDLE;
            updatePlayerSpherePosition();            
        }

        public void tick()
        {
            switch (_currentSphereControllerState)
            {
                case SphereControllerState.IDLE:
                    if (_isTapDown)
                    {
                        startInflatingThrowableSphere();
                    }
                    break;
                case SphereControllerState.INFLATING_THROWABLE:
                    if (_isTapDown || _currentThrowableScale < MIN_THROW_SIZE)
                    {
                        chargeShot();
                    }
                    else
                    {
                        throwSphere();
                        setSphereControllerState(SphereControllerState.IDLE);
                    }
                    break;
                case SphereControllerState.SPHERE_RUN_OUT:
                    if (!_currentThrowableView.isThrowed)
                    {
                        throwSphere();
                    }
                    break;
            }
        }

        private void throwSphere()
        {
            _currentThrowableView.transform.position =
                getScaleRelatedPosition(_currentThrowableView.transform, _currentThrowableScale);
            updatePlayerSpherePosition();

            _currentThrowableView.throwObject();
        }

        private void startInflatingThrowableSphere()
        {
            _currentThrowableView = _sphereViewFactory.getThrowableSphereView();
            _currentThrowableScale = 0;
                        
            _currentThrowableView.inflateComponent.setSize(_currentThrowableScale, true);
            setSphereControllerState(SphereControllerState.INFLATING_THROWABLE);
        }

        private void chargeShot()
        {
            float scaleInflationInFrame = Time.deltaTime * _currentSphereData.inflateMultiplier;

            _currentThrowableScale += scaleInflationInFrame;
            
            inflatePlayerSphere(-scaleInflationInFrame);
            _currentThrowableView.inflateComponent.addSize(scaleInflationInFrame);

            Vector3 playerSpherePosition = updatePlayerSpherePosition();

            Vector3 throwablePosition = playerSpherePosition;
            throwablePosition.z += _playerSphereScale * 0.5f + _currentThrowableScale * 0.5f + _playerSphereScale * 0.5f;
            
            Transform throwableTransform = _currentThrowableView.transform;
            throwableTransform.position = throwablePosition;
            
            throwableTransform.position =
                getScaleRelatedPosition(throwableTransform, _currentThrowableScale);
            
            if (_playerSphereScale <= 0)
            {
                setSphereControllerState(SphereControllerState.SPHERE_RUN_OUT);
            }
        }

        private void setSphereControllerState(SphereControllerState state)
        {
            _currentSphereControllerState = state;
            switch (state)
            {
                case SphereControllerState.SPHERE_RUN_OUT:
                    onTapUp();
                    break;
            }
        }

        // <summary>

        /// updates player's sphere position related to collider size, so it's always on the ground, returns updated sphere position
        /// </summary>
        private Vector3 updatePlayerSpherePosition()
        {
            Transform playerSphereTransform = _currentPlayerSphereView.transform;
            Vector3 playerSpherePosition = getScaleRelatedPosition(playerSphereTransform, _playerSphereScale);
            playerSphereTransform.position = playerSpherePosition;
            return playerSpherePosition;
        }

        public Vector3 getScaleRelatedPosition(Transform transform, float scale)
        {
            Vector3 scaleRelatedPosition = transform.position;
            scaleRelatedPosition.y = _levelStartPoint.transform.position.y + scale * 0.5f;
            return scaleRelatedPosition;
        }

        private void inflatePlayerSphere(float inflation)
        {
            _playerSphereScale += inflation;
            _playerSphereScale = Mathf.Clamp(_playerSphereScale, 0, _currentSphereData.startScale);
            _currentPlayerSphereView.inflateComponent.setSize(_playerSphereScale);
            updatePlayerSpherePosition();
        }

        public PlayerSphereView currentPlayerSphereView
        {
            get { return _currentPlayerSphereView; }
        }

        public void onTapDown()
        {
            _isTapDown = true;
        }

        public void onTapUp()
        {
            _isTapDown = false;
        }
    }
}