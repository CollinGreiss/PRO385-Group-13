using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    
    public PhysicalArea[] player1Areas;
    public PhysicalArea[] player2Areas;

    public CreatureVisualMapping[] creaturePrefabs;
    private Dictionary<string, GameObject> creaturePrefabDict;

    void Awake()
    {
        creaturePrefabDict = new Dictionary<string, GameObject>();
        foreach (var mapping in creaturePrefabs)
        {
            if (!creaturePrefabDict.ContainsKey(mapping.creatureName))
                creaturePrefabDict.Add(mapping.creatureName, mapping.creaturePrefab);
        }
    }

    public void UpdateBoard()
    {

        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) return;

        PlayerSide player1 = gameManager.player1Side;
        PlayerSide player2 = gameManager.player2Side;

        for (int i = 0; i < player1.areas.Length; i++)
        {
            player1Areas[i].SetAreaType(player1.areas[i].GetAreaType());
            player1Areas[i].SetCreatures(player1.areas[i].creatures);
        }

        for (int i = 0; i < player2.areas.Length; i++)
        {
            player2Areas[i].SetAreaType(player2.areas[i].GetAreaType());
            player2Areas[i].SetCreatures(player2.areas[i].creatures);
        }

    }

    public void PlaceCreatureVisual(Creature creature, int areaIndex, bool isPlayer1)
    {
        if (!creaturePrefabDict.TryGetValue(creature.creatureName, out GameObject prefab))
        {
            Debug.LogWarning($"No prefab assigned for creature: {creature.creatureName}");
            return;
        }

        PhysicalArea targetArea = isPlayer1 ? player1Areas[areaIndex] : player2Areas[areaIndex];

        // Instantiate under the visual root or default
        Transform parent = targetArea.visualRoot != null ? targetArea.visualRoot : targetArea.transform;
        GameObject instance = Instantiate(prefab, parent);

        // Optionally position it nicely
        instance.transform.localPosition = new Vector3(0, 0.5f, 0); // adjust height or layout
        instance.transform.localRotation = Quaternion.identity;
    }

}
