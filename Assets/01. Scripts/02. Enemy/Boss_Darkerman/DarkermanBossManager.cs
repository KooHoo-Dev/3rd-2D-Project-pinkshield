using UnityEngine;


namespace WatchYourBack

{
    public class DarkermanBossManager : MonoBehaviour
    {

        public void RealDarkermanDefeated()
        {
            Debug.Log("진짜 Darkerman 처치!");

            GameProgressManager.Instance.ResetProgress(); // 클리어시 게임 클리어
        }

    }    

}


