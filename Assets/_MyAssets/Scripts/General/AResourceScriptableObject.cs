using UnityEngine;

namespace NGeneral
{
    public abstract class AResourceScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static readonly string Path = typeof(T).Name;

        private static T entity;
        public static T Entity
        {
            get
            {
                if (entity == null)
                {
                    entity = Resources.Load<T>(Path);
                    if (entity == null)
                    {
                        Debug.LogError($"Failed to load {Path} from Resources.");
                    }
                }
                return entity;
            }
        }
    }
}