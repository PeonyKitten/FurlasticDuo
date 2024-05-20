using UnityEngine;

namespace Game.Scripts.Patterns
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        /// <summary>
        /// Returns the Singleton's instance, creating a new one if necessary.
        /// </summary>
        /// <remarks>Potentially expensive initial call.</remarks>
        public static T Instance
        {
            get
            {
                _instance ??= FindAnyObjectByType<T>();

                if (_instance is not null) return _instance;
                
                // Create a new GameObject and update _instance
                var go = new GameObject(typeof(T).Name);
                _instance = go.AddComponent<T>();

                return _instance;
            }
        }

        /// <summary>
        /// Returns whether we have an instance, with no side effects.
        /// </summary>
        /// <returns></returns>
        public static bool HasInstance()
        {
            return _instance is not null;
        }

        /// <summary>
        /// Ensures a single instance of this Singleton exists, and is set to be persistent across scene changes.
        /// </summary>
        public void Awake()
        {
            if (!_instance)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}