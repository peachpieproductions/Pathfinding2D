using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    public Rigidbody2D rb;
    public float speed;

    private void Start() {
        rb.velocity = new Vector2(Random.Range(-speed, speed), Random.Range(-speed, speed));
        
    }

    private void FixedUpdate() {
        if (rb.velocity.magnitude == 0) {
            Destroy(rb);
            Destroy(this);
        }
    }


}
