using UnityEngine;

public class PrefabLibrary : MonoBehaviour
{
    public static PrefabLibrary instance;
    private void Awake()
    {
        instance = this;
    }
    [Header("Basic Movement")]
    public GameObject attackVisual;
    public GameObject parryObject;

    [Header("Active Ability Visuals")]
    public GameObject whirlWindsVisual;

    [Header("Diamond Ability Visuals")]
    public GameObject chillingBurstVisual;
    public GameObject flashbangvisual;
    public GameObject shockingWheelVisual;
    public GameObject freezingWheelVisual;

    [Header("Heart Ability Visuals")]
    [Header("Club Ability Visuals")]
    [Header("Spade Ability Visuals")]

    [Header("Bullets")]
    public GameObject continuousBlade;
    public GameObject royalBomb;
    public GameObject spectralBullet;
    public GameObject soundWave;
    public GameObject royalBombAbility;
    public GameObject witheringBullet;
    public GameObject witheringBomb;

    [Header("Indicators")]
    public GameObject diamondIndicator;
}
