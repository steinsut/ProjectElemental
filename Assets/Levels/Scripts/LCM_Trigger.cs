using System;
using UnityEngine;

public class LCC_Trigger : MonoBehaviour
{
    [SerializeField]
    private LevelCameraManager CameraManager;

    [SerializeField]
    private int levelIndex = 0;

    private bool alreadyInitialized = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!alreadyInitialized && collision.CompareTag("Player"))
        {
            alreadyInitialized = true;
            CameraManager.moveTo(levelIndex);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (CameraManager == null)
        {
            Debug.LogError("CameraManager not initialized!");
        }
    }
}
