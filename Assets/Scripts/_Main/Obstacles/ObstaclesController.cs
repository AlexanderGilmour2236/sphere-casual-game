using System;
using System.Collections;
using System.Collections.Generic;
using misc;
using UnityEngine;
using Object = UnityEngine.Object;

namespace sphereGame.obstacle
{
    public class ObstaclesController
    {
        public event Action<ObstacleView, GameObject> obstacleCollide;

        private List<ObstacleView> _obstacleViews;
        private Dictionary<ObstacleView, Coroutine> _explosionCoroutines = new Dictionary<ObstacleView, Coroutine>();
        private MonoPool<ExplosionView> _explosionViewsPool;

        private ExplosionView _explosionView;

        public void setExplosionParticleSystem(ExplosionView explosionView)
        {
            if (_explosionViewsPool != null)
            {
                disposeExplosionsPool();
            }
            _explosionView = explosionView;
            _explosionViewsPool = new MonoPool<ExplosionView>(null, () => Object.Instantiate(_explosionView));
        }

        private void disposeExplosionsPool()
        {
            _explosionViewsPool.Dispose();
        }

        public void setObstacleCollisionTag(string tag)
        {
            foreach (ObstacleView obstacleView in _obstacleViews)
            {
                obstacleView.setCollisionTag(tag);
            }
        }

        public List<ObstacleView> spawnObstacles( /* Obstacles Data */)
        {
            // TODO: temp solution, change to level loader
            _obstacleViews = new List<ObstacleView>();
            _obstacleViews.AddRange(Object.FindObjectsOfType<ObstacleView>());
            
            foreach (ObstacleView obstacleView in _obstacleViews)
            {
                subscribeObstacleView(obstacleView);
            }
            return _obstacleViews;
        }

        private void subscribeObstacleView(ObstacleView obstacleView)
        {
            obstacleView.obstacleCollided += onObstacleCollided;
        }

        private void unsubscribeObstacleView(ObstacleView obstacleView)
        {
            obstacleView.obstacleCollided -= onObstacleCollided;
        }

        private void onObstacleCollided(ObstacleView obstacleView, GameObject collidedObject)
        {
            obstacleCollide?.Invoke(obstacleView, collidedObject);
        }
        
        public void addObstacleToExplosionSequence(ObstacleView obstacleView, float distanceToObstacle)
        {
            if (!_explosionCoroutines.ContainsKey(obstacleView))
            {
                _explosionCoroutines.Add(obstacleView, 
                    CoroutineRunner.instance.startCoroutine(explodeObstacleRoutine(obstacleView, distanceToObstacle)));
            }

        }
        
        private IEnumerator explodeObstacleRoutine(ObstacleView obstacleView, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            ExplosionView explosionView = _explosionViewsPool.GetObject();
            explosionView.transform.position = obstacleView.transform.position;
            
            ParticleSystem explosionParticleSystem = explosionView.explosionParticleSystem;
            explosionParticleSystem.Play();
            
            Object.Destroy(obstacleView.gameObject);

            // inflating bonus feature
            //
            // float playerSphereBonusInflation = 0.1f * _currentSphereScale;
            // inflateSphere(playerSphereBonusInflation);

            yield return new WaitForSeconds(explosionParticleSystem.main.duration);
            _explosionCoroutines.Remove(obstacleView);
            _explosionViewsPool.ReleaseObject(explosionView);
        }

        public void dispose()
        {
            foreach (KeyValuePair<ObstacleView,Coroutine> obstacleViewToCoroutine in _explosionCoroutines)
            {
                CoroutineRunner.instance.stopCoroutine(obstacleViewToCoroutine.Value);
            }
            _explosionCoroutines.Clear();
            
            foreach (ObstacleView obstacleView in _obstacleViews)
            {
                unsubscribeObstacleView(obstacleView);
            }
            _obstacleViews.Clear();
        }
    }
}