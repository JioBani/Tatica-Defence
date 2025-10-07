using System;
using System.Linq;
using UnityEngine;

public class TestClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        var hits = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        Debug.Log(hits.First().gameObject.name);
    }
}
