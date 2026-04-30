using UnityEngine;
using Random = System.Random;
public class KharonCardField : MonoBehaviour
{
    public KharonCardHands kharonHands;
    public CardDeck cardDeck;
    Random rand = new Random();
    private void Awake()
    {
        Card[] hand = kharonHands.cardHands[rand.Next(0,kharonHands.cardHands.Count)].hand;
        for(int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(cardDeck.GetUIObjectFromCard(hand[i]),transform);
            if(card.TryGetComponent(out DraggableItem di))
                di.enabled = false;
        }
    }
}
