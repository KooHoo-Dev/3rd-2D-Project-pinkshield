using UnityEngine;

namespace WatchYourBack
{
    public class GameLineManager : MonoBehaviour
    {


        // 게임 매니저의 역할
        // 각 보스들이 죽었을 때 맞는 대사창 호출 

        // 대사 UI 장면 호출 구현 방법 (단순 생각)
        // gameobject 가 있는지 없는지 여부를 각각 확인 후 해당 캐릭터가 없어지면 UI 대사창 호출
        // space 키로 이미지와 대사를 활성화 - 비활성화로 다음 컷들을 연결

        // Dictionary 로 각 보스의 게임 오브젝트를 지정해서 각 캐릭터에 맞는 연출을 호출하자
        // (원래는 선형적 방식이라 각 캐릭 있는지 없는지 여부를 조건으로 연출하려고 했지만, 그것보단 위 방법이 더 효율적으로 보임)

    }
}


