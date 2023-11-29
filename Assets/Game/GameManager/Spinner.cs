using UnityEngine;

namespace Game.GameManager
{
    [ExecuteInEditMode]
    public class Spinner : MonoBehaviour
    {
        public float speed = 1f;
        
        public void Update()
        {
            transform.Rotate(0, 0, speed * Time.deltaTime);
        }
    }
}