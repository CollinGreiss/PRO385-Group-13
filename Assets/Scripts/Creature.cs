using NUnit.Framework.Constraints;
using UnityEngine;

public class Creature
{

    public Creature(Card card, int id)
    {
        creatureName = card.cardName;
        health = card.cardHealth;
        maxHealth = health;
        attack = card.cardAttack;
        areaType = card.cardArea;
        currentAreaType = areaType;
        isActive = card.activeOnPlay;
        this.id = id;
    }

    public string creatureName;
    public int health;
    public int maxHealth;
    public int attack;
    public CardArea areaType;
    public CardArea currentAreaType = CardArea.empty;
    public bool isActive;
    public PlayerArea currentArea;
    public int id;
    public GameObject currentVisualInstance;

    public virtual void ApplyAreaEffect()
    {
        // Default: no effect
    }

    public void DisplayInfo()
    {
        Debug.Log($"Creature: {creatureName}, Health: {health}, Attack: {attack}, Area: {areaType}");
    }

}