using System.Collections.Generic;
using UnityEngine;

public class PhysicalArea : MonoBehaviour
{
    private CardArea areaType;
    private List<Creature> creatures;

    [Header("Prefabs for Each Area Type")]
    public GameObject forestPrefab;
    public GameObject lavaPrefab;
    public GameObject waterPrefab;
    public GameObject desertPrefab;
    public GameObject emptyPrefab;

    [Header("Optional Visual Root (child under PhysicalArea)")]
    public Transform visualRoot;

    private Dictionary<CardArea, GameObject> prefabMap;
    private GameObject currentVisualInstance;

    [Header("Creature Visual")]
    [Tooltip("Select creature prefab index to display")]
    public int selectedCreatureIndex = -1;

    private GameObject currentCreatureInstance;

    private Board board; // reference to Board

    void Awake()
    {
        prefabMap = new Dictionary<CardArea, GameObject>
        {
            { CardArea.forest, forestPrefab },
            { CardArea.lava, lavaPrefab },
            { CardArea.water, waterPrefab },
            { CardArea.desert, desertPrefab },
            { CardArea.empty, emptyPrefab },
        };

        board = FindFirstObjectByType<Board>();

        UpdateVisual();
        UpdateCreatureVisualAndAreaType();
    }

    public void SetAreaType(CardArea newAreaType)
    {
        if (areaType == newAreaType) return;
        areaType = newAreaType;
        UpdateVisual();
    }

    public void SetCreatures(List<Creature> newCreatures)
    {
        creatures = newCreatures;
    }

    private void UpdateVisual()
    {
        if (currentVisualInstance != null)
        {
            if (Application.isPlaying)
                Destroy(currentVisualInstance);
            else
                DestroyImmediate(currentVisualInstance);
        }

        if (prefabMap.TryGetValue(areaType, out GameObject prefab) && prefab != null)
        {
            Transform parent = visualRoot != null ? visualRoot : this.transform;

            currentVisualInstance = Instantiate(prefab, parent);
            currentVisualInstance.transform.localPosition = Vector3.zero;
            currentVisualInstance.transform.localRotation = Quaternion.identity;
            currentVisualInstance.transform.localScale = Vector3.one;
        }
    }

    private void UpdateCreatureVisualAndAreaType()
    {
        // Destroy old creature visual
        if (currentCreatureInstance != null)
        {
            if (Application.isPlaying)
                Destroy(currentCreatureInstance);
            else
                DestroyImmediate(currentCreatureInstance);
        }

        // Handle no board or no creature list
        if (board == null || board.creaturePrefabs == null || board.creaturePrefabs.Length == 0)
        {
            SetAreaType(CardArea.empty);
            return;
        }

        // Handle invalid index (less than 0 or beyond bounds)
        if (selectedCreatureIndex < 0 || selectedCreatureIndex >= board.creaturePrefabs.Length)
        {
            SetAreaType(CardArea.empty);
            return;
        }

        var mapping = board.creaturePrefabs[selectedCreatureIndex];

        // Handle empty slot in the list
        if (mapping == null || mapping.creaturePrefab == null)
        {
            SetAreaType(CardArea.empty);
            return;
        }

        // Update area type to match the creature
        SetAreaType(mapping.creatureAreaType);

        // Instantiate creature visual
        Transform parent = visualRoot != null ? visualRoot : this.transform;
        currentCreatureInstance = Instantiate(mapping.creaturePrefab, parent);
        currentCreatureInstance.transform.localPosition = new Vector3(0, 0.5f, 0);
        currentCreatureInstance.transform.localRotation = Quaternion.identity;
        //currentCreatureInstance.transform.localScale = new Vector3(0.35f, 1f, 0.35f);
    }



#if UNITY_EDITOR
    private int lastSelectedCreatureIndex = -1;

    void OnValidate()
    {
        board = FindFirstObjectByType<Board>();

        if (selectedCreatureIndex != lastSelectedCreatureIndex)
        {
            lastSelectedCreatureIndex = selectedCreatureIndex;
            UpdateCreatureVisualAndAreaType();
        }
    }
#endif
}
