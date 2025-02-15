using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public Vector2 direction;
    public float projectTileSpeed;

    private float lifetime = 10f;

    void Update()
    {
        transform.position += (Vector3)direction * projectTileSpeed * Time.deltaTime;
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        lifetime = 10f;
    }

    public void SetDirection(Vector2 direction){
        this.direction = direction;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy") return;
        if(other.gameObject.tag == "Player"){
            //other.gameObject.GetComponent<IEnemy>().TakeDamage(1);
        }
        gameObject.SetActive(false);

    }
}
