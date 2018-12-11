using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public int numCells;  // number of independent cell per generation
    public int numGenerations; // number of generations
    public int initialDensity; // probability factor to define the random density of the first generation (0, 100)
    public int[] rules;
    public GameObject cellPrefab;
    public GameObject backgroundPrefab;

    private int[,] grid;
    private int ruleIndex;

	void Start () {
        ruleIndex = 0;
        CreateGrid();
        ShowGrid();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ruleIndex = (ruleIndex + 1) % rules.Length;
            RestartGrid();
        }
        if (Input.GetKey("escape")) {
            Application.Quit();
        }
    }

    void RestartGrid() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        CreateGrid();
        ShowGrid();
    }

    void GridInit() {
        grid = new int[numGenerations, numCells];
        for (int c = 0; c < numCells; c++) {
            grid[0, c] = initialDensity >= Random.Range(1, 101) ? 0 : 1;    // randomly generation of the first generation
        }
    }
	
    void CreateGrid() {
        GridInit();
        for (int g = 1; g < numGenerations; g++) {
            for (int c = 0; c < numCells; c++) {
                grid[g, c] = GetCellValue(g, c);
            }
        }
    }

    int GetCellValue(int generation, int cell) {
        int ruleBit = 1 + GetParent(generation - 1, cell, 1) + GetParent(generation - 1, cell, 0) * 2 + GetParent(generation - 1, cell, -1) * 4;
        bool bit = (rules[ruleIndex] & (1 << ruleBit - 1)) != 0;
        return bit ? 1 : 0;
    }

    int GetParent(int prevGeneration, int cell, int offset) {
        int parentCell = (numCells + cell + offset) % numCells;
        return grid[prevGeneration, parentCell];
    }

    void ShowGrid() {
        var background = Instantiate(backgroundPrefab, this.transform);
        float vScale = 1f / numGenerations;
        float hScale = 1f / numCells;
        float vSpace = 10f / numGenerations;
        float vInit = vSpace * (numGenerations / 2f);
        float hSpace = 10f / numCells;
        float hInit = hSpace * (numCells / 2f);


        for (int g = 0; g < numGenerations; g++) {
            for (int c = 0; c < numCells; c++) {
                if(grid[g, c] == 1) {
                    var x = (c * hSpace) - hInit + (hSpace / 2f);
                    var z = (g * vSpace) - vInit + (vSpace / 2f);
                    var cell = Instantiate(cellPrefab, new Vector3(x, 1.01f, -z), Quaternion.identity, this.transform);
                    cell.transform.localScale = new Vector3(hScale, 1f, vScale);
                }
            }
        }
    }
}
