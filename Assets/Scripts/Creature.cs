using NUnit.Framework.Constraints;
using UnityEngine;

public class Creature
{

    public Creature(Card card)
    {
        creatureName = card.cardName;
        health = card.cardHealth;
        attack = card.cardAttack;
        areaType = card.cardArea;
        currentAreaType = areaType;
        isActive = card.activeOnPlay;
    }

    public string creatureName;
    public int health;
    public int attack;
    public CardArea areaType;
    public CardArea currentAreaType = CardArea.empty;
    public bool isActive;
    public PlayerArea currentArea;


    public void DisplayInfo()
    {
        Debug.Log($"Creature: {creatureName}, Health: {health}, Attack: {attack}, Area: {areaType}");
    }

}