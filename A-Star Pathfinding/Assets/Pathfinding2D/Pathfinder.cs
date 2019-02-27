using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

    public float navSpeed = 3;
    public Grid grid;
    public List<GridNode> path = new List<GridNode>();
    public Vector2 currentTargetPos;
    public bool pathComplete;

    private void Awake() {
        if (grid == null) grid = FindObjectOfType<Grid>();
    }

    public bool FindPath(Vector2 startPos, Vector2 targetPos) {

        currentTargetPos = targetPos;

        GridNode startNode = grid.NodeFromWorldPoint(startPos);
        GridNode targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<GridNode> openSet = new Heap<GridNode>(grid.TotalCellCount);
        HashSet<GridNode> closedSet = new HashSet<GridNode>();
        openSet.Add(startNode);

        bool pathSuccessful = false;

        while (openSet.Count > 0) {
            GridNode currentNode = openSet.RemoveFirst();
            /*for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost) {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);*/
            closedSet.Add(currentNode);

            if (currentNode == targetNode) {
                pathSuccessful = true;
                break;
            }

            foreach (GridNode neighbor in grid.GetNeighbors(currentNode)) {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) {
                    continue;
                }

                int newMovementCostToNeighbor = currentNode.gCost + grid.GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)) {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = grid.GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor)) {
                        openSet.Add(neighbor);
                    }
                }
            }

        }

        if (pathSuccessful) {
            RetracePath(startNode, targetNode);
        }
        
        return pathSuccessful;

    }

    void RetracePath(GridNode startNode, GridNode endNode) {
        List<GridNode> newPath = new List<GridNode>();
        GridNode currentNode = endNode;

        while (currentNode != startNode) {
            newPath.Add(currentNode);
            currentNode = currentNode.parent;
        }
        newPath.Reverse();

        path = newPath;

    }

    public void UpdatePath() {
        FindPath(transform.position, currentTargetPos);
    }

    public void NavigatePath() {
        if (path.Count > 0) StartCoroutine(NavigatePathRoutine());
    }

    IEnumerator NavigatePathRoutine() {
        pathComplete = false;
        while (path.Count > 0) {
            if (Vector2.Distance(transform.position,path[0].worldPos) > .1f) {
                //transform.right = Vector2.Lerp(transform.right, path[0].worldPos - (Vector2)transform.position, Time.deltaTime * 4f);
                transform.right = path[0].worldPos - (Vector2)transform.position;
                transform.position += transform.right * Time.deltaTime * navSpeed;

            } else {
                path.RemoveAt(0);
            }
            yield return null;
        }
        currentTargetPos = transform.position;
        pathComplete = true;
    }

    private void OnDrawGizmos() {
        if (path.Count > 0) {
            GridNode prevNode = null;
            Gizmos.color = Color.green;
            foreach (GridNode n in path) {
                if (prevNode != null) Gizmos.DrawLine(prevNode.worldPos, n.worldPos);
                prevNode = n;
            }
        }
    }



}
