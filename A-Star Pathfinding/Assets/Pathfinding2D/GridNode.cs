using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : IHeapItem<GridNode> {
    public bool walkable;
    public Vector2 worldPos;
    public int gridX;
    public int gridY;

    public int gCost; //distance to start
    public int hCost; //distance to end
    public GridNode parent;

    public int FCost {
        get { return gCost + hCost; }
    }

    public GridNode(bool _walkable, Vector2 _worldPos, int _gridX, int _gridY) {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    int heapIndex;
    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(GridNode nodeToCompare) {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
