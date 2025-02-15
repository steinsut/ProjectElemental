using System.Collections.Generic;
using UnityEngine;

public class LevelCameraManager : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> cameraPositions;

    [SerializeField]
    private GameObject MainCamera;

    private GameObject targetPosition;

    [SerializeField]
    private float speed;

    public void moveTo(int index)
    {
        targetPosition = cameraPositions[index];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPosition = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(MainCamera.transform.position, targetPosition.transform.position) > 1)
        {
            MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, targetPosition.transform.position, speed * Time.deltaTime);
        }
    }
}
