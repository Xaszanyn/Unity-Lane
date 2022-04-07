using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] float difficulty;
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 0, -int.MaxValue), difficulty);
    }
}
