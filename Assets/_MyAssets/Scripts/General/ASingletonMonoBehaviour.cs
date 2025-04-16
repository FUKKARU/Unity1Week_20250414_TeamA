using UnityEngine;

namespace NGeneral
{
    public abstract class ASingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] instances = FindObjectsByType<T>(FindObjectsSortMode.None);
                    if (instances.Length <= 0)
                    {
                        throw new System.Exception($"No instance of {typeof(T).Name} found in the scene.");
                    }
                    else if (instances.Length >= 2)
                    {
                        Debug.LogError($"Multiple instances of {typeof(T).Name} found in the scene. Destroying all but the first instance.");
                        for (int i = 1; i < instances.Length; i++)
                        {
                            Destroy(instances[i].gameObject);
                        }
                        instance = instances[0];
                    }
                    else
                    {
                        instance = instances[0];
                    }
                }
                return instance;
            }
        }
    }
}