using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sphereGame
{
    public class ObstacleView : MonoBehaviour
    {
        [SerializeField] private List<MeshRenderer> _meshRenderers;

        private bool _obstacleCollided = false;
        
        public event Action<ObstacleView, ThrowableView> obstacleCollided;

        public void setObstacleMarked(Material markMaterial)
        {
            foreach (MeshRenderer meshRenderer in _meshRenderers)
            {
                Material[] newMaterials = new Material[meshRenderer.materials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = markMaterial;
                }

                meshRenderer.materials = newMaterials;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!_obstacleCollided && other.CompareTag(SphereGameTags.THROWABLE_TAG))
            {
                _obstacleCollided = true;
                obstacleCollided?.Invoke(this, other.GetComponentInParent<ThrowableView>());
            }
        }
    }
}
