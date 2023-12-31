﻿using System;
using System.Collections;
using System.Collections.Generic;
using misc;
using UnityEngine;
using Object = UnityEngine.Object;

namespace sphereGame.sphere
{
    public class SphereController
    {
        private const float MIN_PLAYER_SPHERE_SIZE = 0.5f;
        private const float MIN_THROW_SIZE = 1;
        private const float DELAY_TO_DESTROY_THROWABLE = 1.0f;
        private const float MOVE_RAYCAST_DISTANCE = 50.0f;

        private SphereViewFactory _sphereViewFactory;

        private bool _isTapDown;
        private SphereData _currentSphereData;
        private PlayerSphereView _currentPlayerSphereView;

        private SphereControllerState _currentSphereControllerState;
        private ThrowableView _currentThrowableView;
        private float _currentThrowableScale = 0;
        private float _playerSphereScale;
        
        private Transform _levelStartPoint;
        private MonoPool<ThrowableView> _throwableViewsPool;
        private Dictionary<ThrowableView, Coroutine> _throwableToAutoDestroyCoroutines = new Dictionary<ThrowableView, Coroutine>();

        public event Action<ThrowableView> throwableRunOut;
        public event Action<float> playerSphereSizeChange;
        
        public SphereController(SphereViewFactory sphereViewFactory)
        {
            _sphereViewFactory = sphereViewFactory;
        }

        public void init(SphereData sphereData, Transform levelStartPoint)
        {
            _currentSphereData = sphereData;
            _playerSphereScale = _currentSphereData.startScale;
            
            _currentPlayerSphereView = _sphereViewFactory.getPlayerSphereView(_currentSphereData);
            _currentPlayerSphereView.transform.position = levelStartPoint.position;
            
            _levelStartPoint = levelStartPoint;
            _throwableViewsPool = new MonoPool<ThrowableView>(null, _sphereViewFactory.getThrowableSphereView);
        }

        public void start()
        {
            _currentSphereControllerState = SphereControllerState.IDLE;
            updatePlayerSphereYPosition();            
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
                    checkCanSphereMove();
                    updatePlayerSphereYPosition();
                    break;
                case SphereControllerState.INFLATING_THROWABLE:
                    if (_isTapDown)
                    {
                        chargeShot();
                        checkCanSphereMove();
                        checkSphereRunOut();
                    }
                    else
                    {
                        if (_currentThrowableScale < MIN_THROW_SIZE)
                        {
                            float difference = MIN_THROW_SIZE - _currentThrowableScale;
                            _currentThrowableScale = Mathf.Clamp(_currentThrowableScale, MIN_THROW_SIZE, _currentThrowableScale);
                            _currentThrowableView.inflateComponent.setSize(_currentThrowableScale);
                            
                            inflatePlayerSphere(-difference);
                        }

                        setSphereControllerState(SphereControllerState.IDLE);
                        throwSphere();
                    }
                    break;
                case SphereControllerState.SPHERE_RUN_OUT:
                    if (!_currentThrowableView.isThrown)
                    {
                        throwSphere();
                    }
                    throwableRunOut?.Invoke(_currentThrowableView);
                    break;
            }
        }

        private void checkSphereRunOut()
        {
            if (_playerSphereScale < MIN_PLAYER_SPHERE_SIZE)
            {
                setSphereControllerState(SphereControllerState.SPHERE_RUN_OUT);
            }
        }

        private void checkCanSphereMove()
        {
            Vector3 playerSpherePosition = _currentPlayerSphereView.transform.position;
            float capsuleRadius = _playerSphereScale * 0.5f;

            Vector3 capsuleCastStart = playerSpherePosition;
            Vector3 capsuleCastEnd = playerSpherePosition ;

            RaycastHit raycastHit;
            if (Physics.CapsuleCast(capsuleCastStart, capsuleCastEnd, capsuleRadius, 
                Vector3.forward, out raycastHit, MOVE_RAYCAST_DISTANCE, 
                1 << LayerMask.NameToLayer(SphereGameTags.OBSTACLE_LAYER)))
            {
                float distanceToObstacle = raycastHit.distance;
                if (distanceToObstacle >= _playerSphereScale * 2)
                {
                    moveSphere(_currentSphereData.movementSpeed);
                }
            }
            else
            {
                moveSphere(_currentSphereData.movementSpeed);
            }
        }
        

