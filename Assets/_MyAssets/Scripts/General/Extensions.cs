using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NGeneral
{
    public static class Extensions
    {
        /// <summary>
        /// Event Trigger に登録する
        /// </summary>
        public static void AddListener(this EventTrigger trigger, EventTriggerType type, UnityAction action)
        {
            if (trigger == null) return;

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(_ => action?.Invoke());

            trigger.triggers.Add(entry);
        }

        public static Vector3 SetZ(this Vector3 vector, float z)
        {
            vector.z = z;
            return vector;
        }

        /// <summary>
        /// X[a, b] を Y[c, d] に線形変換する
        /// </summary
        public static float Remap(this float x, float a, float b, float c, float d)
        {
            if (a == b) return 0;
            return (x - a) * (d - c) / (b - a) + c;
        }
    }
}