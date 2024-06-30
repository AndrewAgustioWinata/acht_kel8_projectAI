using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Pacman
{
    public class PacmanGameController : MonoBehaviour
    {
        [SerializeField] private PacmanCharacter pacmanCharacter = default;
        [SerializeField] private TextMeshProUGUI currentScoreTMP = default;
        [SerializeField] private PathFinding pathFinding = default;
        [SerializeField] private Blinky blinky = default;
        [SerializeField] private Transform pelletTransform = default;
        [SerializeField] private TextMeshProUGUI gameStatusTMP = default;
        [SerializeField] private GameObject RestartGO = default;
        public static PacmanGameController Instance { get; private set; } = null;
        public bool hasStartGame { get; private set; } = false;
        private int score = 0;
        public const float MOVEMENT_SPEED_MULTIPLIER = 5.5f;
        public bool hasLose { get; private set; } = false;

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                hasStartGame = true;
            }
        }

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            StartGame();
        }
        private void StartGame()
        {
            if (hasStartGame)
                return;
            RestartGO.SetActive(false);
            RestartGame();
            blinky.Init(pathFinding, pacmanCharacter);
        }

        public void AddScore(int score)
        {
            this.score += score;
            currentScoreTMP.text = $"Score: {this.score}";

            CheckGame();
        }

        private bool hasWinGame()
        {
            Pellet[] pellets = pelletTransform.GetComponentsInChildren<Pellet>();

            foreach(Pellet pellet in pellets)
            {
                if (pellet.gameObject.activeSelf)
                    return false;
            }

            return true;
        }

        private void CheckGame()
        {
            if (hasWinGame())
            {
                gameStatusTMP.text = "WIN";
                RestartGO.SetActive(true);
                UpdatePlayerGameStatus();
            }

            if (hasLose)
            {
                RestartGO.SetActive(true);
                gameStatusTMP.text = "LOSE";
            }
        }

        public void UpdatePlayerGameStatus()
        {
            hasLose = true;
            CheckGame();
        }

        public void RestartGame()
        {
            RestartGO.SetActive(false);
            hasLose = false;
            hasStartGame = false;
            score = 0;
            currentScoreTMP.text = $"Score: {score}";
            blinky.ResetPos();
            gameStatusTMP.SetText("");
            pacmanCharacter.ResetGameCharacter();

            Pellet[] pellets = pelletTransform.GetComponentsInChildren<Pellet>(true);
            foreach (Pellet pellet in pellets)
            {
                pellet.gameObject.SetActive(true);
            }

        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
