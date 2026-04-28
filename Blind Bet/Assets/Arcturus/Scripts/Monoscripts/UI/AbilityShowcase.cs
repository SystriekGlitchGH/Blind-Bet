using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AbilityShowcase : MonoBehaviour
{
    public TMP_Text abilityTitle;
    public TMP_Text abilityDesc;
    public Image suit;
    public Image icon;
    public Sprite[] icons;
    public Sprite[] suits;

    public void UpdateShowcase(string abilityCode, string abilityName)
    {
        // give blank area and stop method
        if(abilityCode == "na")
        {
            abilityTitle.text = "no ability";
            abilityDesc.text = "";
            icon.color = Color.clear;
            return;
        }
        // active
        else if(abilityCode == "a1" || abilityCode == "n1d" || abilityCode == "n1h" || abilityCode == "n1c" || abilityCode == "n1s")
        {
            abilityTitle.text = "ace high - no ability";
            abilityDesc.text = "";
            icon.color = Color.clear;
        }
        else if(abilityCode == "a2")
        {
            abilityTitle.text = "Pair - "+abilityName;
            abilityDesc.text = "Increases attack damage modifier by 20%";
            icon.sprite = icons[0];
        }
        else if(abilityCode == "a3")
        {
            abilityTitle.text = "Two pair - "+abilityName;
            abilityDesc.text = "Increases attack damage modifier by 20% AND increases attack speed modifier by 20%";
            icon.sprite = icons[1];
        }
        else if(abilityCode == "a4")
        {
            abilityTitle.text = "Three of a kind - "+abilityName;
            abilityDesc.text = "Increases attack size modifier by 30%";
            icon.sprite = icons[2];
        }
        else if(abilityCode == "a5")
        {
            abilityTitle.text = "Flush - "+abilityName;
            abilityDesc.text = "Summon an AOE attack that hits enemies around you - Scales with ATTACK damage";
            icon.sprite = icons[3];
        }
        else if(abilityCode == "a6")
        {
            abilityTitle.text = "Straight - "+abilityName;
            abilityDesc.text = "Send a projectile attack that pierces 1 enemy - Scales with ATTACK damage";
            icon.sprite = icons[4];
        }
        else if(abilityCode == "a7")
        {
            abilityTitle.text = "Full House - "+abilityName;
            abilityDesc.text = "After attacking, an echo attack will trigger at the same spot after a couple seconds - Scales with ATTACK damage";
            icon.sprite = icons[5];
        }
        else if(abilityCode == "a8")
        {
            abilityTitle.text = "Four of a kind - "+abilityName;
            abilityDesc.text = "Attacking hits enemies twice";
            icon.sprite = icons[6];
        }
        else if(abilityCode == "a9")
        {
            abilityTitle.text = "Five of a kind - "+abilityName;
            abilityDesc.text = "Attacking hits enemies 3 times";
            icon.sprite = icons[7];
        }
        else if(abilityCode == "a10")
        {
            abilityTitle.text = "Royal Flush - "+abilityName;
            abilityDesc.text = "Send a bomb projectile that explodes on contact with enemies and walls, or after several seconds - Scales with ATTACK damage";
            icon.sprite = icons[8];
        }
        //diamond
        else if(abilityCode == "n2d")
        {
            abilityTitle.text = "Pair - "+abilityName;
            abilityDesc.text = "Increases run speed modifier by 20%";
            icon.sprite = icons[9];
        }
        else if(abilityCode == "b3d")
        {
            abilityTitle.text = "Two pair - "+abilityName;
            abilityDesc.text = "Summon an AOE attack that deals damage and gives enemies CHILLED effect - Scales with ABILITY damage";
            icon.sprite = icons[10];
        }
        else if(abilityCode == "b4d")
        {
            abilityTitle.text = "Three of a kind - "+abilityName;
            abilityDesc.text = "Summon an AOE attack that deals damage in a cone in front of you - Scales with ABILITY damage";
            icon.sprite = icons[11];
        }
        else if(abilityCode == "n5d")
        {
            abilityTitle.text = "Flush - "+abilityName;
            abilityDesc.text = "Attacking gives enemies POISONED effect";
            icon.sprite = icons[12];
        }
        else if(abilityCode == "b6d")
        {
            abilityTitle.text = "Straight - "+abilityName;
            abilityDesc.text = "Send 5 projectiles in front of you with small spread - Scales with ABILITY damage";
            icon.sprite = icons[13];
        }
        else if(abilityCode == "b7d")
        {
            abilityTitle.text = "Full House - "+abilityName;
            abilityDesc.text = "For several seconds, send projectiles that STUN enemies - Scales with ABILITY damage";
            icon.sprite = icons[14];
        }
        else if(abilityCode == "n8d")
        {
            abilityTitle.text = "Four of a kind - "+abilityName;
            abilityDesc.text = "hold attack down for 3 seconds to deal AOE damage around you - Scales with ABILITY damage";
            icon.sprite = icons[15];
        }
        else if(abilityCode == "n9d")
        {
            abilityTitle.text = "Five of a kind - "+abilityName;
            abilityDesc.text = "hold attack down for 3 seconds to deal AOE damage around you and gives enemies FROZEN effect - Scales with ABILITY damage";
            icon.sprite = icons[16];
        }
        else if(abilityCode == "b10d")
        {
            abilityTitle.text = "Royal Flush - "+abilityName;
            abilityDesc.text = "Send 5 bomb projectiles in front of you with small spread that explode on contact with enemies and walls, or after several seconds - Scales with ABILITY damage";
            icon.sprite = icons[17];
        }
        // heart
        else if(abilityCode == "n2h")
        {
            abilityTitle.text = "Pair - "+abilityName;
            abilityDesc.text = "Lowers dash cooldown modifier by 25%";
            icon.sprite = icons[18];
        }
        else if(abilityCode == "b3h")
        {
            abilityTitle.text = "Two pair - "+abilityName;
            abilityDesc.text = "Dash cooldown lowered drastically for several seconds";
            icon.sprite = icons[19];
        }
        else if(abilityCode == "b4h")
        {
            abilityTitle.text = "Three of a kind - "+abilityName;
            abilityDesc.text = "Heal yourself by 30HP";
            icon.sprite = icons[20];
        }
        else if(abilityCode == "n5h")
        {
            abilityTitle.text = "Flush - "+abilityName;
            abilityDesc.text = "Attacking enemies heals you by a small amount";
            icon.sprite = icons[21];
        }
        else if(abilityCode == "b6h")
        {
            abilityTitle.text = "Straight - "+abilityName;
            abilityDesc.text = "Send 2 projectiles in front of you that lowers enemy MAX HP - Scales with ABILITY damage";
            icon.sprite = icons[22];
        }
        else if(abilityCode == "b7h")
        {
            abilityTitle.text = "Full House - "+abilityName;
            abilityDesc.text = "In a time frame, kill as many enemies as you can, at the end of the timer, you heal 5HP per enemy killed";
            icon.sprite = icons[23];
        }
        else if(abilityCode == "n8h")
        {
            abilityTitle.text = "Four of a kind - "+abilityName;
            abilityDesc.text = "Hold attack down for 3 seconds to, for several seconds, raise your MAX HP. After timer, your MAX HP returns to normal";
            icon.sprite = icons[24];
        }
        else if(abilityCode == "n9h")
        {
            abilityTitle.text = "Five of a kind - "+abilityName;
            abilityDesc.text = "Hold attack down for 3 seconds to, for several seconds, raise your MAX HP. After timer, your MAX HP returns to normal. During the time frame, your dash cooldown is lowered drastically";
            icon.sprite = icons[25];
        }
        else if(abilityCode == "b10h")
        {
            abilityTitle.text = "Royal Flush - "+abilityName;
            abilityDesc.text = "Send 2 bomb projectiles in front of you that explode on contact with enemies and walls, or after several seconds and lowers enemy MAX HP - Scales with ABILITY damage";
            icon.sprite = icons[26];
        }
        // club
        else if(abilityCode == "n2c")
        {
            abilityTitle.text = "Pair - "+abilityName;
            abilityDesc.text = "Increase Dash Damage Modifier by 20%";
            icon.sprite = icons[27];
        }
        else if(abilityCode == "b3c")
        {
            abilityTitle.text = "Two pair - "+abilityName;
            abilityDesc.text = "Charge forward, dealing contact damage to enemies. Harder to turn - Scales with ABILITY damage";
            icon.sprite = icons[28];
        }
        else if(abilityCode == "b4c")
        {
            abilityTitle.text = "Three of a kind - "+abilityName;
            abilityDesc.text = "Send 3 ruptures out in a cone in front of you that damage enemies - Scales with ABILITY damage";
            icon.sprite = icons[29];
        }
        else if(abilityCode == "n5c")
        {
            abilityTitle.text = "Flush - "+abilityName;
            abilityDesc.text = "Attacking enemies gives enemies SLOWED effect";
            icon.sprite = icons[30];
        }
        else if(abilityCode == "b6c")
        {
            abilityTitle.text = "Straight - "+abilityName;
            abilityDesc.text = "Send 5 projectiles in front of you in a cone that damage enemies - Scales with ABILITY damage";
            icon.sprite = icons[31];
        }
        else if(abilityCode == "b7c")
        {
            abilityTitle.text = "Full House - "+abilityName;
            abilityDesc.text = "Enemies around you will become ENRAGED and increase run speed, but you deal 100% more damage";
            icon.sprite = icons[32];
        }
        else if(abilityCode == "n8c")
        {
            abilityTitle.text = "Four of a kind - "+abilityName;
            abilityDesc.text = "Hold attack down for 3 seconds to send 4 ruptures out in a cone in front of you that damage enemies - Scales with ABILITY damage";
            icon.sprite = icons[33];
        }
        else if(abilityCode == "n9c")
        {
            abilityTitle.text = "Five of a kind - "+abilityName;
            abilityDesc.text = "Hold attack down for 3 seconds to send 5 ruptures out in a cone in front of you that damage enemies and charge forward, dealing contact damage to enemies - Scales with ABILITY damage";
            icon.sprite = icons[34];
        }
        else if(abilityCode == "b10c")
        {
            abilityTitle.text = "Royal Flush - "+abilityName;
            abilityDesc.text = "Send 5 bomb projectiles in front of you in a cone that explode on contact with enemies and walls, or after several seconds - Scales with ABILITY damage";
            icon.sprite = icons[35];
        }
        // spade
        else if(abilityCode == "n2s")
        {
            abilityTitle.text = "Pair - "+abilityName;
            abilityDesc.text = "Increase Dash Distance Modifier by 20%";
            icon.sprite = icons[36];
        }
        else if(abilityCode == "b3s")
        {
            abilityTitle.text = "Two pair - "+abilityName;
            abilityDesc.text = "Dashing instead teleports you, dealing small AOE damage when disapearing and reapearing - Scales with ABILITY damage";
            icon.sprite = icons[37];
        }
        else if(abilityCode == "b4s")
        {
            abilityTitle.text = "Three of a kind - "+abilityName;
            abilityDesc.text = "Send projectile that deals damage and STUNs enemies - Scales with ABILITY damage";
            icon.sprite = icons[38];
        }
        else if(abilityCode == "n5s")
        {
            abilityTitle.text = "Flush - "+abilityName;
            abilityDesc.text = "After attacking an enemy, they take equal damage a few seconds afterward";
            icon.sprite = icons[39];
        }
        else if(abilityCode == "b6s")
        {
            abilityTitle.text = "Straight - "+abilityName;
            abilityDesc.text = "Send a high-pierce projectile in front of you that damages enemies - Scales with ABILITY damage";
            icon.sprite = icons[40];
        }
        else if(abilityCode == "b7s")
        {
            abilityTitle.text = "Full House - "+abilityName;
            abilityDesc.text = "For several seconds, summon 3 beams of light in front of you that deal damage over time to enemies in the beam - Scales with ABILITY damage";
            icon.sprite = icons[41];
        }
        else if(abilityCode == "n8s")
        {
            abilityTitle.text = "Four of a kind - "+abilityName;
            abilityDesc.text = "Hold attack down for 3 seconds to dash forard and make a powerful attack - Scales with ABILITY damage";
            icon.sprite = icons[42];
        }
        else if(abilityCode == "n9s")
        {
            abilityTitle.text = "Five of a kind - "+abilityName;
            abilityDesc.text = "Hold attack down for 3 seconds to dash forard and make a powerful attack. Ability dash teleports, dealing small AOE damage when disapearing and reapearing - Scales with ABILITY damage";
            icon.sprite = icons[43];
        }
        else if(abilityCode == "b10s")
        {
            abilityTitle.text = "Royal Flush - "+abilityName;
            abilityDesc.text = "Send 1 high pierce projectile in front of you that summons a small bomb on contacted enemies. Bombs explode after several seconds - Scales with ABILITY damage";
            icon.sprite = icons[44];
        }

    }
}
