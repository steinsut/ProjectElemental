using UnityEngine;

public class PlayerFireball : MonoBehaviour
{
    private Animator _animator;

    public float velocity = 0;
    public int damage = 0;
    public float velocityDeviation = 0;
    private bool blowing = false;
    private float lifeTime = 0;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            StartCoroutine(other.GetComponent<IEnemy>().TakeDamage(damage));
            velocity = 0;
            _animator.SetTrigger("Blow");
            _animator.speed = 1;
            blowing = true;
        }
        else if(other.CompareTag("Wall")) {
            velocity = 0;
            _animator.SetTrigger("Blow");
            _animator.speed = 1;
            blowing = true;
        }
    }

    private void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime > 30)
        {
            Destroy(gameObject);
        }
        else
        {
            _animator.speed = 1 + velocityDeviation / 2;
            transform.Translate(Vector2.right * velocity * Time.deltaTime);
            if(blowing && _animator.playbackTime >= _animator.recorderStopTime) {
                Destroy(gameObject, _animator.GetCurrentAnimatorClipInfo(0).Length - 0.01f);
                blowing = false;
            }
        }
    }
}
