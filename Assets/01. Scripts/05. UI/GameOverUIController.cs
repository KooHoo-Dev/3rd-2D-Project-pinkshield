using UnityEngine;
using UnityEngine.SceneManagement;

namespace WatchYourBack
{
    public class GameOverUIController : MonoBehaviour
    {
        public void RestartCurrentScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            SceneManager.LoadScene(currentSceneIndex);

            gameObject.SetActive(false);
        }

        public void LoadMainMenu()
        {
            // 씬 이름을 사용하는 방법 (정확한 씬 이름을 입력해야 함)
            SceneManager.LoadScene("GameMenu");

            // 또는 빌드 인덱스 0번을 사용하는 방법 (보통 메인 메뉴가 0번)
            // SceneManager.LoadScene(0); 
        }
    }
}



