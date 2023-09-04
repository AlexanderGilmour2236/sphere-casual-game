using UnityEngine;

namespace sphereGame.sphere
{
    public class PlayerSphereView : MonoBehaviour
    {
        [SerializeField] private InflateComponent _inflateComponent;
        [SerializeField] private Material _markMaterial;

        public void playThrowEffect()
        {
            Vector3 localScale = _inflateComponent.inflateTransform.localScale;
            localScale.z *= Random.Range(1.2f, 1.5f);
            localScale.x *= Random.Range(0.7f, 0.9f);
            _inflateComponent.inflateTransform.localScale = localScale;
        }
        
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
