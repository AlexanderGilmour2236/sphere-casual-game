using UnityEngine;

namespace sphereGame
{
    public class PlayerSphereView : MonoBehaviour
    {
        [SerializeField] private InflateComponent _inflateComponent;

        public void addSphereSize(float addedSphereSize)
        {
            _inflateComponent.addSphereSize(addedSphereSize);
        }
    }
}
