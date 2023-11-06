using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehavior: MonoBehaviour
{
    CharacterManager enemy;

    enum Behavior 
    {
        passive,
        agressive,
        defensive,
        boss
    }
    Behavior behavior = Behavior.passive;

    CombatFormulas CF;

    void Start()
    {
        CF = new CombatFormulas();
        enemy = GetComponent<CharacterManager>();
    }

    public void BeginAction()
    {
        switch (behavior)
        {
            case Behavior.passive:
                Passive();
                break;

            case Behavior.agressive:
                Agressive();
                break;

            case Behavior.defensive:
                Defensive();
                break;

            case Behavior.boss:
                break;

            default:
                break;
        }
    }

    //ENEMY BEHAVIOR PATTERNS
    void Passive()
    {
        //If there is a player in your range, go attack him
        //If there are multiple player units in your range check for a kill
        //No kill? Optimize damage
        Debug.Log("Made it to Passive AI");
        var playersReachable = PlayersReachable(enemy.inAtkRangeTiles); //Update when make weapons to account for attack range
        if (!playersReachable.Any())
            StartCoroutine(EnemyWait());
        
        if (playersReachable.Count >= 1)
        {
            //Get closest neighbor tile and pathfind to it
            enemy.target = playersReachable.First();
            var destinationTile = ClosetInRangeTile(MapManager.Instance.getAdjNodes(enemy.target.activeTile));
            StartCoroutine (MovetoAttack(destinationTile));
        }
        else
        {
            StartCoroutine (EnemyWait());
        }
        //else if (playersReachable.Count >= 1)
        //{
            //kill/damage check
        //}

        //Now you attack
        StartCoroutine (EnemyAttack());
        
    }

    IEnumerator MovetoAttack(OverlayTile destinationTile)
    {
        yield return new WaitForSeconds(2);
        enemy.StartPathFinding(destinationTile);
        yield return new WaitForSeconds(2);
        StartCoroutine (EnemyAttack());
    }

    IEnumerator EnemyAttack()
    {
        //ATTACK
        if (!CF.DOES_HIT(enemy, enemy.target))
        {
            // MISS FUNCTION
            Debug.Log($"Enemy Missed");
            // play attacker attack animation
            // play defender dodge animation (little side shimmy) and make whoosh noise
        }
        else
        {
            Debug.Log($"Enemy Hit");
            // PROCEED WITH ATTACK
            // play attacker attack animation
            // play defender hurt animation (turn to white sprite?)
            // subtract from their HP value. If below zero, kill enemy and end combat
            enemy.target._stats.HP -= CF.DMG_PHYS(enemy, enemy.target);
            if (enemy.target._stats.HP <= 0) 
            {
                StartCoroutine(enemy.target.KillUnit());
                enemy.OutOfActions();
                //target = null;
                yield return new WaitForSeconds(1);
                yield break;
            }
        }

        yield return new WaitForSeconds(1);

        // COUNTER ATTACK 
        if (enemy.target.CanAttack() && !CF.DOES_HIT(enemy.target, enemy))
        {
            // MISS FUNCTION
            Debug.Log($"Player Miss");
            // play defender attack animation
            // play attacker dodge animation (little side shimmy) and make whoosh noise
        }
        else if (enemy.target.CanAttack())
        {
            Debug.Log($"Player Hit");
            // PROCEED WITH ATTACK
            // play defender attack animation
            // play attacker hurt animation (turn to white sprite?)
            // subtract from their HP value. If below zero, kill player
            enemy._stats.HP -= CF.DMG_PHYS(enemy.target, enemy);
            if (enemy._stats.HP <= 0) 
            {
                StartCoroutine(enemy.target.KillUnit());
                enemy.OutOfActions();
                //target = null;
                yield return new WaitForSeconds(1);
                yield break;
            }
        }
        enemy.OutOfActions();
    }

    void Agressive()
    {
        //If no players in range, charge towards either a spot on the map or the closest player
        //Reuse killcheck/damage optimization function from Passive()
    }

    void Defensive()
    {
        //Do not move unitl attacked.
        //if (attacked)
        StartCoroutine(EnemyWait());
        behavior = Behavior.agressive;
    }

    // ENEMY ACTIONS
    IEnumerator EnemyWait()
    {
        yield return new WaitForSeconds(1);
        enemy.OutOfActions();
    }

    // TILE CHECKING
    OverlayTile ClosetInRangeTile(List<OverlayTile> playerNeighbors)
    {
        int lowestDistance = 255;
        OverlayTile closestNeighbor = null;
        foreach (var neighbor in playerNeighbors)
        {
            int currentDistance = GetManhattanDistance(enemy.activeTile, neighbor);
            if (currentDistance < lowestDistance)
            {
               lowestDistance = currentDistance;
               closestNeighbor = neighbor; 
            } 
        }
        return closestNeighbor;
    }

    private int GetManhattanDistance(OverlayTile nodeA, OverlayTile nodeB)
    {
        return Mathf.Abs(nodeA.gridLocation.x - nodeB.gridLocation.x) + Mathf.Abs(nodeA.gridLocation.y - nodeB.gridLocation.y);
    }

    List<CharacterManager> PlayersReachable(List<OverlayTile> inAtkRangeTiles)
    {
        // Check if there is a player in your inRangeTiles. Update accordingly when weapons have ranges 
        var playerList = new List<CharacterManager>();

        foreach (var tile in inAtkRangeTiles)
        {
            if (tile.occupiedPlayer == true)
            {
                playerList.Add(tile.playerOn);
            }
        }
        return playerList;
    }
}


