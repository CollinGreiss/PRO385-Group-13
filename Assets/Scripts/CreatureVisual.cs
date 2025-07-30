using UnityEngine;
using UnityEngine.UI;


public class CreatureVisual : MonoBehaviour
{
    public Slider health;
    public float MaxHealth;

    public float Health { set { health.value = value / MaxHealth; } }

}
