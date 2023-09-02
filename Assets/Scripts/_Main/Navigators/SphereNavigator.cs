using System;
using System.Collections.Generic;
using tuesdayPizza;
using UnityEngine;

namespace sphereGame
{
    public class SphereNavigator : Navigator, IInputListener
    {
        private const float MIN_THROW_SIZE = 1;

        private readonly SGSceneAccessor _sceneAccessor;
        private readonly InputSystem _inputSystem;
        private readonly SphereViewFactory _sphereViewFactory;

        private bool _isTapDown;
        private SphereData _currentSphereData;
        private PlayerSphereView _currentPlayerSphereView;

        private SphereNavigatorState _currentSphereNavigatorState;
        private ThrowableView _currentThrowableView;
        private float _currentThrowableScale = 0;
        private float _currentSphereScale;
        private List<ObstacleView> _currentObstacles;

        public SphereNavigator(SGSceneAccessor sceneAccessor, Navigator parent) : base(parent)
        {
            _sceneAccessor = sceneAccessor;
            _sphereViewFactory = sceneAccessor.sphereViewFactory;
            _inputSystem = sceneAccessor.inputSystem;
            _inputSystem.addInputListener(this);
        }

        public override void go()
        {
            base.go();
            setSphereNavigatorState(SphereNavigatorState.IDLE);

            _currentSphereData = getNewSphereData();
            _currentSphereScale = _currentSphereData.startScale;
            _currentPlayerSphereView = _sphereViewFactory.getPlayerSphereView(_currentSphereData);
            _currentPlayerSphereView.transform.position = _sceneAccessor.levelStartPoint.position;
            updatePlayerSpherePosition();
            _currentObstacles = new List<ObstacleView>(_sceneAccessor.obstacleViews);
            initObstacles(_currentObstacles);
        }

        private void initObstacles(List<ObstacleView> obstacleViews)
        {
            foreach (ObstacleView obstacleView in obstacleViews)
            {
                subscribeObstacleView(obstacleView);
            }
        }

        private void subscribeObstacleView(ObstacleView obstacleView)
        {
            obstacleView.obstacleCollided += onObstacleCollided;
        }
        
        private void unsubscribeObstacleView(ObstacleView obstacleView)
        {
            obstacleView.obstacleCollided -= onObstacleCollided;
        }

        private void onObstacleCollided(ObstacleView obstacleView, ThrowableView throwableView)
        {
            throwableView.onHitObstacle();
            applyThrowableToObstacle(throwableView, obstacleView);
            obstacleView.setObstacleMarked(_currentPlayerSphereView.markMaterial);
        }

        private void applyThrowableToObstacle(ThrowableView throwableView, ObstacleView appliedObstacleView)
        {
            foreach (ObstacleView obstacle in _currentObstacles)
            {
                if (Vector3.Distance(obstacle.transform.position, appliedObstacleView.transform.position) <=
                    throwableView.inflateComponent.currentTargetScale)
                {
                    appliedObstacleView.setObstacleMarked(_currentPlayerSphereView.markMaterial);
                }
            }
            GameObject.Destroy(throwableView.gameObject);
        }

        private void setSphereNavigatorState(SphereNavigatorState state)
        {
            _currentSphereNavigatorState = state;
            switch (state)
            {
                case SphereNavigatorState.SPHERE_RUN_OUT:
                    onTapUp();
                    break;
            }
        }

        private SphereData getNewSphereData()
        {
            return _sceneAccessor.defaultSphereData;
        }

        public override void tick()
        {
            base.tick();
            switch (_currentSphereNavigatorState)
            {
                case SphereNavigatorState.IDLE:
                    if (_isTapDown)
                    {
                        startInflatingThrowableSphere();
                    }
                    break;
                case SphereNavigatorState.INFLATING_THROWABLE:
                    if (_isTapDown || _currentThrowableScale < MIN_THROW_SIZE)
                    {
                        inflateThrowableSphere();
                    }
                    else
                    {
                        _currentThrowableView.throwObject();
                        setSphereNavigatorState(SphereNavigatorState.IDLE);
                    }
                    break;
                case SphereNavigatorState.SPHERE_RUN_OUT:
                    if (!_currentThrowableView.isThrowed)
                    {
                        _currentThrowableView.throwObject();
                    }
                    break;
            }

        }

        private void startInflatingThrowableSphere()
        {
            _currentThrowableView = _sphereViewFactory.getThrowableSphereView();
            _currentThrowableScale = 0;
                        
            _currentThrowableView.inflateComponent.setSize(_currentThrowableScale);
            setSphereNavigatorState(SphereNavigatorState.INFLATING_THROWABLE);
        }

        private void inflateThrowableSphere()
        {
            float inflationInFrame = Time.deltaTime * _currentSphereData.inflateMultiplier;

            _currentThrowableScale += inflationInFrame; 
            _currentSphereScale -= inflationInFrame;
                        
            _currentPlayerSphereView.inflateComponent.addSize(-inflationInFrame);
            _currentThrowableView.inflateComponent.addSize(inflationInFrame);

            Vector3 playerSpherePosition = updatePlayerSpherePosition();

            Vector3 throwablePosition = playerSpherePosition;
            throwablePosition.z += _currentSphereScale * 0.5f + _currentThrowableScale * 0.5f;
            _currentThrowableView.transform.position = throwablePosition;

            if (_currentSphereScale <= 0)
            {
                setSphereNavigatorState(SphereNavigatorState.SPHERE_RUN_OUT);
            }
        }

        /// <summary>
        /// updates player's sphere position related to collider size, so it's always on the ground, returns updated sphere position
        /// </summary>
        private Vector3 updatePlayerSpherePosition()
        {
            Transform playerSphereTransform = _currentPlayerSphereView.transform;
            Vector3 playerSpherePosition = playerSphereTransform.position;
            playerSpherePosition.y = _sceneAccessor.levelStartPoint.transform.position.y + _currentSphereScale * 0.5f;
            playerSphereTransform.position = playerSpherePosition;
            return playerSpherePosition;
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