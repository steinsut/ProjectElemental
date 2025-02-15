using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent (typeof(LineRenderer))]
public class CursorController : MonoBehaviour
{
    private Camera _mainCamera;
    private LineRenderer _lineRenderer;

    [SerializeField]
    private Transform _followTransform;

    int materialDirectionId;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _lineRenderer = GetComponent<LineRenderer>();
        materialDirectionId = Shader.PropertyToID("_Direction");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos0 = _followTransform.position;
        pos0.z = 1;
        Vector3 pos1 = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        pos1.z = 1;
        _lineRenderer.SetPosition(0, pos0);
        _lineRenderer.SetPosition(1, pos1);
        pos1.z = -1;
        transform.position = pos1;
        if(_lineRenderer.bounds.size.x > _lineRenderer.bounds.size.y)
        {
            _lineRenderer.material.SetFloat(materialDirectionId, pos1.x - pos0.x > 0 ? 1 : -1);
        }
        else
        {
            _lineRenderer.material.SetFloat(materialDirectionId, pos1.y - pos0.y > 0 ? 1 : -1);
        }
        Debug.Log(_lineRenderer.bounds);
    }
}
