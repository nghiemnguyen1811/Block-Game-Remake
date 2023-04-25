using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Vector2 mousePosition;
    [SerializeField] private LineRenderer lineRenderer;

    private void Update()
    {
        float laserLength = 2f;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)this.transform.position;
        this.transform.up = direction;
        lineRenderer.SetPosition(0, this.transform.position);
        lineRenderer.SetPosition(1, this.transform.position + this.transform.up * laserLength);
        RaycastHit2D ray = Physics2D.Raycast(this.transform.position, this.transform.up, Mathf.Infinity);
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Shoot");

            if (ray)
            {

            }
        }
        Debug.DrawRay(this.transform.position, this.transform.up, Color.green);
    }
}
