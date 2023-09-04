using System;
using System.Collections;
using System.Collections.Generic;
using misc;
using UnityEngine;

namespace sphereGame.sphere
{
    public class SphereController
    {
        private const float MIN_THROW_SIZE = 1;
        private const float DELAY_TO_DESTROY_THROWABLE = 1.0f;

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
                    break;
                case SphereControllerState.INFLATING_THROWABLE:
                    if (_isTapDown)
                    {
                        chargeShot();
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
                        throwSphere();
                        setSphereControllerState(SphereControllerState.IDLE);
                    }
                    break;
                case SphereControllerState.SPHERE_RUN_OUT:
                    if (!_currentThrowableView.isThrown)
                    {
                        throwableRunOut?.Invoke(_currentThrowableView);
                        throwSphere();
                    }
                    break;
            }

            checkCanSphereMove();
        }

        private void checkCanSphereMove()
        {
            Vector3 playerSpherePosition = _currentPlayerSphereView.transform.position;
            
            RaycastHit raycastHit;
            if(Physics.CapsuleCast(playerSpherePosition, playerSpherePosition, 
                _playerSphereScale, Vector3.forward, out raycastHit, 
                1 << LayerMask.NameToLayer(SphereGameTags.OBSTACLE_LAYER)))
            {
                float distanceToPlayerSphere = Vector3.Distance(playerSpherePosition, raycastHit.point);
                if (distanceToPlayerSphere >= _playerSphereScale * 2)
                {
                    _currentPlayerSphereView.transform.Translate(Vector3.forward * Time.deltaTime * 3);
                }
            }
            else
            {
                _currentPlayerSphereView.transform.Translate(Vector3.forward * Time.deltaTime * 3);
            }
        }


        private void throwSphere()
        {
            _currentPlayerSphereView.playThrowEffect();
            _currentThrowableView.transform.position =
                getScaleRelatedPositionOnLevel(_currentThrowableView.transform.position, _currentThrowableScale);

            _currentThrowableView.throwForward();
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
            
            inflatePlayerSphere(-scaleInflationInFrame);
            _currentThrowableView.inflateComponent.addSize(scaleInflationInFrame);

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
            Transform playerSphereTransform = _currentPlayerSphereView.transform;
            Vector3 playerSpherePosition = getScaleRelatedPositionOnLevel(playerSphereTransform.position, 
                _currentPlayerSphereView.inflateComponent.currentScale * 0.5f);
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
            _playerSphereScale += inflation;
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