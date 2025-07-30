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

        /* Creatures 
        Arachnoid,
        Crab,
        Devil,
        FieldFighter,
        Goblin,
        Golem,
        MageSkeleton,
        MechaTrooper,
        MinionSkeleton,
        QuadrupedTank,
        Skeleton,
        Turtle,
        WarriorSkeleton,
        Whale
        
        */
        
        new Card { cardName = "Arachnoid", cardDescription = "A creature with eight legs and a venomous bite.", cardImage = null, cardHealth = 3, cardAttack = 2, cardCost = 2, cardType = CardType.Creature, cardArea = CardArea.forest },
        new Card { cardName = "Crab", cardDescription = "A tough creature with a hard shell.", cardImage = null, cardHealth = 4, cardAttack = 1, cardCost = 3, cardType = CardType.Creature, cardArea = CardArea.water },
        new Card { cardName = "Devil", cardDescription = "A mischievous creature that causes chaos.", cardImage = null, cardHealth = 2, cardAttack = 3, cardCost = 4, cardType = CardType.Creature, cardArea = CardArea.lava },
        new Card { cardName = "FieldFighter", cardDescription = "A brave warrior that excels in combat.", cardImage = null, cardHealth = 5, cardAttack = 4, cardCost = 5, cardType = CardType.Creature, cardArea = CardArea.desert },
        new Card { cardName = "Goblin", cardDescription = "A small, sneaky creature that loves to steal.", cardImage = null, cardHealth = 1, cardAttack = 2, cardCost = 1, cardType = CardType.Creature, cardArea = CardArea.forest },
        new Card { cardName = "Golem", cardDescription = "A massive creature made of stone.", cardImage = null, cardHealth = 6, cardAttack = 5, cardCost = 6, cardType = CardType.Creature, cardArea = CardArea.lava },
        new Card { cardName = "MageSkeleton", cardDescription = "A magical skeleton that casts powerful spells.", cardImage = null, cardHealth = 3, cardAttack = 3, cardCost = 4, cardType = CardType.Creature, cardArea = CardArea.forest },
        new Card { cardName = "MechaTrooper", cardDescription = "A robotic soldier with advanced technology.", cardImage = null, cardHealth = 4, cardAttack = 4, cardCost = 5, cardType = CardType.Creature, cardArea = CardArea.desert },
        new Card { cardName = "MinionSkeleton", cardDescription = "A loyal skeleton that serves its master.", cardImage = null, cardHealth = 2, cardAttack = 1, cardCost = 2, cardType = CardType.Creature, cardArea = CardArea.forest },
        new Card { cardName = "QuadrupedTank", cardDescription = "A heavily armored creature that charges into battle.", cardImage = null, cardHealth = 7, cardAttack = 6, cardCost = 7, cardType = CardType.Creature, cardArea = CardArea.lava },
        new Card { cardName = "Skeleton", cardDescription = "A basic skeleton that can be easily defeated.", cardImage = null, cardHealth = 1, cardAttack = 1, cardCost = 1, cardType = CardType.Creature, cardArea = CardArea.forest },
        new Card { cardName = "Turtle", cardDescription = "A slow but sturdy creature with a protective shell.", cardImage = null, cardHealth = 5, cardAttack = 2, cardCost = 3, cardType = CardType.Creature, cardArea = CardArea.water },
        new Card { cardName = "WarriorSkeleton", cardDescription = "A skeleton warrior that fights fiercely.", cardImage = null, cardHealth = 3, cardAttack = 4, cardCost = 4, cardType = CardType.Creature, cardArea = CardArea.forest },
        new Card { cardName = "Whale", cardDescription = "A giant creature of the sea with immense power.", cardImage = null, cardHealth = 8, cardAttack = 7, cardCost = 8, cardType = CardType.Creature, cardArea = CardArea.water },

        /* Spells WIP */

        /* Landscapes 
        
        Forest,
        Lava,
        Water,
        Desert

        */
        new Card { cardName = "Forest", cardDescription = "A lush area filled with trees and wildlife.", cardImage = null, cardHealth = 0, cardAttack = 0, cardCost = 0, cardType = CardType.Landscape, cardArea = CardArea.forest },
        new Card { cardName = "Lava", cardDescription = "A dangerous area filled with molten rock.", cardImage = null, cardHealth = 0, cardAttack = 0, cardCost = 0, cardType = CardType.Landscape, cardArea = CardArea.lava },
        new Card { cardName = "Water", cardDescription = "A serene area filled with lakes and rivers.", cardImage = null, cardHealth = 0, cardAttack = 0, cardCost = 0, cardType = CardType.Landscape, cardArea = CardArea.water },
        new Card { cardName = "Desert", cardDescription = "A vast area filled with sand and heat.", cardImage = null, cardHealth = 0, cardAttack = 0, cardCost = 0, cardType = CardType.Landscape, cardArea = CardArea.desert }

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

    public static Card GetRandomCard()
    {
        if (cards.Length == 0) return null;
        int randomIndex = Random.Range(0, cards.Length);
        return cards[randomIndex];
    }  

}