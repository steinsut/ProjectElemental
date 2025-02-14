using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectilePooling : MonoBehaviour
{
    public static ProjectilePooling SingletonInstance;
    public GameObject projectilePrefab;
    private List<GameObject> projectilePool;
    public int poolSize = 100;
    void Awake(){
        SingletonInstance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        projectilePool = new List<GameObject>();
        GameObject tmp;
        for(int i = 0; i < poolSize; i++)
        {
            tmp = Instantiate(projectilePrefab);
            tmp.SetActive(false);
            projectilePool.Add(tmp);
        }
    }

    public GameObject GetProjectile(){
        for(int i = 0; i < poolSize; i++)
        {
            if(!projectilePool[i].activeInHierarchy)
            {
                return projectilePool[i];
            }
        }
        return null;
    }

}
