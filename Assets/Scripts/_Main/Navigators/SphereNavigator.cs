using System;
using tuesdayPizza;
using UnityEngine;

namespace sphereGame
{
    public class SphereNavigator : Navigator, IInputListener
    {
        private readonly SGSceneAccessor _sceneAccessor;
        private readonly InputSystem _inputSystem;
        private readonly SphereViewFactory _sphereViewFactory;

        private bool _isTapDown;
        private SphereData _currentSphereData;
        private PlayerSphereView _currentPlayerSphereView;

        private SphereNavigatorState _currentSphereNavigatorState;
        private ThrowableSphereView _currentThrowableSphereView;
        private float _currentThrowableScale = 0;
        private float _currentSphereScale;

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
        }

        private void setSphereNavigatorState(SphereNavigatorState state)
        {
            _currentSphereNavigatorState = state;
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
                case SphereNavigatorState.NONE:
                    break;
                case SphereNavigatorState.IDLE:
                    if (_isTapDown)
                    {
                        _currentThrowableSphereView = _sphereViewFactory.getThrowableSphereView();
                        _currentThrowableScale = 0;
                        
                        _currentThrowableSphereView.inflateComponent.setSize(_currentThrowableScale);
                        setSphereNavigatorState(SphereNavigatorState.INFLATING_THROWABLE);
                    }
                    break;
                case SphereNavigatorState.INFLATING_THROWABLE:
                    if (_isTapDown)
                    {
                        float inflationInFrame = Time.deltaTime * _currentSphereData.inflateMultiplier;

                        _currentThrowableScale += inflationInFrame; 
                        _currentSphereScale -= inflationInFrame;
                        
                        _currentPlayerSphereView.inflateComponent.addSize(-inflationInFrame);
                        _currentThrowableSphereView.inflateComponent.addSize(inflationInFrame);

                        Transform playerSphereTransform = _currentPlayerSphereView.transform;
                        Vector3 playerSpherePosition = playerSphereTransform.position;
                        playerSpherePosition.y = _sceneAccessor.levelStartPoint.transform.position.y + _currentSphereScale * 0.5f;
                        playerSphereTransform.position = playerSpherePosition;
                        
                        Vector3 throwablePosition = playerSpherePosition;
                        throwablePosition.z += _currentSphereScale * 0.5f + _currentThrowableScale * 0.5f;
                        _currentThrowableSphereView.transform.position = throwablePosition;

                        if (_currentSphereScale <= 0)
                        {
                            onTapUp();
                            setSphereNavigatorState(SphereNavigatorState.SPHERE_RUN_OUT);
                        }
                    }
                    break;
                case SphereNavigatorState.THROW:
                    
                    break;
                case SphereNavigatorState.MOVING:
                    break;
            }

        }

        public void onTapDown()
        {
            _isTapDown = true;

        }

        public void onTapUp()
        {
            _isTapDown = false;
            if (_currentSphereNavigatorState == SphereNavigatorState.INFLATING_THROWABLE)
            {
                _currentThrowableSphereView.throwSphere();
                setSphereNavigatorState(SphereNavigatorState.IDLE);
            }
        }
    }
}