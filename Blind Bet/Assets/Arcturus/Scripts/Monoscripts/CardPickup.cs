using UnityEngine;

public class CardPickup : MonoBehaviour
{
    private bool growing = true;
    private bool shrinking;
    public GameObject uiCard;

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
                pm.playerStats.AddCard(uiCard.GetComponent<DraggableItem>().card,4);
                Instantiate(uiCard,pm.playerUI.bench);
                // Debug.Log(pm.playerStats.bench[0].rank);
                Destroy(gameObject);
            }
            
        }
    }
}
