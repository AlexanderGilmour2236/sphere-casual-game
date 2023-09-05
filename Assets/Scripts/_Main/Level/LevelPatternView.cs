using System.Collections.Generic;
using sphereGame.obstacle;
using UnityEngine;

namespace sphereGame.level
{
    public class LevelPatternView : MonoBehaviour
    {
        [SerializeField] private DoorView _doorView;
        [SerializeField] private List<ObstacleView> _obstacles;

        public List<ObstacleView> obstacles
        {
            get { return _obstacles; }
        }

        public DoorView doorView
        {
            get { return _doorView; }
        }
    }
}