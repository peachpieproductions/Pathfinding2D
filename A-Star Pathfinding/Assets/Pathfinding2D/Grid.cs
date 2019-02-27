using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public Vector2 gridOrigin;
    public float cellSize;

    GridNode[,] grid;
    float cellRadius;
    public int gridSizeX, gridSizeY;

    public int TotalCellCount {
        get {
            return gridSizeX * gridSizeY;
        }
    }

    private void Awake() {
        cellRadius = cellSize * .5f;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / cellSize);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / cellSize);
        CreateGrid();
    }

    void CreateGrid() {
        grid = new GridNode[gridSizeX, gridSizeY];
        gridOrigin = (Vector2)transform.position - Vector2.right * gridWorldSize.x/2 - Vector2.up * gridWorldSize.y/2;

        //Set Unwalkable Cells
        UpdateUnwalkableCells();
    }

    public void UpdateUnwalkableCells() {
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector2 worldPoint = gridOrigin + Vector2.right * (x * cellSize + cellRadius) + Vector2.up * (y * cellSize + cellRadius);
                bool walkable = !(Physics2D.OverlapBox(worldPoint, cellSize * Vector2.one * .8f, 0, unwalkableMask));
                grid[x, y] = new GridNode(walkable, worldPoint, x, y);
            }
        }
    }

    public int GetDistance(GridNode nodeA, GridNode nodeB) {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY) return 14 * distY + 10 * (distX - distY); //More info in Episode 3 @ 14:00
        return 14 * distX + 10 * (distY - distX);
    }

    public List<GridNode> GetNeighbors(GridNode node) {
        List<GridNode> neighbors = new List<GridNode>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    public GridNode NodeFromWorldPoint(Vector2 worldPos) {
        Vector2 offsetFromOrigin = worldPos - gridOrigin;
        return grid[Mathf.Clamp(Mathf.FloorToInt(offsetFromOrigin.x / cellSize), 0, gridSizeX - 1),
            Mathf.Clamp(Mathf.FloorToInt(offsetFromOrigin.y / cellSize), 0, gridSizeY - 1)];
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null) {
            foreach(GridNode n in grid) {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawWireCube(n.worldPos, Vector3.one * (cellSize - .1f));
            }
        }
    }




}
