using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float speed = 1;

    Pathfinder pathfinder;
    Rigidbody2D rb;

    private void Awake() {
        pathfinder = GetComponent<Pathfinder>();
        if (pathfinder == null) gameObject.AddComponent<Pathfinder>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {

        rb.velocity += new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Coin")) {
            MainManager.inst.UpdateMoney(1);
            AudioManager.am.PlaySound(Random.Range(5, 7));
            Destroy(collision.gameObject);
        }
    }





}
