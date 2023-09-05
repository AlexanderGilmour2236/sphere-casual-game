using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace sphereGame
{
    public class GameLoader : MonoBehaviour
    {
        [SerializeField] private SGSceneAccessor _sgSceneAccessor;
        private SGApp _sgApp;

        public void Start()
        {
            Application.targetFrameRate = 60;
            _sgApp = new SGApp(new SGGameNavigator(_sgSceneAccessor, null), new SGServicesLoader());
            _sgApp.start();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                SceneManager.LoadScene("Scenes/MainScene");
            }
        }
    }
}