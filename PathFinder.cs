using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> inRangeTiles)
    {
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closedList = new List<OverlayTile>();
        
        openList.Add(start);

        while (openList.Count > 0)
        {
            OverlayTile currentTile = openList.OrderBy(x => x.F).First(); //Return lowest F cost of openList

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            if (currentTile == end)
            {
                return GetFinishedList(start, end);
            }

            var AdjNodes = MapManager.Instance.getAdjNodes(currentTile); //HOW TO CALL FUNCTIONS FROM OTHER SCRIPTS

            foreach (var neighbor in AdjNodes)
            {
                if (closedList.Contains(neighbor) || !inRangeTiles.Contains(neighbor)) 
                {
                    continue;
                }
                neighbor.G = GetManhattanDistance(start, neighbor);
                neighbor.H = GetManhattanDistance(end, neighbor);

                neighbor.previous = currentTile;

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
            }
        }
        return new List<OverlayTile>();
    }

    private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>();
        OverlayTile currentTile = end;
        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previous;
        }
        finishedList.Reverse();

        return finishedList;
    }

    private int GetManhattanDistance(OverlayTile nodeA, OverlayTile nodeB)
    {
        return Mathf.Abs(nodeA.gridLocation.x - nodeB.gridLocation.x) + Mathf.Abs(nodeA.gridLocation.y - nodeB.gridLocation.y);
    }
}
