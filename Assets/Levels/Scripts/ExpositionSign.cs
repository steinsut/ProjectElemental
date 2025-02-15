using Unity.VisualScripting;
using UnityEngine;

public class ExpositionSign : MonoBehaviour
{

    [SerializeField]
    private GameObject Exposition;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            showExposition();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hideExposition();
        }
    }

    void showExposition()
    {
        Exposition.SetActive(true);
    }

    void hideExposition()
    {
        Exposition.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
