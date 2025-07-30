using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    public NetworkManager networkManager;

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

    public bool isReady = false;
    public bool isRemoteReady = true;

    public Creature selectedCreature = null;
    public Camera arCam;

    public Card playingCard = null;

    int creatureIndex = 0;

    bool isHost = true;

    private void Start()
    {

        arCam = Camera.main;
        networkManager = GetComponent<NetworkManager>();

    }

    public void CheckHit(RaycastHit hit)
    {

        Debug.Log("Tapped: " + hit.transform.name);

        if (playingCard != null)
        {

            Debug.Log(hit.transform.name);

            if (hit.transform.gameObject.TryGetComponent(out PhysicalArea area))
            {

                int areaIndex = area.id;
                if (areaIndex == -1) return;
                PlayCard(playingCard, areaIndex);
                playingCard = null;
                board.UpdateBoard();

            }

            return;

        }

        if (selectedCreature == null && hit.transform.TryGetComponent<Creature>(out Creature hitCreature))
        {

            selectedCreature = hitCreature;
            Debug.Log("Selected Creature: " + selectedCreature.creatureName);

        }
        else if (selectedCreature == null) return;

        // Creature selected, now check for action

        if (hit.transform.TryGetComponent<Creature>(out Creature targetCreature))
        {

            GameManager.Instance.ActivateCreatureAttack(selectedCreature, targetCreature);

        }
        else if (hit.transform.TryGetComponent<PlayerArea>(out PlayerArea targetArea))
        {

            GameManager.Instance.ActivateCreatureMove(selectedCreature, targetArea);

        }

        selectedCreature = null;

    }

    public void InitializeGame()
    {

        player1Side = new PlayerSide();
        player2Side = new PlayerSide();

        // Initialize areas
        for (int i = 0; i < 4; i++)
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

        isReady = true;
        SendCommand(isHost, "GameReady");
        if (isRemoteReady) InitializeGame();

    }

    [Rpc(SendTo.Everyone)]
    public void SendCommand(bool hostIsSender, string command)
    {

        if (isHost && hostIsSender) return; // If host is sending, skip processing on host side
        if (!isHost && !hostIsSender) return; // If client is sending, skip processing on client side
        OnCommandReceived(command);
        
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

    private int GetAreaIndex(PlayerArea area)
    {

        for (int i = 0; i < player1Side.areas.Length; i++)
        {
            if (player1Side.areas[i] == area) return i;
        }

        return -1; // Not found

    }

    public void PlayCard(Card card, int area)
    {
        if (turn < 0)
        {
            Debug.LogError("Game not initialized.");
            return;
        }

        SendCommand(isHost, "PlayCard:" + card.cardName + ":" + area);

        PlayerSide side = turn % 2 == 0 ? player1Side : player2Side;
        PlayerArea targetArea = side.areas[area];

        if (side.power < card.cardCost) return;
        side.power -= card.cardCost;

        switch (card.cardType)
        {
            case CardType.Creature:
                Creature newCreature = new Creature(card, creatureIndex++);
                targetArea.creatures.Add(newCreature);
                newCreature.currentArea = targetArea;

                // Place creature visual
                board.PlaceCreatureVisual(newCreature, area, turn % 2 == 0);
                break;

            case CardType.Landscape:

                side.areas[area].SetAreaType(card.cardArea);
                Debug.Log($"Played landscape: {card.cardName}");
                targetArea.SetAreaType(card.cardArea);
                board.UpdateBoard(); // to refresh area visuals
                break;

            case CardType.Spell:
                Debug.Log($"Played spell: {card.cardName}");
                break;
        }

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

        SendCommand(isHost, "ActivateCreatureMove:" + creature.id + ":" + GetAreaIndex(targ));

        if (GetAreaIndex(targ) == -1)
        {

            player2Side.health -= creature.attack;
            Debug.Log($"{creature.creatureName} attacked the opponent's health directly!");
            if (player2Side.health <= 0)
            {
                Debug.Log("Player 1 wins!");
            }

        }

        creature.currentArea.creatures.Remove(creature);
        creature.currentArea = targ;
        targ.creatures.Add(creature);
        creature.currentAreaType = targ.GetAreaType();


        creature.isActive = false;

    }

    public void ActivateCreatureAttack(Creature attacker, Creature defender)
    {

        if (!attacker.isActive) return;

        SendCommand(isHost, "ActivateCreatureAttack:" + attacker.id + ":" + defender.id);

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

        // Debug
        turn++;
        StartTurn();

    }

    public void StartTurn()
    {

        PlayerSide currentPlayer = turn == 0 ? player1Side : player2Side;
        currentPlayer.power = turn / 2 + 3;

    }

}
