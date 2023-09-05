using System.Collections;
using System.Collections.Generic;
using mics.input;
using sphereGame.camera;
using sphereGame.level;
using sphereGame.obstacle;
using sphereGame.UI;
using tuesdayPizza;
using UnityEngine;
using Object = UnityEngine.Object;

namespace sphereGame.sphere
{
    public class SphereNavigator : Navigator, IInputListener
    {
        private const float DELAY_TO_FIRST_EXPLOSION = 0.25f;
        private const string WIN_GAME_OVER_TEXT = "GREAT!";
        private const string LOSE_GAME_OVER_TEXT = "TRY AGAIN";

        private readonly SGSceneAccessor _sceneAccessor;
        private readonly InputSystem _inputSystem;

        private readonly SphereController _sphereController;
        private readonly ObstaclesController _obstaclesController;
        private readonly CameraController _cameraController;

        private List<ObstacleView> _currentObstacles;
        private LevelController _levelController;
        private GameHUD _gameHUD;
        private bool _isGameOver;

        private int _currentLevelIndex;
        private LevelPatternData _currentLevelPattern;
        private LevelPatternView _currentLevelPatternView;


        public SphereNavigator(SGSceneAccessor sceneAccessor, Navigator parent) : base(parent)
        {
            _sceneAccessor = sceneAccessor;
            _sphereController = new SphereController(_sceneAccessor.sphereViewFactory);
            _obstaclesController = new ObstaclesController();
            _cameraController = new CameraController(_sceneAccessor.camera, _sceneAccessor.cameraRelativeOffset);
            _levelController = new LevelController(_sceneAccessor.levelCollectionData, _sceneAccessor.levelPatternPosition);
            _gameHUD = _sceneAccessor.gameHUD;
            
            _inputSystem = sceneAccessor.inputSystem;
            _inputSystem.addInputListener(this);

            subscribeSphereController();
            subscribeLevelController();
            subscribeObstacleController();
        }

        public override void go()
        {
            base.go();

            startCurrentLevel();
        }

        private void startCurrentLevel()
        {
            _isGameOver = false;
            _currentLevelPattern = _levelController.getLevelPattern(_currentLevelIndex);
            _currentLevelPatternView = _levelController.loadLevel(_currentLevelPattern.levelPatternView);
            
            _sphereController.init(_currentLevelPattern.sphereData, _sceneAccessor.levelStartPoint);
            _sphereController.start();
            
            _cameraController.setTarget(_sphereController.currentPlayerSphereView.transform);
            _cameraController.setOffsetScale(_sphereController.playerSphereScale);
            
            _currentObstacles = _obstaclesController.setObstacles(_currentLevelPatternView.obstacles);
            
            _obstaclesController.setExplosionParticleSystem(_sceneAccessor.explosionView);
            _obstaclesController.setObstacleCollisionTag(SphereGameTags.THROWABLE_TAG);
            
            _levelController.changeDoorSize(_sphereController.playerSphereScale);
            _levelController.setPlayerTransform(_sphereController.currentPlayerSphereView.transform);
        }

        private void subscribeObstacleController()
        {
            _obstaclesController.obstacleCollide += onObstacleCollidedWithThrowable;
        }

        private void subscribeSphereController()
        {
            _sphereController.playerSphereSizeChange += onPlayerSphereSizeChange;
            _sphereController.throwableRunOut += onThrowableRunOut;
        }

        private void subscribeLevelController()
        {
            _levelController.playerReachedDoorPosition += onPlayerReachedFinish;
        }

        private void onPlayerReachedFinish()
        {   
            onGameOver(true);

        }

        private void startNewLevel()
        {
            _levelController.dispose();
            _sphereController.dispose();
            _obstaclesController.dispose();
            _isGameOver = false;
            
            startCurrentLevel();
        }

        private void onThrowableRunOut(ThrowableView throwableView)
        {
            onGameOver(false);
        }

        private void onGameOver(bool isWin)
        {
            if (!_isGameOver)
            {
                _isGameOver = true;
                string gameOverText;
                
                if (isWin)
                {
                    gameOverText = WIN_GAME_OVER_TEXT;
                    _currentLevelIndex++;
                }
                else
                {
                    gameOverText = LOSE_GAME_OVER_TEXT;
                    _cameraController.setTarget(null);
                }
                
                _gameHUD.setGameOverText(gameOverText);
                _gameHUD.playGameOverSequence(startNewLevel, () => _isGameOver = false);

            }
        }

        private void onPlayerSphereSizeChange(float sphereSize)
        {
            _cameraController.setOffsetScale(sphereSize);
            _levelController.changeDoorSize(sphereSize);
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
            _cameraController.tick();
            _levelController.tick();
        }

        public void onTapDown()
        {
            if (!_isGameOver)
            {
                _sphereController.onTapDown();
            }
        }

        public void onTapUp()
        {
            _sphereController.onTapUp();
        }
    }
}