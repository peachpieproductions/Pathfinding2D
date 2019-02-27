using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public Transform target;
    public float maxHp;
    public Transform hpBarBg;
    public Transform hpBarFill;

    float hp;
    Pathfinder pathfinder;

    private void Start() {
        target = MainManager.inst.enemyTarget;
        pathfinder = GetComponent<Pathfinder>();
        if (pathfinder == null) gameObject.AddComponent<Pathfinder>();
        pathfinder.FindPath(transform.position, target.position);
        pathfinder.NavigatePath();

        if (MainManager.inst.currentLevel < 10) {
            pathfinder.navSpeed += Mathf.Clamp01(MainManager.inst.currentLevel * .05f);
            maxHp += MainManager.inst.currentLevel * 2;
        } else if (MainManager.inst.currentLevel < 20) {
            pathfinder.navSpeed = 1 + (MainManager.inst.currentLevel - 10) * .05f;
            maxHp += MainManager.inst.currentLevel * 3f;
        } else {
            pathfinder.navSpeed = 1 + (MainManager.inst.currentLevel - 5) * .05f;
            maxHp += MainManager.inst.currentLevel * 4.5f;
        }
        hp = maxHp;
    }

    void Update() {
        
        if (pathfinder.pathComplete) {
            MainManager.inst.PlayerTakeDamage();
            Destroy(gameObject);
        }
    }

    private void LateUpdate() {
        hpBarBg.SetPositionAndRotation(transform.position + Vector3.up * .4f, Quaternion.Euler(0, 0, 0));
    }

    public bool TakeDamage(float damage) {
        if (hp == maxHp) hpBarBg.gameObject.SetActive(true);
        hp -= damage;
        if (hp > 0) hpBarFill.localScale = new Vector3(hp / maxHp, 1, 1);
        if (hp <= 0) {
            AudioManager.am.PlaySound(1);
            Instantiate(MainManager.inst.coinPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    private void OnDestroy() {
        MainManager.inst.enemies.Remove(this);
    }



}
