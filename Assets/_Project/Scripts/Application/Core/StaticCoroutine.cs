using UnityEngine;

namespace _Project.Scripts.Application.Core
{
    public class StaticCoroutine : MonoBehaviour
    {
        private static StaticCoroutine _instance;
        public static StaticCoroutine Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("StaticCoroutine");
                    _instance = obj.AddComponent<StaticCoroutine>();
                    Object.DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }
    }
}