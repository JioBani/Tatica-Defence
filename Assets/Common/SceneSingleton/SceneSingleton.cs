using UnityEngine;

namespace Common.SceneSingleton
{
    // 씬에 하나만 존재하는 전역 접근 싱글톤 베이스
    public abstract class SceneSingleton<T> : MonoBehaviour where T : Component
    {
        static T _instance;
        static bool _quitting;

        // 전역 접근 진입점
        public static T Instance
        {
            get
            {
                if (_quitting) return null;

                // 이미 캐시되어 있으면 반환
                if (_instance != null) return _instance;

                _instance = FindFirstObjectByType<T>(FindObjectsInactive.Exclude);

                return _instance;
            }
        }

        // 중복 방지 및 DontDestroyOnLoad 옵션
        [SerializeField] bool _dontDestroyOnLoad = true;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning($"{typeof(T).Name} duplicate detected. Destroying {name}.");
                Destroy(gameObject);
                return;
            }
            _instance = this as T;

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            OnAwakeSingleton(); // 파생 클래스 초기화 훅
        }

        protected virtual void OnApplicationQuit() { _quitting = true; }
        protected virtual void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        protected virtual void OnAwakeSingleton() { }
    }
}
