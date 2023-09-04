using UnityEngine;

namespace sphereGame.sphere
{
    public class SphereViewFactory : MonoBehaviour
    {
        [SerializeField] private PlayerSphereView _defaultPlayerSphereViewPrefab;
        [SerializeField] private ThrowableView _throwableViewPrefab;

        public PlayerSphereView getPlayerSphereView(SphereData sphereData)
        {
            PlayerSphereView playerSphereView = Instantiate(_defaultPlayerSphereViewPrefab);
            playerSphereView.inflateComponent.setSize(sphereData.startScale, true);
            return playerSphereView;
        }

        public ThrowableView getThrowableSphereView()
        {
            ThrowableView throwableView = Instantiate(_throwableViewPrefab);
            return throwableView;
        }
    }
}