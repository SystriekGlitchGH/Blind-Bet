using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "KharonCardHands", menuName = "Scriptable Objects/KharonCardHands")]
public class KharonCardHands : ScriptableObject
{
    [Serializable]
    public struct KharonHands
    {
        public Card[] hand;
    }
    public List<KharonHands> cardHands = new List<KharonHands>();
}
