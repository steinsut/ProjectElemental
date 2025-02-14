using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public Vector2 direction;
    public float projectTileSpeed;

    void Update()
    {
        transform.position += (Vector3)direction * projectTileSpeed * Time.deltaTime;
        if (Mathf.Abs(transform.position.x) > 20f  || Mathf.Abs(transform.position.y) > 20f)
        {
            gameObject.SetActive(false);
        }
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
