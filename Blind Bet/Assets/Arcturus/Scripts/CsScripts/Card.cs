using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Scriptable Objects/Cards")]
public class Card : ScriptableObject
{
    public enum Suit
    {
        blank,diamond,heart,club,spade
    }
    public int rank;
    public Suit suit;
}
