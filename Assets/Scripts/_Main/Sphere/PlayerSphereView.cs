using UnityEngine;

namespace sphereGame
{
    public class PlayerSphereView : MonoBehaviour
    {
        [SerializeField] private InflateComponent _inflateComponent;
        [SerializeField] private Material _markMaterial;

        public InflateComponent inflateComponent
        {
            get { return _inflateComponent; }
        }

        public Material markMaterial
        {
            get
            {
                return _markMaterial;
            }
        }
    }
}
