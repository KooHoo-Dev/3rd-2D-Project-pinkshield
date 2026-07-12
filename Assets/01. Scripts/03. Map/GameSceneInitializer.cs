using UnityEngine;

namespace WatchYourBack
{
    /// <summary>
    /// 씬이 시작될 때, 저장된 진행 상황에 맞춰 방/보스/플레이어 위치를 세팅합니다
    /// "이어하기"로 들어왔을 때만 의미가 있고, 새 게임이면 아무 것도 하지 않습니다
    /// </summary>
    public class GameSceneInitializer : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform defaultSpawnPoint; // 새 게임 시작 위치
        [SerializeField] private DoorManager doorManager; // 추가

        private void Start()
        {

            // 예외 처리: SoundManager가 아직 준비 안 된 극단적 상황 대비
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.ResetCombatMusicState();
            }

            int clearedRoomIndex = GameProgressManager.Instance.LastClearedRoomIndex;

            // 클리어한 방이 없으면 새 게임이므로 초기화할 것이 없음
            if (clearedRoomIndex < 0)
            {
                return;
            }

            ApplyClearedRooms(clearedRoomIndex);
            doorManager.ApplyProgressInstantly(clearedRoomIndex);
            MovePlayerToNextRoom(clearedRoomIndex);
        }

        /// <summary>
        /// 이미 클리어한 방들의 보스와 문을 전부 비활성화합니다
        /// </summary>
        private void ApplyClearedRooms(int clearedRoomIndex)
        {
            Room[] allRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);

            foreach (Room room in allRooms)
            {
                if (room.RoomIndex <= clearedRoomIndex)
                {
                    room.DeactivateBossOnly();
                }
            }
        }

        /// <summary>
        /// 플레이어를 다음 방(클리어한 방 + 1)의 시작 위치로 이동시킵니다
        /// </summary>
        private void MovePlayerToNextRoom(int clearedRoomIndex)
        {
            int nextRoomIndex = clearedRoomIndex + 1;
            Room[] allRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);

            foreach (Room room in allRooms)
            {
                if (room.RoomIndex == nextRoomIndex)
                {
                    player.position = room.PlayerSpawnPoint.position;
                    return;
                }
            }
        }
    }
}