using UnityEngine;

public abstract class Stage : MonoBehaviour
{
    private GameObject _obstaclePrefab;
    private Vector2 _cellsDimension;
    private LayerMask _whatIsWall;
    private int _rows;
    private int _columns;

    private Stage()
    {
        _rows = 10;
        _columns = 10;
        _cellsDimension = new Vector2(1f, 1f);
        _obstaclePrefab = new GameObject();
    }
    
    protected Stage(GameObject obstaclePrefab, Vector2 cellsDimension, LayerMask whatIsWall, int rows, int columns)
    {
        _obstaclePrefab = obstaclePrefab;
        _cellsDimension = cellsDimension;
        _whatIsWall = whatIsWall;
        _rows = rows;
        _columns = columns;
    }

    public virtual void CreateStage()
    {
        Instantiate(_obstaclePrefab, new Vector3(0, 0, 0), Quaternion.identity);
    } 
}