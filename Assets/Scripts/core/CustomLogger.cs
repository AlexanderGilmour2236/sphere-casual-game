using UnityEngine;

namespace tuesdayPizza
{
    public static class CustomLogger
    {
        public static void showLog(string text)
        {
            Debug.Log(text);
        }

        public static void showError(string text)
        {
            Debug.LogError(text);
        }
    }
}