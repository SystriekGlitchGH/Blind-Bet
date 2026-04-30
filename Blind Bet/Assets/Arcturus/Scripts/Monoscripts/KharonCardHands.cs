using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "KharonCardHands", menuName = "Scriptable Objects/KharonCardHands")]
public class KharonCardHands : ScriptableObject
{
    [Serializable]
    public struct KharonHand
    {
        public Card[] hand;
    }
    [Serializable]
    public struct KharonBuffDebuff
    {
        public string name;
        public string effect;
    }
    public List<KharonHand> cardHands = new List<KharonHand>();
    public List<KharonBuffDebuff> buffs = new List<KharonBuffDebuff>();
    public List<KharonBuffDebuff> debuffs = new List<KharonBuffDebuff>();

}
