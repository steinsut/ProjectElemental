using UnityEngine;

public class Leaf : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // TODO: Add Score
            this.gameObject.SetActive(false);
        }
    }
}
