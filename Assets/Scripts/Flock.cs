using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    float speed;
    bool turning = false;
    void Start()
    {
        speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
    }

    void Update()
    {
        Bounds b = new Bounds(FlockManager.FM.transform.position, FlockManager.FM.swimLimits * 1.5f); // Keep fishin in radius from flockmanger centre

        if (!b.Contains(transform.position))
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction = FlockManager.FM.transform.position - transform.position; // Turn fish back inside the bounding box
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager.FM.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
            }

            if (Random.Range(0, 100) < 10)
            {
                ApplyRules();
            }
        }
        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    private void ApplyRules()
    {
        GameObject[] gos;
        gos = FlockManager.FM.allfish;
        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float groupSpeed = 0.01f;
        float neighbourDistance;
        int groupSize = 0;

        //for each fish in the fish array in the school of fish
        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                neighbourDistance = Vector3.Distance(go.transform.position, this.transform.position); //check fish distance compared to neighbour
                if (neighbourDistance <= FlockManager.FM.neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if (neighbourDistance < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position); // Avoid neighbour fish
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    groupSpeed = groupSpeed + anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            //average group center
            vcentre = vcentre / groupSize + (FlockManager.FM.goalPos - this.transform.position);
            speed = groupSpeed / groupSize;
            if (speed > FlockManager.FM.maxSpeed)
            {
                speed = FlockManager.FM.maxSpeed;
            }
            //new direction fish
            Vector3 direction = (vcentre + vavoid) - transform.position;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
