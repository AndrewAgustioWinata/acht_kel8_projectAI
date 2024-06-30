using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    public class PowerPellet : MonoBehaviour
    {
        public const int POWER_PELLET_POINT = 300;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                PacmanGameController.Instance.AddScore(POWER_PELLET_POINT);
                gameObject.SetActive(false);
            }
        }
    }
}
