using UnityEngine;
using Random = System.Random;
public class CardPickup : MonoBehaviour
{
    private bool growing = true;
    private bool shrinking;
    public GameStats gameStats;
    public CardDeck cardDeck;
    Random rand = new Random();
    void Awake()
    {
        if(rand.Next(1,101) < 20)
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if(growing)
        {
            transform.localScale += new Vector3(0.25f,0.25f,0.25f) *2* Time.deltaTime;
            if(transform.localScale.x > 1.5f)
            {
                growing = false;
                shrinking = true;
            }
        }
        if(shrinking)
        {
            transform.localScale -= new Vector3(0.25f,0.25f,0.25f) *2* Time.deltaTime;
            if(transform.localScale.x < 1f)
            {
                growing = true;
                shrinking = false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMovement pm))
        {
            if(pm.playerStats.bench.Count < 7)
            {
                // finding rank
                int rank = 0;
                if (gameStats.level >= 1 && gameStats.level < 9)
                    rank = rand.Next(1+gameStats.level,6+gameStats.level);
                else if(gameStats.level == 9 || gameStats.level == 10)
                    rank = rand.Next(10,15);
                // finding suit
                Card.Suit suit = Card.Suit.blank;
                int suitNum = rand.Next(1,5);
                if(suitNum == 1)
                    suit = Card.Suit.diamond;
                else if(suitNum == 2)
                    suit = Card.Suit.heart;
                else if(suitNum == 3)
                    suit = Card.Suit.club;
                else if(suitNum == 4)
                    suit = Card.Suit.spade;
                // adding to player
                Debug.Log(suit);
                Debug.Log(rank);
                pm.playerStats.bench.Add(cardDeck.GetCardFromComponents(rank,suit));
                Instantiate(cardDeck.GetUIObjectFromCard(cardDeck.GetCardFromComponents(rank,suit)),pm.playerUI.bench);
                Destroy(gameObject);
            }
            
        }
    }
}
