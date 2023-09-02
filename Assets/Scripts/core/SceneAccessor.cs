using UnityEngine;

namespace tuesdayPizza
{
    public class SceneAccessor : MonoBehaviour
    {
        public static SceneAccessor findSceneAccessorInScene()
        {
            return FindObjectOfType<SceneAccessor>();
        }
    }
}