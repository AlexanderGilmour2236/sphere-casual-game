using System.Collections.Generic;
using mics.input;
using sphereGame.obstacle;
using sphereGame.sphere;
using tuesdayPizza;
using UnityEngine;

namespace sphereGame
{
    public class SGSceneAccessor : SceneAccessor
    {
        [Header("Main")]
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private Transform _levelStartPoint;
        
        [Header("Sphere")]
        [SerializeField] private SphereViewFactory _sphereViewFactory;
        [SerializeField] private SphereData _defaultSphereData;
        
        [Header("Obstacles")]
        [SerializeField] private ExplosionView _explosionView;
        
        [Header("Camera")]
        [SerializeField] private Camera _camera;
        [SerializeField] private Vector3 _cameraRelativeOffset;

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
    }
}