        private void moveSphere(float movementSpeed)
        {
            _currentPlayerSphereView.transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);
        }


        private void throwSphere()
        {
            _currentPlayerSphereView.playThrowEffect();
            updatePlayerSphereYPosition();
            _currentThrowableView.transform.position =
                getScaleRelatedPositionOnLevel(_currentThrowableView.transform.position, _currentThrowableScale);

            _currentThrowableView.throwForward();
            setSphereControllerState(SphereControllerState.IDLE);
            
            checkSphereRunOut();
        }

        private void startInflatingThrowableSphere()
        {
            _currentThrowableView = _throwableViewsPool.GetObject();
            _currentThrowableScale = 0;
                        
            _currentThrowableView.inflateComponent.setSize(_currentThrowableScale, true);
            setSphereControllerState(SphereControllerState.INFLATING_THROWABLE);
        }

        private void chargeShot()
        {
            float scaleInflationInFrame = Time.deltaTime * _currentSphereData.inflateMultiplier;

            _currentThrowableScale += scaleInflationInFrame;
            
            _currentThrowableView.inflateComponent.addSize(scaleInflationInFrame);
            
            inflatePlayerSphere(-scaleInflationInFrame);
            
            Vector3 playerSpherePosition = updatePlayerSphereYPosition();

            float forwardOffset = _playerSphereScale * 0.5f + _currentThrowableScale * 0.5f + _playerSphereScale * 0.5f;
            Vector3 throwablePosition = playerSpherePosition;
            throwablePosition.z += forwardOffset;
            
            Transform throwableTransform = _currentThrowableView.transform;
            
            throwableTransform.position =
                getScaleRelatedPositionOnLevel(throwablePosition, _currentThrowableScale);
            

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
        private Vector3 updatePlayerSphereYPosition()
        {
            Debug.Log(_currentPlayerSphereView.inflateComponent.currentScale);
            float playerSphereCurrentScale = _currentPlayerSphereView.inflateComponent.currentScale;
            
            Transform playerSphereTransform = _currentPlayerSphereView.transform;
            Vector3 playerSpherePosition = getScaleRelatedPositionOnLevel(playerSphereTransform.position, 
                playerSphereCurrentScale * 0.5f);
            playerSphereTransform.position = playerSpherePosition;
            
            return playerSpherePosition;
        }

        public Vector3 getScaleRelatedPositionOnLevel(Vector3 position, float scale)
        {
            Vector3 scaleRelatedPosition = position;
            scaleRelatedPosition.y = _levelStartPoint.transform.position.y + scale * 0.5f;
            return scaleRelatedPosition;
        }

        private void inflatePlayerSphere(float inflation)
        {
            if (!Mathf.Approximately(_playerSphereScale, MIN_PLAYER_SPHERE_SIZE) && _playerSphereScale + inflation < MIN_PLAYER_SPHERE_SIZE)
            {
                _playerSphereScale = MIN_PLAYER_SPHERE_SIZE;
                throwSphere();
                onTapUp();
            }
            else
            {
                _playerSphereScale += inflation;
            }
            _playerSphereScale = Mathf.Clamp(_playerSphereScale, 0, _currentSphereData.startScale);
            _currentPlayerSphereView.inflateComponent.setSize(_playerSphereScale);
            updatePlayerSphereYPosition();
            playerSphereSizeChange?.Invoke(_playerSphereScale);
        }

        public void autoDestroyThrowable(ThrowableView throwableView)
        {
            _throwableToAutoDestroyCoroutines.Add(throwableView,
                CoroutineRunner.instance.startCoroutine(returnThrowableToPool(throwableView, DELAY_TO_DESTROY_THROWABLE)));
        }

        private IEnumerator returnThrowableToPool(ThrowableView throwableView, float delay)
        {
            yield return new WaitForSeconds(delay);
            _throwableViewsPool.ReleaseObject(throwableView);
            _throwableToAutoDestroyCoroutines.Remove(throwableView);
        }

        public void dispose()
        {
            _currentSphereData = null;
            _currentThrowableScale = 0;
            _playerSphereScale = 0;

            foreach (KeyValuePair<ThrowableView,Coroutine> throwableToAutoDestroyCoroutine in _throwableToAutoDestroyCoroutines)
            {
                CoroutineRunner.instance.stopCoroutine(throwableToAutoDestroyCoroutine.Value);
            }
            _throwableToAutoDestroyCoroutines.Clear();
            
            _throwableViewsPool.ReleaseObject(_currentThrowableView);
            _currentThrowableView = null;

            _throwableViewsPool.Dispose();
            _currentSphereControllerState = SphereControllerState.NONE;
            
            Object.Destroy(_currentPlayerSphereView.gameObject);
        }

        public void onTapDown()
        {
            _isTapDown = true;
        }

        public void onTapUp()
        {
            _isTapDown = false;
        }

        public PlayerSphereView currentPlayerSphereView
        {
            get { return _currentPlayerSphereView; }
        }

        public float playerSphereScale
        {
            get { return _playerSphereScale; }
        }
    }
}