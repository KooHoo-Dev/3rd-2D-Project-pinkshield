using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WatchYourBack
{
    public class TestManager : MonoBehaviour
    {

        [Header("Debug")]
        [SerializeField] private EnemyStateManager debugTargetAlleyman;
        [SerializeField] private EnemyStateManager debugTargetWatchman;
        [SerializeField] private EnemyStateManager debugTargetRatman;
        [SerializeField] private EnemyStateManager debugTargetDarkerman;

        [Header("Darkerman Boss(엔딩호출)")]
        [SerializeField] private DarkermanBossManager bossManager;


        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.F2))
            {
                KillAlleyman();
            }

            if(SimpleInput.GetKeyDown(Key.F3))
            {
                KillWatchman();
            }

            if (SimpleInput.GetKeyDown(Key.F4))
            {
                KillRatman();
            }

            if (SimpleInput.GetKeyDown(Key.F5))
            {
                KillDarkerman();
            }
        }



        public void KillAlleyman()
        {
            if (debugTargetAlleyman == null)
            {
                Debug.LogError("KillEnemy : debugTargetEnemy가 연결되지 않았습니다.");
                return;
            }

            debugTargetAlleyman.ChangeState(new EnemyKnockDownState());
        }

        public void KillWatchman()
        {
            if (debugTargetWatchman == null)
            {
                Debug.LogError("KillEnemy : debugTargetEnemy가 연결되지 않았습니다.");
                return;
            }

            debugTargetWatchman.ChangeState(new EnemyKnockDownState());
        }

        

        public void KillRatman()
        {
            if (debugTargetRatman == null)
            {
                Debug.LogError("KillEnemy : debugTargetEnemy가 연결되지 않았습니다.");
                return;
            }

            debugTargetRatman.ChangeState(new EnemyKnockDownState());

        }

        public void KillDarkerman()
        {
            if (debugTargetDarkerman == null)
            {
                Debug.LogError("KillEnemy : debugTargetEnemy가 연결되지 않았습니다.");
                return;
            }

            debugTargetDarkerman.ChangeState(new EnemyKnockDownState());
        }

    }
}


