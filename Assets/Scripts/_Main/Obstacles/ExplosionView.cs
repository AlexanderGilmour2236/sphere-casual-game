using misc;
using UnityEngine;

namespace sphereGame.obstacle
{
    public class ExplosionView : MonoBehaviour, IPoolObject
    {
        [SerializeField] private ParticleSystem _explosionParticleSystem;

        public void onSpawn()
        {
            _explosionParticleSystem.time = 0;
            _explosionParticleSystem.Play(true);
        }

        public void onRelease()
        {
            _explosionParticleSystem.Stop();
        }

        public ParticleSystem explosionParticleSystem
        {
            get { return _explosionParticleSystem; }
        }
    }
}