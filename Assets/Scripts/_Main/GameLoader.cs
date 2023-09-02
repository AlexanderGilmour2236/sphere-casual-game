using UnityEngine;

namespace sphereGame
{
    public class GameLoader : MonoBehaviour
    {
        [SerializeField] private SGSceneAccessor _sgSceneAccessor;
        private SGApp _sgApp;

        public void Start()
        {
            _sgApp = new SGApp(new SGGameNavigator(_sgSceneAccessor, null), new SGServicesLoader());
            _sgApp.start();
        }
    }
}