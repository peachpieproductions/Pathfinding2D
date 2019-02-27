using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainManager : MonoBehaviour {

    public static MainManager inst;

    public int currentLevel = 1;
    public float playerHp = 10f;
    public int playerMoney = 50;
    public bool gameOver;
    public GameObject[] turrets;
    public Material[] materials;
    public int selectedTurretIndex;
    public List<Enemy> enemies = new List<Enemy>();
    public bool buildMode;
    public GameObject floor;
    public Transform enemySpawnPoint;
    public GameObject bulletPrefab;
    public GameObject turretPrefab;
    public GameObject enemyPrefab;
    public GameObject coinPrefab;
    public Transform turretVisualPlacer;
    public Transform enemyTarget;
    public Vector2 mouseWorldPos;
    public GridNode currentGridHover;
    public Turret hoveringTurret;
    public TextMeshProUGUI hudText;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI endGameText;
    public LineRenderer line;

    float waitTimer;
    Pathfinder pathfinder;
    Grid grid;
    Turret[,] turretGrid;

    private void Awake() {
        inst = this;
        grid = FindObjectOfType<Grid>();
        pathfinder = GetComponent<Pathfinder>();
    }

    private void Start() {
        turretGrid = new Turret[grid.gridSizeX, grid.gridSizeY];
        foreach (Turret t in FindObjectsOfType<Turret>()) {
            turretGrid[grid.NodeFromWorldPoint(t.transform.position).gridX, grid.NodeFromWorldPoint(t.transform.position).gridY] = t;
        }

        UpdateInfoText();
        StartCoroutine(SpawnEnemies());
    }

    private void Update() {

        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentGridHover = grid.NodeFromWorldPoint(mouseWorldPos);

        if (Input.GetKeyDown(KeyCode.B)) {
            ToggleBuildMode(!buildMode);
        }

        if (buildMode) {
            turretVisualPlacer.position = currentGridHover.worldPos;

            if (Input.GetMouseButton(0) && playerMoney >= 15 && turretGrid[currentGridHover.gridX, currentGridHover.gridY] == null
                && grid.NodeFromWorldPoint(mouseWorldPos).walkable && Input.mousePosition.y > Screen.height * .1f) {
                var newTower = Instantiate(turrets[selectedTurretIndex], currentGridHover.worldPos, Quaternion.identity);
                grid.UpdateUnwalkableCells();

                bool validPathExists = pathfinder.FindPath(enemySpawnPoint.position, enemyTarget.position);

                if (validPathExists) {
                    AudioManager.am.PlaySound(0);
                    turretGrid[currentGridHover.gridX, currentGridHover.gridY] = newTower.GetComponent<Turret>();
                    foreach (Pathfinder f in FindObjectsOfType<Pathfinder>()) f.UpdatePath();
                    UpdateMoney(-15);
                } else {
                    Destroy(newTower);
                    grid.UpdateUnwalkableCells();
                }
            }
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) {
                ToggleBuildMode(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && !gameOver) { Time.timeScale = 3; if (waitTimer > 0) waitTimer = 0; }
        if (Input.GetKeyUp(KeyCode.F) && !gameOver) Time.timeScale = 1;
    }

    private void LateUpdate() {
        int segments = 40;
        float angle = 20f;
        float radius = 2;
        line.positionCount = segments + 1;

        for (int i = 0; i < (segments + 1); i++) {
            line.SetPosition(i, new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle) * radius, Mathf.Cos(Mathf.Deg2Rad * angle) * radius, 0));
            angle += (360f / segments);
        }
    }

    public void ToggleBuildMode(bool buildModeActive) {
        buildMode = buildModeActive;
        turretVisualPlacer.gameObject.SetActive(buildMode);
        if (buildMode) {
            floor.GetComponent<MeshRenderer>().material = materials[1];
            turretVisualPlacer.GetChild(0).GetComponent<SpriteRenderer>().sprite = turrets[selectedTurretIndex].GetComponent<SpriteRenderer>().sprite;
            turretVisualPlacer.GetChild(0).localScale = turrets[selectedTurretIndex].transform.localScale;
        } else {
            floor.GetComponent<MeshRenderer>().material = materials[0];
        }
    }

    public void SelectTurretToBuild(int index) {
        selectedTurretIndex = index;
        ToggleBuildMode(true);
    }

    public void UpdateMoney(int amount) {
        playerMoney += amount;
        UpdateInfoText();
    }

    public void UpdateInfoText() {
        infoText.text = "Wave: " + currentLevel + "      $" + playerMoney;
    }

    public void PlayerTakeDamage() {
        if (!gameOver) {
            playerHp--;
            if (playerHp <= 0) {
                endGameText.gameObject.SetActive(true);
                endGameText.text = "You Lose!";
                gameOver = true;
                Time.timeScale = .1f;
            } else {
                AudioManager.am.PlaySound(3);
            }
        }
    }

    IEnumerator WaitTimer() {
        while (waitTimer > 0) {
            waitTimer -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator SpawnEnemies() {

        while (!gameOver) {

            waitTimer = 10f;
            StartCoroutine(WaitTimer());
            yield return new WaitUntil(() => waitTimer <= 0);

            int enemiesToSpawn = 6 + Mathf.Min(16, currentLevel) * 3;

            while (enemiesToSpawn > 0) {
                var newEnemy = Instantiate(enemyPrefab,(Vector2)enemySpawnPoint.position, Quaternion.identity);
                enemies.Add(newEnemy.GetComponent<Enemy>());
                enemiesToSpawn--;
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitUntil(() => enemies.Count == 0);

            currentLevel++;
            UpdateInfoText();

            if (currentLevel > 25) {
                endGameText.gameObject.SetActive(true);
                endGameText.text = "You Win!!!";
                AudioManager.am.PlaySound(4);
                gameOver = true;
            }
        }
    }




}
