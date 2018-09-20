using UnityEngine;

namespace Gamekit2D
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Checkpoint : MonoBehaviour
    {
        public bool respawnFacingLeft;

        private void Reset()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            PlayerCharacter c = collision.GetComponent<PlayerCharacter>();
            Debug.Log("C name" + c.gameObject.name);
            if (c != null)
            {
                c.SetChekpoint(this);
                Debug.Log("Checkpoint set");
            }
        }
    }
}