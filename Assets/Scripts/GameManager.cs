using System.Collections.Generic;
using NUnit.Framework;
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

    public bool isReady = false;
    public bool isRemoteReady = false;

    public Board board;

    public PlayerSide player1Side;
    public PlayerSide player2Side;

    public int creatureIndex = 0;

    public int turn = -1;

    private void Start()
    {

        NetworkManager.Instance.CommandReceived += OnCommandReceived;

    }

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

    public void IsReadyToStart()
    {

        NetworkManager.Instance.SendCommand("GameReady");
        isReady = true;
        if (isRemoteReady) InitializeGame();

    }

    public void OnCommandReceived(string command)
    {
        
        PlayerSide side = turn % 2 == 0 ? player1Side : player2Side;

        switch (command.Substring(0, command.IndexOf(':')))
        {
            case "GameReady":
                isRemoteReady = true;
                if (isReady) InitializeGame();
                break;

            case "PlayCard":
                string[] parts = command.Split(':');
                if (parts.Length < 3) return;

                string cardName = parts[1];
                int areaIndex = int.Parse(parts[2]);

                Card card = CardDatabase.GetCardByName(cardName);
                if (card == null) return;

                PlayCard(card, areaIndex);
                break;

            case "MoveCreature":
                string[] moveParts = command.Split(':');
                if (moveParts.Length < 3) return;

                int creatureId = int.Parse(moveParts[1]);
                int targetAreaIndex = int.Parse(moveParts[2]);

                Creature creature = FindCreatureById(creatureId);
                if (creature == null) return;

                PlayerArea targetArea = side.areas[targetAreaIndex];
                if (targetArea == null) return;

                ActivateCreatureMove(creature, targetArea);
                break;

            default:
                Debug.LogWarning($"Unknown command received: {command}");
                break;
        }

    }

    private Creature FindCreatureById(int id)
    {
        foreach (var area in player1Side.areas)
        {
            foreach (var creature in area.creatures)
            {
                if (creature.id == id) return creature;
            }
        }

        foreach (var area in player2Side.areas)
        {
            foreach (var creature in area.creatures)
            {
                if (creature.id == id) return creature;
            }
        }

        return null;
    }   

    public void PlayCard(Card card, int area)
    {
        if (turn < 0)
        {
            Debug.LogError("Game not initialized. Call InitializeGame() first.");
            return;
        }

        PlayerSide side = turn % 2 == 0 ? player1Side : player2Side;

        if (side.power < card.cardCost) return;
        side.power -= card.cardCost;

        NetworkManager.Instance.SendCommand($"PlayCard:{card.cardName}:{area}");

        switch (card.cardType)
        {

            case CardType.Creature:
                Creature newCreature = new Creature(card, creatureIndex++);
                side.areas[area].creatures.Add(newCreature);
                Debug.Log($"Played creature: {newCreature.creatureName} in area {area}");
                break;

            case CardType.Spell:
                // Handle spell logic here
                Debug.Log($"Played spell: {card.cardName}");
                break;

            case CardType.Landscape:
                // Handle landscape logic here
                Debug.Log($"Played landscape: {card.cardName}");
                break;

            default:
                Debug.LogWarning("Unknown card type!");
                break;

        }

        board.UpdateBoard();

    }

    public void ActivateCreatureMove(Creature creature, PlayerArea targ)
    {

        if (!creature.isActive) return;
        NetworkManager.Instance.SendCommand($"MoveCreature:{creature.id}:{targ.GetAreaType()}");

        creature.currentArea.creatures.Remove(creature);
        creature.currentArea = targ;
        targ.creatures.Add(creature);
        creature.currentAreaType = targ.GetAreaType();

        creature.isActive = false;

    }

    public void ActivateCreatureAttack(Creature attacker, Creature defender)
    {

        if (!attacker.isActive) return;
        NetworkManager.Instance.SendCommand($"AttackCreature:{attacker.id}:{defender.id}");

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
