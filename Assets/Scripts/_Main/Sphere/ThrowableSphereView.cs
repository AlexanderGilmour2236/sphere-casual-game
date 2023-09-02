using UnityEngine;

namespace sphereGame
{
    public class ThrowableSphereView : MonoBehaviour
    {
        [SerializeField] private InflateComponent _inflateComponent;

        public void addSphereSize(float addedSphereSize)
        {
            _inflateComponent.addSphereSize(addedSphereSize);
        }
    }
}