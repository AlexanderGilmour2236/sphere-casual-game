using UnityEngine;

namespace sphereGame
{
    public class SphereViewFactory : MonoBehaviour
    {
        [SerializeField] private PlayerSphereView _defaultPlayerSphereViewPrefab;
        [SerializeField] private ThrowableSphereView _throwableSphereViewPrefab;

        public PlayerSphereView getPlayerSphereView(SphereData sphereData)
        {
            PlayerSphereView playerSphereView = Instantiate(_defaultPlayerSphereViewPrefab);
            playerSphereView.inflateComponent.setSize(sphereData.startScale);
            return playerSphereView;
        }

        public ThrowableSphereView getThrowableSphereView()
        {
            ThrowableSphereView sphereView = Instantiate(_throwableSphereViewPrefab);
            return sphereView;
        }
    }
}