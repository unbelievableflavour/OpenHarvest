using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData : MonoBehaviour
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new Dictionary<Vector3Int, PlacementData>();
    public void addObjectAt(
        Vector3Int gridPosition, 
        Vector2Int objectSize, 
        int ID,
        int placedObjectIndex
    ){
        List<Vector3Int> positionToOccupy = calculatePosition(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach(var pos in positionToOccupy){
            if(placedObjects.ContainsKey(pos)){
                Debug.Log($"Dictionary already contains this {pos}");
            }
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> calculatePosition(Vector3Int gridPosition, Vector2Int objectSize) {
        List<Vector3Int> returnVal = new List<Vector3Int>();
        for(int x = 0; x < objectSize.x; x++){
            for(int y = 0; y < objectSize.y; y++){
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool canPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize){
        List<Vector3Int> positionToOccupy = calculatePosition(gridPosition, objectSize);
        foreach(var pos in positionToOccupy){
            if(placedObjects.ContainsKey(pos)){
                return false;
            }
        }
        return true;
    }
}

public class PlacementData {
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set;}
    public int placedObjectIndex { get; private set;}
    public PlacementData(List<Vector3Int> occupiedPositions, int ID, int placedObjectIndex) {
        this.occupiedPositions = occupiedPositions;
        this.ID = ID;
        this.placedObjectIndex = placedObjectIndex;
    }
}