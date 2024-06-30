using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    public class Passage : MonoBehaviour
    {
        [SerializeField] Passage otherPassage = default;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Vector3 offset = transform.position.x > 0 ? new Vector3(1,0,0) : new Vector3(-1, 0, 0);

            collision.transform.position = otherPassage.transform.position + offset;
        }
    }
}
