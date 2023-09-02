using tuesdayPizza;
using UnityEngine;

namespace sphereGame
{
    public class SphereNavigator : Navigator, IInputListener
    {
        private readonly PlayerSphereView _currentPlayerSphereView;
        private readonly InputSystem _inputSystem;
        
        private bool _isTapDown;

        public SphereNavigator(SGSceneAccessor sgSceneAccessor, Navigator parent) : base(parent)
        {
            _currentPlayerSphereView = sgSceneAccessor.currentPlayerSphereView;
            _inputSystem = sgSceneAccessor.inputSystem;
            _inputSystem.addInputListener(this);
        }

        public override void tick()
        {
            base.tick();
            if (_isTapDown)
            {
                _currentPlayerSphereView.addSphereSize(-Time.deltaTime);
            }
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