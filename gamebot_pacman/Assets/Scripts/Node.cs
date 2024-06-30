using UnityEngine;

namespace Pacman
{
    public class Node
    {
        public Vector2Int GridPosition { get; private set; }
        public int G { get; set; }
        public int H { get; set; }
        public int F => G + H;
        public Node Parent { get; set; }

        public Node(Vector2Int gridPosition)
        {
            GridPosition = gridPosition;
            G = int.MaxValue;
            H = 0;
            Parent = null;
        }
    }
}