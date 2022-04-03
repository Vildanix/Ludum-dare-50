using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Grid : MonoBehaviour
{
    [SerializeField, Min(1)] int width;
    [SerializeField, Min(1)] int height;
    [SerializeField] int offsetX;
    [SerializeField] int offsetY;
    [SerializeField] Cell cellPrefab;
    [SerializeField] int seedRandom;
    [SerializeField, Range(0f, 1f)] float poiProbability = 0.1f;
    [SerializeField, Range(0f, 1f)] float backgroundProbability = 0.2f;
    [SerializeField] int randomNoiseOffset;

    int lastOffsetX;
    int lastOffsetY;
    Dictionary<(int, int), Cell> cellReferences = new Dictionary<(int, int), Cell>();
    ObjectPool<Cell> cellPoll;

    private void Awake()
    {
        lastOffsetX = offsetX;
        lastOffsetY = offsetY;
        InitializePool();
        UpdateMapCells();
        Random.InitState(seedRandom);
        randomNoiseOffset = Random.Range(10, 100);
    }

    private void InitializePool()
    {
        cellPoll = new ObjectPool<Cell>(CreateCell, OnTakeFromPool, OnReturnToPool, maxSize: width * height * 2);
    }

    public void Initialize()
    {
        lastOffsetX = offsetX = 0;
        lastOffsetY = offsetY = 0;
        UpdateMapCells();
    }

    private void Update()
    {
        if (offsetX != lastOffsetX || offsetY != lastOffsetY)
        {
            UpdateMapCells();
        }

    }

    public Cell GetCellAtPosition(Vector3 position)
    {
        var posX = Mathf.RoundToInt(position.x);
        var posY = Mathf.RoundToInt(position.z);
        if (cellReferences.ContainsKey((posX, posY))) {
            return cellReferences[(posX, posY)];
        }

        return null;
    }

    public void SetCenterPosition(Vector3 position)
    {
        var coords = GetCellCoordsFromPoint(position);
        offsetX = coords.Item1;
        offsetY = coords.Item2;
    }

    private void UpdateMapCells()
    {
        int differenceX = Mathf.Abs(offsetX - lastOffsetX);
        int differenceY = Mathf.Abs(offsetY - lastOffsetY);
        int halfWidth = Mathf.RoundToInt(width / 2.0f);
        int halfHeight = Mathf.RoundToInt(height / 2.0f);
        for (int x = offsetX - halfWidth - differenceX; x <= offsetX + halfWidth + differenceX; x++)
        {
            for (int y = offsetY - halfHeight - differenceY; y <= offsetY + halfHeight + differenceY; y++)
            {
                if (IsInVisibleArea(x, halfWidth, y, halfHeight) && !cellReferences.ContainsKey((x, y)))
                {
                    InitializeCell(x, y);
                }
                else if (!IsInVisibleArea(x, halfWidth, y, halfHeight) && cellReferences.ContainsKey((x, y)))
                {
                    cellReferences[(x, y)].RemoveCell();
                    cellReferences.Remove((x, y));
                }
            }
        }

        lastOffsetX = offsetX;
        lastOffsetY = offsetY;
    }

    private (int, int) GetCellCoordsFromPoint(Vector3 point)
    {
        return (Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.z));
    }

    private void InitializeCell(int x, int y)
    {
        var newCell = cellPoll.Get();
        newCell.transform.position = new Vector3(x, 0, y);
        newCell.name = $"Cell {x}, {y}";
        newCell.InitContent(x + randomNoiseOffset,
                            y + randomNoiseOffset,
                            poiProbability,
                            backgroundProbability);
        cellReferences.Add((x, y), newCell);
    }

    private bool IsInVisibleArea(int x, int halfWidth, int y, int halfHeight)
    {
        return IsInRange(x, offsetX - halfWidth, offsetX + halfWidth) && IsInRange(y, offsetY - halfHeight, offsetY + halfHeight);
    }

    public bool IsInRange(int testValue, int start, int end)
    {
        if (start > end)
            return testValue >= end && testValue <= start;
        return testValue >= start && testValue <= end;
    }

    public List<Cell> GetPathBetweenCells(Cell startCell, Cell targetCell)
    {
        var path = new List<Cell>();
        var currentPos = GetCellCoordsFromPoint(startCell.transform.position);
        var endPos = GetCellCoordsFromPoint(targetCell.transform.position);

        //path.Add(startCell); // TODO: replace with current player position to prevent rendering path from last cell during movement

        int totalDistance = Mathf.Max(Mathf.Abs(endPos.Item1 - currentPos.Item1), Mathf.Abs(endPos.Item2 - currentPos.Item2));
        for (int step = 0; step < totalDistance; step++ )
        {
            var stepDirection = GetOptimalStepDirection(currentPos, endPos);
            currentPos.Item1 += stepDirection.Item1;
            currentPos.Item2 += stepDirection.Item2;
            if (cellReferences.ContainsKey(currentPos))
            {
                path.Add(cellReferences[currentPos]);
            }
        }
        return path;
    }

    private (int, int) GetOptimalStepDirection((int, int) currentPos, (int, int) endPos)
    {
        var horizontalDiff = Mathf.Abs(currentPos.Item1 - endPos.Item1);
        var verticalDiff = Mathf.Abs(currentPos.Item2 - endPos.Item2);
        if (horizontalDiff > verticalDiff)
        {
            return (Mathf.RoundToInt(Mathf.Sign(endPos.Item1 - currentPos.Item1)), 0);
        }
        if (horizontalDiff < verticalDiff)
        {
            return (0, Mathf.RoundToInt(Mathf.Sign(endPos.Item2 - currentPos.Item2)));
        }

        return (Mathf.RoundToInt(Mathf.Sign(endPos.Item1 - currentPos.Item1)), Mathf.RoundToInt(Mathf.Sign(endPos.Item2 - currentPos.Item2))); 
    }

    #region Cell pooling functions
    private Cell CreateCell()
    {
        var cell  = Instantiate<Cell>(cellPrefab, transform);
        cell.SetPool(cellPoll);
        return cell;
    }

    private void OnTakeFromPool(Cell cell)
    {
        cell.gameObject.SetActive(true);
    }

    private void OnReturnToPool(Cell cell)
    {
        cell.gameObject.SetActive(false);
    }

    #endregion
}
