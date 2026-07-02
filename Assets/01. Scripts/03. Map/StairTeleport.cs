using UnityEngine;

namespace WatchYourBack
{
    public class StairTeleport : MonoBehaviour
    {   
        //이동할 지점의 포인트
        [SerializeField] private Transform destination;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.position = destination.position;
            }
        }

    }
}


