using System.Collections.Generic;
using UnityEngine;

public class PhysicalArea : MonoBehaviour
{

    private List<Creature> creatures;
    private CardArea areaType;

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

        // Optional: if you want to initialize visual at start
        // SetAreaType(areaType);
    }

    public void SetAreaType(CardArea newAreaType)
    {
        if (areaType == newAreaType) return; // no change needed
        areaType = newAreaType;
        UpdateVisual();
    }

    public void SetCreatures(List<Creature> newCreatures)
    {
        creatures = newCreatures;
    }

    private void UpdateVisual()
    {
        // Destroy the previous visual instance properly (using Destroy for runtime)
        if (currentVisualInstance != null)
        {
            Destroy(currentVisualInstance);
        }

        if (prefabMap.TryGetValue(areaType, out GameObject prefab) && prefab != null)
        {
            // Decide where to instantiate: under visualRoot if assigned, else as child of this object
            Transform parent = visualRoot != null ? visualRoot : this.transform;

            currentVisualInstance = Instantiate(prefab, parent);

            // Reset position & rotation relative to parent
            currentVisualInstance.transform.localPosition = Vector3.zero;
            currentVisualInstance.transform.localRotation = Quaternion.identity;

            // Optional: reset scale if needed
            currentVisualInstance.transform.localScale = Vector3.one;
        }
    }

#if UNITY_EDITOR
    [Header("Editor Testing")]
    public CardArea testAreaType = CardArea.empty;
    private CardArea lastTestAreaType = CardArea.empty;

    void OnValidate()
    {
        if (testAreaType != lastTestAreaType)
        {
            lastTestAreaType = testAreaType;
            SetAreaType(testAreaType);
        }
    }
#endif

}
