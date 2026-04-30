using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
public class KharonBuffDebuffField : MonoBehaviour
{
    public KharonCardHands kharonHands;
    public TMP_Text buffTitle;
    public TMP_Text buffDesc;
    public TMP_Text debuffTitle;
    public TMP_Text debuffDesc;
    Random rand = new Random();
    void Awake()
    {
        KharonCardHands.KharonBuffDebuff kharonBuff = kharonHands.buffs[rand.Next(0,kharonHands.buffs.Count)];
        buffTitle.text = kharonBuff.name;
        buffDesc.text = kharonBuff.effect;
        KharonCardHands.KharonBuffDebuff kharonDebuff = kharonHands.debuffs[rand.Next(0,kharonHands.debuffs.Count)];
        debuffTitle.text = kharonDebuff.name;
        debuffDesc.text = kharonDebuff.effect;
    }
}
