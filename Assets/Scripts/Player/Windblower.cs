using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class Windblower : MonoBehaviour
{
    private bool _blowing = false;

    [SerializeField]
    private PlayerController _player;
 
    private Dictionary<int, IEnemy> _enemies = new Dictionary<int, IEnemy>();

    public bool Blowing
    {
        get { return _blowing; }
        set
        {
            _blowing = value;
            if (!value)
            {
                foreach(IEnemy enemy in _enemies.Values)
                {
                    enemy.StopPush();
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!_enemies.ContainsKey(collision.GetHashCode()))
        {
            if(collision.CompareTag("Enemy"))
            {
                _enemies.Add(collision.GetHashCode(), collision.GetComponent<IEnemy>());
                if(_blowing)
                {
                    _player.PushEnemy(_enemies[collision.GetHashCode()]);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(_blowing)
        {
            if(_enemies.ContainsKey(collision.GetHashCode()))
            {
                _player.PushEnemy(_enemies[collision.GetHashCode()]);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(_enemies.ContainsKey(collision.GetHashCode()))
        {
            IEnemy enemy = _enemies[collision.GetHashCode()];
            if(enemy != null)
            {
                _enemies[collision.GetHashCode()].StopPush();
            }
            _enemies.Remove(collision.GetHashCode());
        }
    }
}
