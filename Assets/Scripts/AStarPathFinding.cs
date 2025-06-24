using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinding : MonoBehaviour
{
    #region VARIABLES

    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
	
    }
    void Update()
    {
	
    }
    #endregion

    #region FUNCTIONS
    public static List<Vector3> FindPath(HexagonTile startHex, HexagonTile targetHex)
    {
        // A* open and closed lists
        List<HexagonTile> openList = new List<HexagonTile>();
        HashSet<HexagonTile> closedList = new HashSet<HexagonTile>();

        // Dictionaries for gCost, hCost, and parent tracking
        Dictionary<HexagonTile, float> gCost = new Dictionary<HexagonTile, float>();
        Dictionary<HexagonTile, float> hCost = new Dictionary<HexagonTile, float>();
        Dictionary<HexagonTile, float> fCost = new Dictionary<HexagonTile, float>();
        Dictionary<HexagonTile, HexagonTile> cameFrom = new Dictionary<HexagonTile, HexagonTile>();

        // Initialize start hex
        openList.Add(startHex);
        gCost[startHex] = 0;
        hCost[startHex] = Vector3.Distance(startHex.buildPoint.position, targetHex.buildPoint.position);
        fCost[startHex] = gCost[startHex] + hCost[startHex];

        while (openList.Count > 0)
        {
            // Get hexagon with the lowest fCost
            HexagonTile current = openList[0];
            foreach (var hex in openList)
            {
                if (fCost[hex] < fCost[current])
                {
                    current = hex;
                }
            }

            // If we reached the target, construct the path
            if (current == targetHex)
            {
                return ReconstructPath(cameFrom, current);
            }

            openList.Remove(current);
            closedList.Add(current);

            // Evaluate neighbors
            foreach (var neighbor in current.adjacentTiles)
            {
                if (neighbor.isUsed || closedList.Contains(neighbor))
                    continue;

                float tentativeGCost = gCost[current] + Vector3.Distance(current.buildPoint.position, neighbor.buildPoint.position);

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                else if (tentativeGCost >= gCost[neighbor])
                {
                    continue;
                }

                // Update path information
                cameFrom[neighbor] = current;
                gCost[neighbor] = tentativeGCost;
                hCost[neighbor] = Vector3.Distance(neighbor.buildPoint.position, targetHex.buildPoint.position);
                fCost[neighbor] = gCost[neighbor] + hCost[neighbor];
            }
        }

        // No path found
        return null;
    }
    public static List<Vector3> FindPath(OfflineHexagon startHex, OfflineHexagon targetHex)
    {
        // A* open and closed lists
        List<OfflineHexagon> openList = new List<OfflineHexagon>();
        HashSet<OfflineHexagon> closedList = new HashSet<OfflineHexagon>();

        // Dictionaries for gCost, hCost, and parent tracking
        Dictionary<OfflineHexagon, float> gCost = new Dictionary<OfflineHexagon, float>();
        Dictionary<OfflineHexagon, float> hCost = new Dictionary<OfflineHexagon, float>();
        Dictionary<OfflineHexagon, float> fCost = new Dictionary<OfflineHexagon, float>();
        Dictionary<OfflineHexagon, OfflineHexagon> cameFrom = new Dictionary<OfflineHexagon, OfflineHexagon>();

        // Initialize start hex
        openList.Add(startHex);
        gCost[startHex] = 0;
        hCost[startHex] = Vector3.Distance(startHex.buildPoint.position, targetHex.buildPoint.position);
        fCost[startHex] = gCost[startHex] + hCost[startHex];

        while (openList.Count > 0)
        {
            // Get hexagon with the lowest fCost
            OfflineHexagon current = openList[0];
            foreach (var hex in openList)
            {
                if (fCost[hex] < fCost[current])
                {
                    current = hex;
                }
            }

            // If we reached the target, construct the path
            if (current == targetHex)
            {
                return ReconstructPath(cameFrom, current);
            }

            openList.Remove(current);
            closedList.Add(current);

            // Evaluate neighbors
            foreach (var neighbor in current.adjacentTiles)
            {
                if (neighbor.isUsed || closedList.Contains(neighbor))
                    continue;

                float tentativeGCost = gCost[current] + Vector3.Distance(current.buildPoint.position, neighbor.buildPoint.position);

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                else if (tentativeGCost >= gCost[neighbor])
                {
                    continue;
                }

                // Update path information
                cameFrom[neighbor] = current;
                gCost[neighbor] = tentativeGCost;
                hCost[neighbor] = Vector3.Distance(neighbor.buildPoint.position, targetHex.buildPoint.position);
                fCost[neighbor] = gCost[neighbor] + hCost[neighbor];
            }
        }

        // No path found
        return null;
    }

    private static List<Vector3> ReconstructPath(Dictionary<HexagonTile, HexagonTile> cameFrom, HexagonTile current)
    {
        List<Vector3> path = new List<Vector3>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current.buildPoint.position);
            current = cameFrom[current];
        }
        path.Add(current.buildPoint.position); // Add the start hex
        path.Reverse();
        return path;
    }
    private static List<Vector3> ReconstructPath(Dictionary<OfflineHexagon, OfflineHexagon> cameFrom, OfflineHexagon current)
    {
        List<Vector3> path = new List<Vector3>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current.buildPoint.position);
            current = cameFrom[current];
        }
        path.Add(current.buildPoint.position); // Add the start hex
        path.Reverse();
        return path;
    }
    #endregion
}
