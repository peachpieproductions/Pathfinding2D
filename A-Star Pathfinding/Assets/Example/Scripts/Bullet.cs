using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Enemy target;
    public bool homing;
    public float speed;

    float distToEnemy;

    private void Start() {
        if (target) distToEnemy = (target.transform.position - transform.position).magnitude;
    }

    private void Update() {
        if (target) {
            if (homing) {
                distToEnemy -= Time.deltaTime * 27f;
                transform.position += (target.transform.position - transform.position).normalized * Time.deltaTime * speed;
                if (distToEnemy <= 0) {
                    target.TakeDamage(1f);
                    Destroy(gameObject);
                }
            }
        } else {
            Destroy(gameObject);
        }
    }


}
