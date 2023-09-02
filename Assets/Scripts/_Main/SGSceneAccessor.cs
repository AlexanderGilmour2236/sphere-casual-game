using tuesdayPizza;
using UnityEngine;

namespace sphereGame
{
    public class SGSceneAccessor : SceneAccessor
    {
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private PlayerSphereView _currentPlayerSphereView;

        public PlayerSphereView currentPlayerSphereView
        {
            get { return _currentPlayerSphereView; }
        }

        public InputSystem inputSystem
        {
            get { return _inputSystem; }
        }
    }
}