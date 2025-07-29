using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

     void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public Board board;

    public PlayerSide player1Side;
    public PlayerSide player2Side;

    public int turn = -1;

    public void InitializeGame()
    {

        player1Side = new PlayerSide();
        player2Side = new PlayerSide();

        // Initialize areas
        for (int i = 0; i < player1Side.areas.Length; i++)
        {
            player1Side.areas[i] = new PlayerArea();
            player2Side.areas[i] = new PlayerArea();
        }

        board = FindFirstObjectByType<Board>();
        if (board == null)
        {
            Debug.LogError("Board not found in the scene!");
            return;
        }

        turn = 0;
        board.UpdateBoard();
        StartTurn();

    }

    public void PlayCard(Card card, int area)
    {
        if (turn < 0)
        {
            Debug.LogError("Game not initialized. Call InitializeGame() first.");
            return;
        }

        PlayerSide side = turn % 2 == 0 ? player1Side : player2Side;
        PlayerArea targetArea = side.areas[area];

        Creature newCreature = card.cardName switch
        {
            "MageSkeleton" => new MageSkeleton(card),
            _ => new Creature(card)
        };

        newCreature.currentArea = targetArea;
        targetArea.creatures.Add(newCreature);

        newCreature.ApplyAreaEffect();

        bool isPlayer1 = (turn % 2 == 0);
        board.PlaceCreatureVisual(newCreature, area, isPlayer1);

        Debug.Log($"Played creature: {newCreature.creatureName} in area {area}");

        board.UpdateBoard();
    }

    public void PlaceCreatureVisual(Creature creature, int areaIndex, bool isPlayer1)
    {
        GameObject prefab = GetCreaturePrefab(creature.creatureName);
        if (prefab == null)
        {
            Debug.LogWarning($"No prefab for creature: {creature.creatureName}");
            return;
        }

        PhysicalArea area = isPlayer1 ? board.player1Areas[areaIndex] : board.player2Areas[areaIndex];
        Transform parent = area.visualRoot != null ? area.visualRoot : area.transform;

        GameObject instance = GameObject.Instantiate(prefab, parent);

        instance.transform.localPosition = new Vector3(0, 0.5f, 0); // adjust as needed
        instance.transform.localRotation = Quaternion.identity;
    }

    private GameObject GetCreaturePrefab(string creatureName)
    {
        foreach (CreatureVisualMapping go in board.creaturePrefabs)
        {
            if (go.creatureName == creatureName)
                return go.creaturePrefab;
        }
        return null;
    }



    public void ActivateCreatureMove(Creature creature, PlayerArea targ)
    {

        if (!creature.isActive) return;

        creature.currentArea.creatures.Remove(creature);
        creature.currentArea = targ;
        targ.creatures.Add(creature);
        creature.currentAreaType = targ.GetAreaType();

        creature.isActive = false;

    }

    public void ActivateCreatureAttack(Creature attacker, Creature defender)
    {

        if (!attacker.isActive) return;
        defender.health -= attacker.attack;
        attacker.health -= defender.attack;

        if (defender.health <= 0)
        {
            defender.currentArea.creatures.Remove(defender);
            Debug.Log($"{defender.creatureName} has been defeated!");
        }

        if (attacker.health <= 0)
        {
            attacker.currentArea.creatures.Remove(attacker);
            Debug.Log($"{attacker.creatureName} has been defeated!");
        }

        attacker.isActive = false;
        board.UpdateBoard();

    }

    public void EndTurn()
    {

        turn++;
        turn %= 2;

    }

    public void StartTurn()
    {

        PlayerSide currentPlayer = turn == 0 ? player1Side : player2Side;
        currentPlayer.power = turn / 2 + 3;

    }

}
