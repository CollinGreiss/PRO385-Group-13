using UnityEngine;


[System.Serializable]
public class CreatureVisualMapping
{
    public string creatureName;
    public GameObject creaturePrefab;
    public CardArea creatureAreaType;
    public Vector3 prefabScale = new Vector3(0.35f, 1f, 0.35f);
}
