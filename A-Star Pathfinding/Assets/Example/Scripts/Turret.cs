using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    public enum Type { Boopler, Bapper}
    public Type type;
    public AudioClip sndAttack;
    public GameObject bulletPrefab;
    public Enemy currentTarget;
    public List<Enemy> enemiesInTrigger;
    public float damage = 2f;
    public float shootTimer = 1f;
    public int upgradeCost = 10;
    public int currentLevel = 1;

    private void Start() {
        StartCoroutine(Attack());
    }

    private void OnMouseOver() {
        if (currentLevel < 5) {
            MainManager.inst.hoveringTurret = this;
            MainManager.inst.hudText.text = "RMB: Upgrade Turret, Costs: " + upgradeCost;
            if (Input.GetMouseButtonDown(1)) {
                if (MainManager.inst.playerMoney >= upgradeCost) {
                    MainManager.inst.UpdateMoney(-upgradeCost);
                    Upgrade();
                }
            }
        } else {
            MainManager.inst.hudText.text = "";
        }
    }

    private void OnMouseExit() {
        if (MainManager.inst.hoveringTurret == this) {
            MainManager.inst.hoveringTurret = null;
            MainManager.inst.hudText.text = "";
        }
    }

    IEnumerator Attack() {
        while (true) {
            if (currentTarget == null) {
                if (enemiesInTrigger.Count > 0) currentTarget = enemiesInTrigger[0];
            }
            if (currentTarget) {
                AudioManager.am.PlayClip(sndAttack, true, .4f);
                switch (type) {
                    case Type.Bapper:
                        Vector2 dir = currentTarget.transform.position - transform.position;
                        float dist = dir.magnitude;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        var inst = Instantiate(bulletPrefab, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
                        inst.transform.localScale = new Vector3(dist, 1, 1);
                        Destroy(inst, .1f);
                        if (currentTarget.TakeDamage(damage)) {
                            currentTarget = null;
                            if (enemiesInTrigger.Count > 0) currentTarget = enemiesInTrigger[0];
                        }
                        break;

                    case Type.Boopler:
                        var newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Bullet>();
                        newBullet.target = currentTarget;
                        break;
                }
            }
            yield return new WaitForSeconds(shootTimer);
        }
    }

    public void Upgrade() {
        currentLevel++;
        AudioManager.am.PlaySound(0);
        GetComponent<SpriteRenderer>().color *= 1.2f;
        switch (type) {
            case Type.Boopler:
                damage += .5f;
                shootTimer *= .9f;
                upgradeCost += 5;
                transform.GetChild(0).localScale = Vector3.one * currentLevel * .15f;
                break;

            case Type.Bapper:
                damage += .5f;
                shootTimer *= .9f;
                upgradeCost += 5;
                transform.GetChild(0).localScale = Vector3.one * currentLevel * .15f;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Enemy")) {
            enemiesInTrigger.Add(collision.GetComponent<Enemy>());
            if (currentTarget == null) currentTarget = collision.GetComponent<Enemy>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Enemy")) {
            enemiesInTrigger.Remove(collision.GetComponent<Enemy>());
            if (currentTarget == collision.GetComponent<Enemy>()) currentTarget = null;
        }
    }

}
