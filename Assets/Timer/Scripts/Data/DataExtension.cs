using UnityEngine;

namespace Timer.Scripts.Data
{
    public static class DataExtension
    {
        public static string ToJson(this object progress) => 
            JsonUtility.ToJson(progress);

        public static T ToDeserialized<T>(this string progress) => 
            JsonUtility.FromJson<T>(progress);
    }
}