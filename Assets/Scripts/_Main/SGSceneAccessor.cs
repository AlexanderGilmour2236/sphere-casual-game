using System.Collections.Generic;
using mics.input;
using sphereGame.level;
using sphereGame.obstacle;
using sphereGame.sphere;
using sphereGame.UI;
using tuesdayPizza;
using UnityEngine;

namespace sphereGame
{
    public class SGSceneAccessor : SceneAccessor
    {
        [Header("Main")]
        [SerializeField] private InputSystem _inputSystem;
        
        [Header("Level")]
        [SerializeField] private Transform _levelStartPoint;
        [SerializeField] private Transform _levelPatternPosition;
        [SerializeField] private LevelCollectionData _levelCollectionData;
        
        [Header("Sphere")]
        [SerializeField] private SphereViewFactory _sphereViewFactory;
        [SerializeField] private SphereData _defaultSphereData;
        
        [Header("Obstacles")]
        [SerializeField] private ExplosionView _explosionView;
        
        [Header("Camera")]
        [SerializeField] private Camera _camera;
        [SerializeField] private Vector3 _cameraRelativeOffset;
        
        [Header("UI")] 
        [SerializeField] private GameHUD _gameHUD;

        public GameHUD gameHUD
        {
            get { return _gameHUD; }
        }

        public new Camera camera
        {
            get { return _camera; }
        }

        public InputSystem inputSystem
        {
            get { return _inputSystem; }
        }

        public SphereData defaultSphereData
        {
            get { return _defaultSphereData; }
        }

        public Transform levelStartPoint
        {
            get { return _levelStartPoint; }
        }

        public SphereViewFactory sphereViewFactory
        {
            get { return _sphereViewFactory; }
        }

        public ExplosionView explosionView
        {
            get { return _explosionView; }
        }

        public Vector3 cameraRelativeOffset
        {
            get { return _cameraRelativeOffset; }
        }
        public Transform levelPatternPosition
        {
            get { return _levelPatternPosition; }
        }

        public LevelCollectionData levelCollectionData
        {
            get { return _levelCollectionData; }
        }
    }
}