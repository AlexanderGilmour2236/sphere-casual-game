using System;
using sphereGame.sphere;
using UnityEngine;

namespace sphereGame.level
{
    [Serializable]
    public class LevelPatternData
    {
        [SerializeField] private SphereData _sphereData;
        [SerializeField] private LevelPatternView _levelPatternView;

        public SphereData sphereData
        {
            get { return _sphereData; }
        }

        public LevelPatternView levelPatternView
        {
            get { return _levelPatternView; }
        }
    }
}