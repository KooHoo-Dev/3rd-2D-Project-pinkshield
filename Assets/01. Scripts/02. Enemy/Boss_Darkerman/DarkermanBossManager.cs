using UnityEngine;
using UnityEngine.SceneManagement;

namespace WatchYourBack

{
    public class DarkermanBossManager : MonoBehaviour
    {
        [SerializeField] private string endingSceneName = "Ending";
        
        public void RealDarkermanDefeated()
        {
            
            Debug.Log("진짜 Darkerman 처치!");
            GameProgressManager.Instance.ResetProgress(); // 클리어시 게임 클리어            
            SceneManager.LoadScene(endingSceneName);
            

        }

    }    

}


