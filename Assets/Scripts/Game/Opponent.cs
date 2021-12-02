using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    [SerializeField] float pushTime = 1f;
    [SerializeField] float pushSpeed = 10f;

    bool left;








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
        timer = 0f; // tester ça là !
        StopAllCoroutines();
    }
}
