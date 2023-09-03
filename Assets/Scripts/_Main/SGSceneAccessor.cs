using System.Collections.Generic;
using tuesdayPizza;
using UnityEngine;

namespace sphereGame
{
    public class SGSceneAccessor : SceneAccessor
    {
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private SphereViewFactory _sphereViewFactory;
        [SerializeField] private Transform _levelStartPoint;
        [SerializeField] private SphereData _defaultSphereData;
        [SerializeField] private List<ObstacleView> _obstacleViews;
        [SerializeField] private ParticleSystem _explosionParticle;

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

        public List<ObstacleView> obstacleViews
        {
            get { return _obstacleViews; }
        }

        public ParticleSystem explosionParticle
        {
            get { return _explosionParticle; }
        }
    }
}