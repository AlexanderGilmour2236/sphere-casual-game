﻿using System.Collections;
using System.Collections.Generic;
using mics.input;
using sphereGame.obstacle;
using tuesdayPizza;
using UnityEngine;
using Object = UnityEngine.Object;

namespace sphereGame.sphere
{
    public class SphereNavigator : Navigator, IInputListener
    {
        private const float DELAY_TO_FIRST_EXPLOSION = 0.25f;

        private readonly SGSceneAccessor _sceneAccessor;
        private readonly InputSystem _inputSystem;
        
        private readonly SphereController _sphereController;
        private readonly ObstaclesController _obstaclesController;
        
        private List<ObstacleView> _currentObstacles;

        public SphereNavigator(SGSceneAccessor sceneAccessor, Navigator parent) : base(parent)
        {
            _sceneAccessor = sceneAccessor;
            _sphereController = new SphereController(_sceneAccessor.sphereViewFactory);
            _obstaclesController = new ObstaclesController();
            
            _inputSystem = sceneAccessor.inputSystem;
            _inputSystem.addInputListener(this);

            subscribeObstacleController();
        }

        public override void go()
        {
            base.go();

            _currentObstacles = new List<ObstacleView>(_sceneAccessor.obstacleViews);
            
            _sphereController.init(getNewSphereData(), _sceneAccessor.levelStartPoint);
            _sphereController.start();
            
            _obstaclesController.spawnObstacles();
            _obstaclesController.setExplosionParticleSystem(_sceneAccessor.explosionView);
            _obstaclesController.setObstacleCollisionTag(SphereGameTags.THROWABLE_TAG);
        }

        private void subscribeObstacleController()
        {
            _obstaclesController.obstacleCollide += onObstacleCollidedWithThrowable;
        }

        private void onObstacleCollidedWithThrowable(ObstacleView obstacleView, GameObject collidedObject)
        {
            ThrowableView throwableView = collidedObject.GetComponentInParent<ThrowableView>();
            if (throwableView != null && !throwableView.isHitObstacle)
            {
                obstacleView.isObstacleCollided = true;
                throwableView.throwAtObstacle(obstacleView.transform);
                applyThrowableToObstacle(throwableView, obstacleView);
                obstacleView.setObstacleMarked(_sphereController.currentPlayerSphereView.markMaterial);
            }
        }

        private void applyThrowableToObstacle(ThrowableView throwableView, ObstacleView appliedObstacleView)
        {
            _sphereController.autoDestroyThrowable(throwableView);
            _obstaclesController.addObstacleToExplosionSequence(appliedObstacleView, DELAY_TO_FIRST_EXPLOSION);
            List<ObstacleView> collidedObstacles = new List<ObstacleView>(){ appliedObstacleView };
            
            foreach (ObstacleView obstacleView in _currentObstacles)
            {
                if (obstacleView == appliedObstacleView)
                {
                    continue;
                }

                float distanceToObstacle = Vector3.Distance(obstacleView.transform.position,
                    appliedObstacleView.transform.position);
                
                if (distanceToObstacle <= throwableView.inflateComponent.currentTargetScale)
                {
                    collidedObstacles.Add(obstacleView);
                    obstacleView.setObstacleMarked(_sphereController.currentPlayerSphereView.markMaterial);
                    _obstaclesController.addObstacleToExplosionSequence(obstacleView, DELAY_TO_FIRST_EXPLOSION + distanceToObstacle * 0.25f);
                }
            }

            foreach (ObstacleView obstacleView in collidedObstacles)
            {
                _currentObstacles.Remove(obstacleView);
            }

        }

        private SphereData getNewSphereData()
        {
            return _sceneAccessor.defaultSphereData;
        }

        public override void tick()
        {
            base.tick();
            _sphereController.tick();
        }

        public void onTapDown()
        {
            _sphereController.onTapDown();
        }

        public void onTapUp()
        {
            _sphereController.onTapUp();
        }
    }
}