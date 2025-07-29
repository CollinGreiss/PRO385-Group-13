using UnityEngine;

public class Card
{


    public string cardName;
    public string cardDescription;
    public Sprite cardImage;
    public int cardHealth;
    public int cardAttack;
    public int cardCost;
    public CardType cardType;
    public CardArea cardArea;
    public bool activeOnPlay = false;

}



public enum CardType
{

    Creature,
    Spell,
    Landscape

}

public enum CardArea
{

    empty,
    forest,
    lava,
    water,
    desert

}

public class CardDatabase
{

    public static Card[] cards = {

        new Card { cardName = "Forest Guardian", cardDescription = "A powerful creature that protects the forest.", cardImage = null, cardHealth = 5, cardAttack = 3, cardCost = 4, cardType = CardType.Creature, cardArea = CardArea.forest },
        new Card { cardName = "Lava Burst", cardDescription = "Deals damage to all creatures in the lava area.", cardImage = null, cardHealth = 0, cardAttack = 0, cardCost = 3, cardType = CardType.Spell, cardArea = CardArea.lava },
        new Card { cardName = "Water Spirit", cardDescription = "A mystical creature that thrives in water.", cardImage = null, cardHealth = 4, cardAttack = 2, cardCost = 3, cardType = CardType.Creature, cardArea = CardArea.water },
        new Card { cardName = "Desert Mirage", cardDescription = "Creates an illusion in the desert area.", cardImage = null, cardHealth = 0, cardAttack = 0, cardCost = 2, cardType = CardType.Spell, cardArea = CardArea.desert },
        new Card { cardName = "Forest", cardDescription = "A lush area filled with trees and wildlife.", cardImage = null, cardHealth = 0, cardAttack = 0, cardCost = 0, cardType = CardType.Landscape, cardArea = CardArea.forest },
        new Card { cardName = "Lava", cardDescription = "A dangerous area filled with molten rock.", cardImage = null, cardHealth = 0, cardAttack = 0, cardCost = 0, cardType = CardType.Landscape, cardArea = CardArea.lava },

    };

    public static Card GetCardByName(string name)
    {
        foreach (var card in cards)
        {
            if (card.cardName == name)
            {
                return card;
            }
        }
        return null;
    }

}