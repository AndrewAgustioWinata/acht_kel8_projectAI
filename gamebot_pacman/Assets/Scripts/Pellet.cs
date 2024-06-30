using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    public class Pellet : MonoBehaviour
    {
        public const int PELLET_POINT = 200;    

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                gameObject.SetActive(false);
                PacmanGameController.Instance.AddScore(PELLET_POINT);
            }
        }
    }


}
