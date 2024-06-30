using UnityEngine;

namespace Pacman
{
    public class Blinky : Ghost
    {
        [SerializeField] private Transform scatterPos = default;
        protected override Vector3Int GetChasePosition()
        {
            return Vector3Int.FloorToInt(PathFinding.tilemap.WorldToCell(PacmanPosition));
        }
    }
}
