using UnityEngine;

namespace sphereGame
{
    public class PlayerSphereView : MonoBehaviour
    {
        [SerializeField] private InflateComponent _inflateComponent;

        public InflateComponent inflateComponent
        {
            get { return _inflateComponent; }
        }
    }
}
