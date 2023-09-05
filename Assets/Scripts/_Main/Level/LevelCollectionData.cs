using System.Collections.Generic;
using UnityEngine;

namespace sphereGame.level
{
    [CreateAssetMenu(fileName = "LevelCollectionData", menuName = "Data/Level/LevelCollectionData")]
    public class LevelCollectionData : ScriptableObject
    {
        [SerializeField] private List<LevelPatternData> _levelPatternsCollection;

        public List<LevelPatternData> levelPatternsCollection
        {
            get { return _levelPatternsCollection; }
        }
    }
}