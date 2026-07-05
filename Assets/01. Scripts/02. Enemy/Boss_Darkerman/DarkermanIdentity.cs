using UnityEngine;

namespace WatchYourBack
{
    public class DarkermanIdentity : MonoBehaviour
    {
        [SerializeField] private bool isRealDarkerman = false;


        // 기본값 false(가짜). 인스턴스 중 진짜 1개만 true로 체크할 것
        public bool IsRealDarkerman => isRealDarkerman;
    }
}




