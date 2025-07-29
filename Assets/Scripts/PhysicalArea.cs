using System.Collections.Generic;
using UnityEngine;

public class PhysicalArea : MonoBehaviour
{

    private List<Creature> creatures;
    private CardArea areaType;

    [Header("Visual Prefabs or Objects by Area Type")]
    public GameObject forestVisual;
    public GameObject lavaVisual;
    public GameObject waterVisual;
    public GameObject desertVisual;
    public GameObject emptyVisual;

    private Dictionary<CardArea, GameObject> visuals;

    void Awake()
    {
        visuals = new Dictionary<CardArea, GameObject>
        {
            { CardArea.forest, forestVisual },
            { CardArea.lava, lavaVisual },
            { CardArea.water, waterVisual },
            { CardArea.desert, desertVisual },
            { CardArea.empty, emptyVisual },
        };
    }

    public void SetAreaType(CardArea newAreaType)
    {
        areaType = newAreaType;
        UpdateVisuals();
    }

    public void SetCreatures(List<Creature> newCreatures)
    {
        creatures = newCreatures;
    }

    private void UpdateVisuals()
    {
        foreach (var visual in visuals.Values)
        {
            if (visual != null) visual.SetActive(false);
        }

        if (visuals.TryGetValue(areaType, out GameObject visualToShow) && visualToShow != null)
        {
            visualToShow.SetActive(true);
        }
    }

}
