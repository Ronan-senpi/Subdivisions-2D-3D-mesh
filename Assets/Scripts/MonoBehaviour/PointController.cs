using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointController : MonoBehaviour
{
    private void Start()
    {
        if (transform.childCount > 0)
            transform.GetChild(0).GetChild(0).GetComponent<Text>().text =
                Math.Round(transform.position.x, 2) + " : " + Math.Round(transform.position.y, 2);
    }

    public void DestroyGO()
    {
        Destroy(gameObject);
    }
}