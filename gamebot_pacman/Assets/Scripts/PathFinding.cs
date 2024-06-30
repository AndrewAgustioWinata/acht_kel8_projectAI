using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pacman
{
    public class PathFinding : MonoBehaviour
    {
        [SerializeField] public Tilemap tilemap = default;

        private List<Node> openList = new List<Node>();
        private List<Node> closedList = new List<Node>();
        private Dictionary<Vector2Int, Node> allNodes = new Dictionary<Vector2Int, Node>();

        public List<Node> FindPath(Vector3Int start, Vector3Int target)
        {
            Node startNode = GetNode(new Vector2Int(start.x, start.y));
            Node targetNode = GetNode(new Vector2Int(target.x, target.y));

            openList.Clear();
            closedList.Clear();

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = openList[0];
                foreach (Node node in openList)
                {
                    if (node.F < currentNode.F || node.F == currentNode.F && node.H < currentNode.H)
                    {
                        currentNode = node;
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (Node neighbor in GetNeighbors(currentNode))
                {
                    if (closedList.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbor = currentNode.G + GetDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor < neighbor.G || !openList.Contains(neighbor))
                    {
                        neighbor.G = newMovementCostToNeighbor;
                        neighbor.H = GetDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            return null;
        }

        private List<Node> RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Mathf.Abs(nodeA.GridPosition.x - nodeB.GridPosition.x);
            int dstY = Mathf.Abs(nodeA.GridPosition.y - nodeB.GridPosition.y);

            return 10 * (dstX + dstY);
        }

        private List<Node> GetNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>();

            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0),
                new Vector2Int(1, 0)
            };

            foreach (var direction in directions)
            {
                Vector2Int neighborPos = node.GridPosition + direction;
                if (tilemap.HasTile((Vector3Int)neighborPos))
                {
                    if (node.Parent == null || (neighborPos - node.Parent.GridPosition).magnitude > 0.1f)
                    {
                        neighbors.Add(GetNode(neighborPos));
                    }
                }
            }

            return neighbors;
        }

        private Node GetNode(Vector2Int gridPosition)
        {
            if (!allNodes.ContainsKey(gridPosition))
            {
                allNodes[gridPosition] = new Node(gridPosition);
            }
            return allNodes[gridPosition];
        }
    }
}