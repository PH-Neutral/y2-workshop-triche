using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    [SerializeField] FollowPath path;
    [SerializeField] bool rollBaby = false;
    public float speed;
    [SerializeField] float pushTime = 1f;
    [SerializeField] float pushSpeed = 10f;
    int index = 0;

    bool left;

    private void Update() {
        if(rollBaby) {
            float deltaDistance = speed * Time.deltaTime;
            Vector3 targetPos = path.GetWaypoint(index).position;
            Vector3 targetDir = (targetPos - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, targetPos);
            if(distanceToTarget != 0) {
                transform.LookAt(targetPos, Vector3.up);
            }
            if(deltaDistance >= distanceToTarget) {
                Move(targetDir * distanceToTarget);
                index = path.GetNextIndex(index);
            } else {
                Move(targetDir * deltaDistance);
            }
        }
    }

    void Move(Vector3 motion) {
        transform.position += motion;
    }

    public void CallPush(bool left)
    {
        this.left = left;
        StartCoroutine(nameof(Push));
    }
    float timer = 0f;
    public IEnumerator Push()
    {
        Debug.Log("push");
        while (timer < pushTime)
        {
            transform.position += Vector3.left * pushSpeed * Time.deltaTime * (left ? 1 : -1);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0f; // tester �a l� !
        StopAllCoroutines();
    }
    IEnumerator Move()
    {
        yield return new WaitForSeconds(5);
        if (speed < 10)
        {
            speed += 5;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Grapple")
        {
            speed = 0.5f;
            StartCoroutine(Move());
        }

        if (other.gameObject.tag == "GoGoGadget")
        {
            speed = 0;
            StartCoroutine(Move());
        }
    }
}
