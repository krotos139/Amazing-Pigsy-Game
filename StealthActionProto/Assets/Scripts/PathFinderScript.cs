using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderScript : MonoBehaviour
{

    //public AINavMeshGenerator ai;
    public GameObject target;
    public Vector2[] points;

    // Start is called before the first frame update
    void Start()
    {
        points = AINavMeshGenerator.pathfinder.FindPath(new Vector2(transform.position.x, transform.position.y), new Vector2(target.transform.position.x, target.transform.position.y), null);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            Debug.DrawLine(points[i], points[i + 1], Color.red);
        }
    }
}
