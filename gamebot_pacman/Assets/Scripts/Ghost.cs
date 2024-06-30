using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    public abstract class Ghost : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer = default;
        [SerializeField] private Sprite upSprite = default;    
        [SerializeField] private Sprite downSprite = default;  
        [SerializeField] private Sprite leftSprite = default;  
        [SerializeField] private Sprite rightSprite = default; 

        public PathFinding PathFinding { get; private set; } = default;
        private PacmanCharacter pacmanCharacter = default;
        public Vector3 PacmanPosition => pacmanCharacter.transform.position;

        private Vector3 previousPosition;
        protected List<Node> path;
        protected int currentPathIndex;
        private Vector3 lastDirection = default;
        private Vector3 defaultPos = default;
        public void Init(PathFinding pathFinding, PacmanCharacter pacmanCharacter)
        {
            PathFinding = pathFinding;
            this.pacmanCharacter = pacmanCharacter;
            defaultPos = transform.position;
            previousPosition = transform.position;
            InvokeRepeating(nameof(UpdatePath), 0, 1f);
        }

        public void ResetPos()
        {
            transform.position = defaultPos;
        }

        protected virtual void UpdatePath()
        {
            Vector3Int ghostGridPos = Vector3Int.FloorToInt(PathFinding.tilemap.WorldToCell(transform.position));
            Vector3Int targetGridPos = GetTargetGridPos();
            path = PathFinding.FindPath(ghostGridPos, targetGridPos);

            if (path != null && path.Count > 0)
            {
                currentPathIndex = 0;
            }
        }

        private Vector3Int GetTargetGridPos()
        {
            Vector3Int targetPos = GetChasePosition();
            return targetPos;
        }
        protected abstract Vector3Int GetChasePosition();

        protected virtual void Update()
        {
            if (!PacmanGameController.Instance.hasStartGame || PacmanGameController.Instance.hasLose)
            {
                return;
            }

            if (path != null && currentPathIndex < path.Count)
            {
                Vector3 offSet = new Vector3(.5f, .5f, 0);
                Vector3 targetPosition = PathFinding.tilemap.CellToWorld((Vector3Int)path[currentPathIndex].GridPosition) + offSet;
                targetPosition.z = transform.position.z;

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, PacmanGameController.MOVEMENT_SPEED_MULTIPLIER * Time.deltaTime);

                if (transform.position == targetPosition)
                {
                    currentPathIndex++;
                }

                Vector3 direction = transform.position - previousPosition;
                if (direction.y > 0)
                {
                    lastDirection = Vector3.up;
                    spriteRenderer.sprite = upSprite;
                }
                else if (direction.y < 0)
                {
                    lastDirection = Vector3.down;
                    spriteRenderer.sprite = downSprite;
                }
                else if (direction.x > 0)
                {
                    lastDirection = Vector3.right;
                    spriteRenderer.sprite = rightSprite;
                }
                else if (direction.x < 0)
                {
                    lastDirection = Vector3.left;
                    spriteRenderer.sprite = leftSprite;
                }

                previousPosition = transform.position;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.position + lastDirection, PacmanGameController.MOVEMENT_SPEED_MULTIPLIER * Time.deltaTime);   
            }
        }
    }
}
