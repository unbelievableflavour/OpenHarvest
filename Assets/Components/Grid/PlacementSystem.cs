using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator;
    
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private GridData floorData, furnitureData;

    private List<GameObject> placedGameObjects = new List<GameObject>();

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    void Start() {
        StopPlacement();
        floorData = new GridData();
        furnitureData = new GridData();

        OnToggleMode();
    }

    public void OnEnable() {
        GameState.Instance.OnToggleMode += OnToggleMode;
    }

    public void OnDisable() {
        GameState.Instance.OnToggleMode -= OnToggleMode;
    }

    void OnToggleMode() {
        if(GameState.Instance.GetMode() == "build") {
            StartPlacement(0);
        } else {
            StopPlacement();
        }
    }

    private void Update(){
        if(!HarvestInputManager.Instance) {
            return;
        }
        
        if(selectedObjectIndex == -1) {
            return;
        }

        Vector3 mousePosition = HarvestInputManager.Instance.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        if(lastDetectedPosition == gridPosition){
            return;
        }
        
        lastDetectedPosition = gridPosition;
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        mouseIndicator.transform.position = mousePosition;

        preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
    }

    private void StartPlacement(int ID) {
        StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        var currentObject = database.objectsData[selectedObjectIndex];

        if(currentObject == null) {
            Debug.Log("selectObjectIndex not set");
            return;
        }

        gridVisualization.SetActive(true);
        preview.StartShowingPlacementPreview(currentObject.prefab, currentObject.size);
        HarvestInputManager.Instance.OnTriggerRight += PlaceStructure;
        HarvestInputManager.Instance.OnBButton += OnPreviousItem;
        HarvestInputManager.Instance.OnAButton += OnNextItem;
    }

    private void PlaceStructure() {
        Vector3 mousePosition = HarvestInputManager.Instance.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if(!placementValidity){
            return;
        }
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
        placedGameObjects.Add(newObject);
    
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 
        ? floorData 
        : furnitureData;

        selectedData.addObjectAt(
            gridPosition, 
            database.objectsData[selectedObjectIndex].size,
            database.objectsData[selectedObjectIndex].ID,
            placedGameObjects.Count - 1
        );

        preview.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private void StopPlacement() {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        preview.StopShowingPreview();
        HarvestInputManager.Instance.OnTriggerRight -= PlaceStructure;
        HarvestInputManager.Instance.OnBButton -= OnPreviousItem;
        HarvestInputManager.Instance.OnAButton -= OnNextItem;

        lastDetectedPosition = Vector3Int.zero;
    }

    public void PlaceStructureWithoutPreview(Vector3Int gridPosition, int selectedObjectIndex) {
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
        placedGameObjects.Add(newObject);
    
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 
        ? floorData 
        : furnitureData;

        selectedData.addObjectAt(
            gridPosition, 
            database.objectsData[selectedObjectIndex].size,
            database.objectsData[selectedObjectIndex].ID,
            placedGameObjects.Count - 1
        );
    }

    public bool CheckPlacementValidity (Vector3Int gridPosition,  int selectedObjectIndex) {
        // if floor tile?
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 
        ? floorData 
        : furnitureData;

        return selectedData.canPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].size);
    }

    private void OnPreviousItem () {
        if(selectedObjectIndex > 0 ){
            StartPlacement(selectedObjectIndex - 1);
        }
    }

    private void OnNextItem () {
        if(selectedObjectIndex < database.objectsData.Count - 1 ) {
            StartPlacement(selectedObjectIndex + 1);
        }
    }
}