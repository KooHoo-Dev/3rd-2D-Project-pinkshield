using Study.Utilities;
using UnityEngine;

namespace WatchYourBack
{   
    /// <summary>
    /// 게임 진행 상황(마지막으로 클리어한 방)을 기억하는 싱글톤입니다
    /// 씬이 재로드되어도 사라지지 않고 유지됩니다 (DontDestroyOnLoad)
    /// </summary>
    public class GameProgressManager : SingletonBase<GameProgressManager>
    {       
        

        public int LastClearedRoomIndex => lastClearedRoomIndex;
        public bool HasSeenTutorial => hasSeenTutorial;



        private bool hasSeenTutorial = false;

        // 클리어한 방이 없으면 -1 (아직 첫 보스도 못 잡은 상태)
        private int lastClearedRoomIndex = -1;



        protected override void OnInitialize()
        {
            DontDestroyOnLoad(gameObject);
        }


        /// <summary>
        /// 튜토리얼을 다 봤음을 기록합니다. 이후 재시작/이어하기에서는 다시 뜨지 않습니다
        /// </summary>
        public void MarkTutorialSeen()
        {
            hasSeenTutorial = true;
        }

        /// <summary>
        /// 방을 클리어했을 때 호출합니다. 더 앞선 방 번호가 들어와도 무시합니다(역행 방지)
        /// </summary>
        public void MarkRoomCleared(int roomIndex)
        {
            if (roomIndex > lastClearedRoomIndex)
            {
                lastClearedRoomIndex = roomIndex;
            }
        }



        /// <summary>
        /// 진행 상황을 전부 초기화합니다 ("처음부터 시작" 선택 시 호출)
        /// </summary>
        public void ResetProgress()
        {
            lastClearedRoomIndex = -1;
        }
    }
}