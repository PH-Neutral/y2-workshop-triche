using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goforward : MonoBehaviour
{
    [SerializeField] float lifeTime = 0.8f;
    private void Awake()
    {
        StartCoroutine(nameof(Destroy));
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    /*
    public float speed;
    public bool isCollide;

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Destroy());
        if (!isCollide)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else 
        {
            transform.Translate(-Vector3.forward * speed * Time.deltaTime);
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }
    IEnumerator Destroy2()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Opponent")
        {
            speed = 1;
            StartCoroutine(Destroy2());
        }
        if (other.gameObject.tag == "Point2")
        {
            speed = 3;
            StartCoroutine(Destroy2());
        }
    }
    */
}
