using System.Collections;
using System.Collections.Generic;
using misc;
using UnityEngine;

namespace sphereGame.sphere
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
        
        private Transform _levelStartPoint;
        private MonoPool<ThrowableView> _throwableViewsPool;
        private Dictionary<ThrowableView, Coroutine> _throwableToAutoDestroyCoroutines = new Dictionary<ThrowableView, Coroutine>();
        
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
                    if (!_currentThrowableView.isThrown)
                    {
                        throwSphere();
                    }
                    break;
            }
        }

        private void throwSphere()
        {
            _currentThrowableView.transform.position =
                getScaleRelatedPositionOnLevel(_currentThrowableView.transform.position, _currentThrowableScale);
            updatePlayerSpherePosition();

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

            Vector3 playerSpherePosition = updatePlayerSpherePosition();

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
        private Vector3 updatePlayerSpherePosition()
        {
            Transform playerSphereTransform = _currentPlayerSphereView.transform;
            Vector3 playerSpherePosition = getScaleRelatedPositionOnLevel(playerSphereTransform.position, _playerSphereScale);
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
            updatePlayerSpherePosition();
        }

        public void autoDestroyThrowable(ThrowableView throwableView)
        {
            _throwableToAutoDestroyCoroutines.Add(throwableView,
                CoroutineRunner.instance.startCoroutine(returnThrowableToPool(throwableView, 3.0f)));
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