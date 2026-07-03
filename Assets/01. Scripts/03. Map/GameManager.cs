using UnityEngine;

namespace WatchYourBack
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public int LastCheckpointIndex = 0; // 보스 체크 포인트 0: 시작점,  1: 1번 보스 후, 2: 2번 보스후 ..

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // 씬 전환/ 재시작 시 시작 파괴 방지
            }
            else 
            {
                Destroy(gameObject);
            }
        }


    }

}



