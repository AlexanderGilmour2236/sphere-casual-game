using System;
using System.Collections;
using System.Collections.Generic;
using tuesdayPizza;
using UnityEngine;
using Object = UnityEngine.Object;

namespace sphereGame
{
    public class SphereNavigator : Navigator, IInputListener
    {
        private const float DELAY_TO_FIRST_EXPLOSION = 0.25f;

        private readonly SGSceneAccessor _sceneAccessor;
        private readonly InputSystem _inputSystem;
        
        private readonly SphereController _sphereController;
        private List<ObstacleView> _currentObstacles;

        public SphereNavigator(SGSceneAccessor sceneAccessor, Navigator parent) : base(parent)
        {
            _sceneAccessor = sceneAccessor;
            _sphereController = new SphereController(_sceneAccessor.sphereViewFactory);
            
            _inputSystem = sceneAccessor.inputSystem;
            _inputSystem.addInputListener(this);
        }

        public override void go()
        {
            base.go();

            _currentObstacles = new List<ObstacleView>(_sceneAccessor.obstacleViews);
            initObstacles(_currentObstacles);
            
            _sphereController.init(getNewSphereData(), _sceneAccessor.levelStartPoint, _currentObstacles);
            _sphereController.start();
        }

        private void initObstacles(List<ObstacleView> obstacleViews)
        {
            foreach (ObstacleView obstacleView in obstacleViews)
            {
                subscribeObstacleView(obstacleView);
            }
        }

        private void subscribeObstacleView(ObstacleView obstacleView)
        {
            obstacleView.obstacleCollided += onObstacleCollided;
        }
        
        private void unsubscribeObstacleView(ObstacleView obstacleView)
        {
            obstacleView.obstacleCollided -= onObstacleCollided;
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

        private void addObstacleToExplosionQueue(ObstacleView obstacleView, float distanceToObstacle)
        {
            _sceneAccessor.StartCoroutine(explodeObstacleRoutine(obstacleView, distanceToObstacle));
        }

        private void onObstacleCollided(ObstacleView obstacleView, ThrowableView throwableView)
        {
            if (!throwableView.isHitObstacle)
            {
                applyThrowableToObstacle(throwableView, obstacleView);
                obstacleView.setObstacleMarked(_sphereController.currentPlayerSphereView.markMaterial);
            }
        }

        private IEnumerator explodeObstacleRoutine(ObstacleView obstacleView, float delay)
        {
            yield return new WaitForSeconds(delay);
            ParticleSystem explosionPS = Object.Instantiate(_sceneAccessor.explosionParticle, obstacleView.transform.position, Quaternion.identity);
            Object.Destroy(obstacleView.gameObject);
            explosionPS.Play();
            
            // inflating bonus feature
            //
            // float playerSphereBonusInflation = 0.1f * _currentSphereScale;
            // inflateSphere(playerSphereBonusInflation);

            yield return new WaitForSeconds(explosionPS.main.duration);
            Object.Destroy(explosionPS.gameObject);
        }
        
        private void applyThrowableToObstacle(ThrowableView throwableView, ObstacleView appliedObstacleView)
        {
            throwableView.throwAtObstacle(appliedObstacleView.transform);
            
            addObstacleToExplosionQueue(appliedObstacleView, DELAY_TO_FIRST_EXPLOSION);
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
                    addObstacleToExplosionQueue(obstacleView, DELAY_TO_FIRST_EXPLOSION + distanceToObstacle * 0.25f);
                }
            }

            foreach (ObstacleView obstacleView in collidedObstacles)
            {
                _currentObstacles.Remove(obstacleView);
            }
            
            Object.Destroy(throwableView.gameObject, 5);
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