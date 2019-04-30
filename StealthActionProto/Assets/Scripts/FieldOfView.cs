using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//https://www.youtube.com/watch?v=rQG9aUWarwE
//https://github.com/SebLague/Field-of-View
public class FieldOfView : MonoBehaviour
{
    public Animator animator;

    public float viewRadius = 10.0f;
    [Range(0, 360)]
    public float viewAngle = 109.0f;
    [Range(0, 20)]
    public float waitViewRotation = 4.0f;

    public float coinSpotRadius;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    [HideInInspector]
    public List<Transform> visibleCoins = new List<Transform>();

    public float speed = 3.0f;
    public float runSpeed = 5.0f;
    private float waitTime;
    public float startWaitTime = 2.0f;

    public Transform[] moveSpots;
    private int currentSpot;
    public int state = 0;
    // 0 - patrol
    // 1 - run to pig
    // 2 - go to last position
    // 3 - wait
    // 4 - attack
    // 5 - go to coin
    // 6 - go on path to waypoint
    // 7 - wait near coin
    // 8 - wait after attack
    private Vector3 lastPosition;
    public Vector2[] pathPoints;
    private int currentPathIndex;
    private GameObject coinObj;
    private Transform coinPos;
    public Vector2 viewVector;

    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);
        currentSpot = 0;
        waitTime = startWaitTime;
        viewVector = new Vector2(0.0f, 1.0f);
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(viewVector, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                //float dstToTarget = Vector3.Distance(transform.position, moveSpots[currentSpot].position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask.value);
                //Debug.Log("Hit2 d:" + hit.distance + " c:" + hit.collider + " p:" + hit.point+ " obstacleMask:"+ obstacleMask.value);
                if (hit.distance < 0.1f)
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees -= transform.eulerAngles.z;
        }
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void Update()
    {
        if (state == 0) // patrol
        {
            
            if (visibleTargets.Count == 0)
            {
                animator.SetBool("isMoving", false);
                animator.SetBool("isAttacking", false);
                Debug.Log("patrul!");
                pathPoints = AINavMeshGenerator.pathfinder.FindPath(new Vector2(transform.position.x, transform.position.y), new Vector2(moveSpots[currentSpot].position.x, moveSpots[currentSpot].position.y), null);
                currentPathIndex = 0;
                state = 6;
                currentSpot += 1;
                Debug.Log("currentSpot:"+ currentSpot);
                if (currentSpot >= moveSpots.Length)
                {
                    currentSpot = 0;
                }
                /*transform.position = Vector2.MoveTowards(transform.position, moveSpots[currentSpot].position, speed * Time.deltaTime);

                if (waitTime == startWaitTime)
                {
                    Vector3 dirToTarget = (moveSpots[currentSpot].position - transform.position).normalized;
                    float angle = Vector3.SignedAngle(transform.up, dirToTarget, transform.forward) * 0.2f;
                    transform.Rotate(transform.forward, angle);
                }
                else
                {
                    transform.Rotate(transform.forward, waitViewRotation * Mathf.Sin((waitTime / startWaitTime) * 2.0f * Mathf.PI));
                }

                if (Vector2.Distance(transform.position, moveSpots[currentSpot].position) < 0.5f)
                {
                    if (waitTime <= 0)
                    {
                        currentSpot += 1;
                        if (currentSpot >= moveSpots.Length)
                        {
                            currentSpot = 0;
                        }
                        waitTime = startWaitTime;
                        Debug.Log("Goto " + currentSpot + " spot");
                    }
                    else
                    {
                        Debug.Log("Wait...");
                        goToSpot = false;
                        waitTime -= Time.deltaTime;
                    }
                }*/
            }
            else
            {
                lastPosition = visibleTargets[0].position;
                state = 1; // run to pig
            }
        }

        if (state == 1) // run to pig
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", false);
            GameObject[] list = GameObject.FindGameObjectsWithTag("Coin");
            if (list.Length > 0)
            {
                coinObj = list[0];
                coinPos = coinObj.GetComponent<Transform>();
                pathPoints = AINavMeshGenerator.pathfinder.FindPath(new Vector2(transform.position.x, transform.position.y), new Vector2(coinPos.position.x, coinPos.position.y), null);
                currentPathIndex = 0;
                state = 5;
            }
            if (visibleTargets.Count == 0)
            {
                state = 2; // goto last pig position
                Debug.Log("Goto last position");
            }
            else {
                if (Vector2.Distance(transform.position, visibleTargets[0].position) < 1.6f)
                {
                    Debug.Log("Hit pig");
                    state = 4; // attack pig
                }
                else
                {
                    //Debug.Log("Goto pig!");
                    Vector3 dirToTarget = (visibleTargets[0].position - transform.position).normalized;
                    float angle = Vector3.SignedAngle(transform.up, dirToTarget, transform.forward);
                    //
                    viewVector.Set(-Mathf.Sin(angle * Mathf.PI / 180.0f), Mathf.Cos(angle * Mathf.PI / 180.0f));
                    //transform.Rotate(transform.forward, angle);
                    transform.position = Vector2.MoveTowards(transform.position, visibleTargets[0].position, runSpeed * Time.deltaTime);

                    lastPosition = visibleTargets[0].position;
                }
            }
        }

        if (state == 2) // go to last position
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", false);
            if (visibleTargets.Count == 0)
            {
                //Debug.Log("Goto last pig position!");
                Vector3 dirToTarget = (lastPosition - transform.position).normalized;
                float angle = Vector3.SignedAngle(transform.up, dirToTarget, transform.forward);
                viewVector.Set(Mathf.Sin(angle * Mathf.PI / 180.0f), Mathf.Cos(angle * Mathf.PI / 180.0f));
                //transform.Rotate(transform.forward, angle);
                transform.position = Vector2.MoveTowards(transform.position, lastPosition, speed * Time.deltaTime);

                float dstToTarget = Vector3.Distance(transform.position, lastPosition);
                
                if (dstToTarget < 0.5f)
                {
                    state = 3; // wait

                    Debug.Log("Wait");
                }
            } else {
                state = 1; // run to pig
            }
        }

        if (state == 3) // wait
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", false);
            if (visibleTargets.Count == 0)
            {


                //transform.Rotate(transform.forward, waitViewRotation * Mathf.Sin((waitTime / startWaitTime) * 2.0f * Mathf.PI));

                    if (waitTime <= 0)
                    {
                        waitTime = startWaitTime;
                        state = 0;
                        Debug.Log("Goto next spot");
                    }
                    else
                    {
                        waitTime -= Time.deltaTime;
                    }
            }
            else
            {
                lastPosition = visibleTargets[0].position;
                state = 1; // run to pig
            }
        }

        if (state == 4) // wait after attack
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", true);
            Debug.Log("attack");
            Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

            if (targetsInViewRadius.Length > 0)
            {
                GameObject pig = targetsInViewRadius[0].gameObject;
                Controller pigController = pig.GetComponent<Controller>();
                pigController.onDamage();
            }
            state = 8;
        }

        if (state == 5) // go to coin
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", false);
            transform.position = Vector2.MoveTowards(transform.position, pathPoints[currentPathIndex], speed * Time.deltaTime);

            Vector3 dirToTarget = (moveSpots[currentSpot].position - transform.position).normalized;
            float angle = Vector3.SignedAngle(transform.up, dirToTarget, transform.forward) * 0.2f;
            //transform.Rotate(transform.forward, angle);

            if (Vector2.Distance(transform.position, pathPoints[currentPathIndex]) < 0.5f)
            {
                currentPathIndex += 1;
                if (currentPathIndex >= pathPoints.Length - 1)
                {
                    state = 7; // wait ...
                    
                }

            }
        }

        if (state == 6) // go on path to waypoint
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", false);
            GameObject[] list = GameObject.FindGameObjectsWithTag("Coin");
            if (list.Length > 0)
            {
                coinObj = list[0];
                coinPos = coinObj.GetComponent<Transform>();
                Debug.Log("Goto coin:" + coinPos.position.x + ", " + coinPos.position.y);
                pathPoints = AINavMeshGenerator.pathfinder.FindPath(new Vector2(transform.position.x, transform.position.y), new Vector2(coinPos.position.x, coinPos.position.y), null);
                currentPathIndex = 0;
                state = 5;
            }
            if (visibleTargets.Count == 0)
            {
                //Debug.Log("currentPathIndex:"+ currentPathIndex+ " pathPoints[currentPathIndex]:"+ pathPoints[currentPathIndex]);
                transform.position = Vector2.MoveTowards(transform.position, pathPoints[currentPathIndex], speed * Time.deltaTime);

                Vector3 dirToTarget = (moveSpots[currentSpot].position - transform.position).normalized;
                float angle = Vector3.SignedAngle(transform.up, dirToTarget, transform.forward) * 0.2f;
               //transform.Rotate(transform.forward, angle);

                if (Vector2.Distance(transform.position, pathPoints[currentPathIndex]) < 0.5f)
                {
                    currentPathIndex += 1;
                    if (currentPathIndex >= pathPoints.Length-1)
                    {
                        state = 3; // wait ...
                    }
                    //Debug.Log("Goto " + currentSpot + " spot");

                }
            }
            else
            {
                lastPosition = visibleTargets[0].position;
                state = 1;
            }
        }

        if (state == 7) // wait near coin
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", false);
            //transform.Rotate(transform.forward, waitViewRotation * Mathf.Sin((waitTime / startWaitTime) * 2.0f * Mathf.PI));

            if (waitTime <= 0)
            {
                Destroy(coinObj);
                coinObj = null;
                coinPos = null;
                waitTime = startWaitTime;
                state = 0;
                Debug.Log("Goto next spot");
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
            
        }

        if (state == 8) // wait after attack
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", true);
            if (waitTime <= 0)
            {
                state = 0;
                Debug.Log("Goto next spot");
            }
            else
            {
                waitTime -= Time.deltaTime*2.0f;
            }

        }


        //Vector3 dirToTarget2 = (moveSpots[currentSpot].position - transform.position).normalized;
        //Debug.DrawRay(transform.position, dirToTarget2, Color.green);
    }
}