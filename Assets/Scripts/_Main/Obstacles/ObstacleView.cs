using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sphereGame.obstacle
{
    public class ObstacleView : MonoBehaviour
    {
        [SerializeField] private List<MeshRenderer> _meshRenderers;

        private bool _isObstacleCollided = false;
        private string _collisionTag = "Untagged";

        public event Action<ObstacleView, GameObject> obstacleCollided;

        public void setCollisionTag(string collisionTag)
        {
            _collisionTag = collisionTag;
        }
        
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
            if (!_isObstacleCollided && other.CompareTag(_collisionTag))
            {
                obstacleCollided?.Invoke(this, other.gameObject);
            }
        }
        
        public bool isObstacleCollided
        {
            get { return _isObstacleCollided; }
            set { _isObstacleCollided = value; }
        }
    }
}
