using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder
{
    public List<OverlayTile> getAtkTilesInRange(List<OverlayTile> inMovRangeTiles, int[] atkRange, bool onlyOut=false)
    {
        //Take all the inMovRangeTiles, and expand outwards without descrimination atkRange number of times
        var inAtkRangeTiles = new List<OverlayTile>();
        inAtkRangeTiles.AddRange(inMovRangeTiles);

        int minRange = atkRange.Min();
        int maxRange = atkRange.Max();

        // get all neighbors, only take the new items, add new items
        for (int i = 0; i < maxRange; i++)
        {
            int loopNum = inAtkRangeTiles.Count-1;
            for (int j = loopNum; j > 0; j--)
            {
                inAtkRangeTiles.AddRange(MapManager.Instance.getAdjNodes(inAtkRangeTiles[j]).Distinct().ToList());
            }
        }

        //Eventually, prune values lower than the minRange
        //COULD BE BUG WITH inAtkRangeTiles.Count-1. IT BROKE SINGLE TILE VERSION SO IT COULB BREAK THIS ONE?

        if (onlyOut)
        {
           inAtkRangeTiles = inAtkRangeTiles.Except(inMovRangeTiles).ToList();
        }  //IMPORTANT TRICK, GETS OUTER ATTACK TILES
        return inAtkRangeTiles.Distinct().ToList();;
    }
    
//IF MANHATTAN DISTANCE == RANGE, ADD TILE

    public List<OverlayTile> getAtkTilesFromSingle(OverlayTile tile, int[] atkRange)
    {
        var inAtkRangeTiles = new List<OverlayTile>
        {
            tile
        };

        int minRange = atkRange.Min();
        int maxRange = atkRange.Max();

        inAtkRangeTiles.AddRange(MapManager.Instance.getAdjNodes(inAtkRangeTiles[0]).Distinct().ToList());

        for (int i = 0; i < maxRange; i++)
        {
            int loopNum = inAtkRangeTiles.Count-1;
            for (int j = loopNum; j > 0; j--)
            {
                inAtkRangeTiles.AddRange(MapManager.Instance.getAdjNodes(inAtkRangeTiles[j]).Distinct().ToList());
            }
        }

        //Debug.Log($"PrePrune: {inAtkRangeTiles.Count}");

        for (int i = inAtkRangeTiles.Count-1; i > 0; i--)
        {
            var distance = GetManhattanDistance(tile, inAtkRangeTiles[i]);
            if (distance < minRange || distance > maxRange)
            {   
                //Debug.Log($"Removing Tile {inAtkRangeTiles[i]}");
                inAtkRangeTiles.Remove(inAtkRangeTiles[i]);
            }
        }

        for (int i = inAtkRangeTiles.Count-1; i > 0; i--) //Debug.Log($"{inAtkRangeTiles[i].gridLocation}");
        inAtkRangeTiles.Remove(tile);

        //Debug.Log($"PostPrune: {inAtkRangeTiles.Count}");

        return inAtkRangeTiles;
    }

    public List<OverlayTile> getMovTilesInRange(OverlayTile startTile, int range, bool isPlayer)
    {
        //Declare three lists, frontier, explored, and InRangeTiles
        var frontier = new List<OverlayTile>();
        var explored = new List<OverlayTile>();
        var InRangeTiles = new List<OverlayTile>();

        // start tile path cost = 0
        startTile.pathCost = 0;

        // initialize frontier and inrangetiles with start tile
        frontier.Add(startTile);
        InRangeTiles.Add(startTile);

        // while frontier is not empty
        while(frontier.Any())
        {  
            //current tile = last member of frontier
            var tile = frontier.First();
            frontier.Remove(tile); 
            explored.Add(tile);

            if (tile.pathCost <= range)
            {
                InRangeTiles.Add(tile);
            }

            //get neighbor tiles
            var neighbors = MapManager.Instance.getAdjNodes(tile);

            //Remove explored
            RemoveifContainedIn(neighbors, explored);

            //for each neighbor
            foreach (var neighbor in neighbors)
            {
                var newCost = tile.pathCost + neighbor.movCost;
                if (isPlayer && !neighbor.occupiedEnemy) //player
                {
                    if (newCost < neighbor.pathCost)
                    {
                        neighbor.previous = tile;
                        neighbor.pathCost = newCost;
                    }
                    if (newCost <= range)
                        frontier.Add(neighbor);
                }
                if (!isPlayer && !neighbor.occupiedPlayer)
                {
                    if (newCost < neighbor.pathCost)
                    {
                        neighbor.previous = tile;
                        neighbor.pathCost = newCost;
                    }
                    if (newCost <= range)
                        frontier.Add(neighbor);
                }
            }
        }
   
        //loop through explored and set all the path costs back to 99 just before returning
        foreach (var tile in explored)
            tile.pathCost = 99;

        return InRangeTiles.Distinct().ToList();
    }

    private void RemoveifContainedIn(List<OverlayTile> ListToRemove, List<OverlayTile> ListToCheck)
    {
        if (ListToRemove.Count <= 0 || ListToCheck.Count <= 0)
            return;

        for (int i = ListToRemove.Count() - 1; i >= 0; i--)
        {
            if (ListToCheck.Contains(ListToRemove[i]))
            {
                ListToRemove.Remove(ListToRemove[i]);
            }
        }
    }
    
    private int GetManhattanDistance(OverlayTile nodeA, OverlayTile nodeB)
    {
        return Mathf.Abs(nodeA.gridLocation.x - nodeB.gridLocation.x) + Mathf.Abs(nodeA.gridLocation.y - nodeB.gridLocation.y);
    }
}
