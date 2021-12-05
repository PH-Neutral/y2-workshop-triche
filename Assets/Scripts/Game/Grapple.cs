using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour {
    public float Speed { get => speed + baseSpeed; }
    public float speed, maxRange;
    public float malusSpeed, duration;

    float baseSpeed = 0;
    bool launched = false, stuck = false, unstuck = false;
    LineRenderer lr;
    Transform launcher;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    private void Update() {
        if(unstuck) {
            PullBack(Time.deltaTime);
        } else if(launched && !stuck) { 
            Move(Time.deltaTime);
        }
        lr.SetPosition(1, transform.InverseTransformPoint(launcher.position));
    }

    public void Launch(Transform launcher, float baseSpeed) {
        this.launcher = launcher;
        this.baseSpeed = baseSpeed;
        launched = true;
        stuck = false;
    }

    RaycastHit hit;
    void Move(float dt) {
        if(Vector3.Distance(launcher.position, transform.position) > maxRange) {
            unstuck = true;
        }
        if(stuck) return;

        Vector3 motion = transform.forward * Speed * dt;
        transform.position += motion;
        if(Physics.Raycast(transform.position, motion.normalized, out hit, motion.magnitude)) {
            Opponent op;
            if((op = hit.collider.GetComponentInParent<Opponent>()) != null) {
                stuck = true;
                transform.SetParent(op.transform);
                StartCoroutine(SlowDownCar(op, malusSpeed, duration));
            }
        }
    }

    IEnumerator SlowDownCar(Opponent car, float minusSpeed, float duration) {
        car.speed -= minusSpeed;
        yield return new WaitForSeconds(duration);
        car.speed += minusSpeed;
    }

    void PullBack(float dt) {
        Destroy(gameObject);
    }
}