using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DestroyDirection
{
    Front,
    Back,
    Left,
    Right
}
public class Wall : MonoBehaviour, IDamageable
{
    private float health;
    public float Health
    {
        get => health;
        set
        {
            health = value;
        }
    }
    private readonly string wallNameFormat = "Wall.{0}.{1}";
    private readonly int numOfDimension = 4;

    [SerializeField] List<GameObject> subWalls;
    private Vector3 currentCenter;
    private GameObject[,] subWallsGrid;
    private Vector3 lastDamageSourcePos;

    public Action<int, int> OnSubWallDestroy;
    public Action OnDie;

    private void Awake()
    {
        GetWallsGrid();
        lastDamageSourcePos = transform.position;
        currentCenter = transform.position;
    }

    private void GetWallsGrid()
    {
        subWallsGrid = new GameObject[numOfDimension, numOfDimension];
        Dictionary<string, GameObject> wallsMap = new Dictionary<string, GameObject>();
        foreach (var subWall in subWalls)
        {
            wallsMap[subWall.name] = subWall;
        }

        for (int i = 0; i < numOfDimension; i++)
        {
            for (int j = 0; j < numOfDimension; j++)
            {
                subWallsGrid[i, j] = wallsMap[String.Format(wallNameFormat, i, j)];
                subWallsGrid[i, j].AddComponent<SubWall>();
                subWallsGrid[i, j].GetComponent<SubWall>().SetRowAndCol(i, j);
            }
        }
    }

    private void Start()
    {
        Health = subWallsGrid.Length;
    }

    public void Die()
    {
        OnDie?.Invoke();

        Destroy(this.gameObject);
    }

    public void TakeDamage(float damage, byte damageDealerID, Vector3? damageSourcePos = null)
    {
        if (damageSourcePos != null && damageSourcePos != lastDamageSourcePos)
        {
            lastDamageSourcePos = damageSourcePos.Value;
            DestroySubWallsBasedOn(damageSourcePos.Value);
        }

        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }

    private void DestroySubWallsBasedOn(Vector3 damageSourcePos)
    {
        Vector3 dirToSource = damageSourcePos - currentCenter;
        bool shouldDestroyVerticalSubWalls = Mathf.Abs(dirToSource.z) >= Mathf.Abs(dirToSource.x);
        DestroyDirection destroyDirection;
        if (shouldDestroyVerticalSubWalls)
        {
            // Vertical
            // At this point we need to know what walls that need to be destroyed (front or back)
            destroyDirection = dirToSource.z >= 0 ? DestroyDirection.Front : DestroyDirection.Back;
        }
        else
        {
            // Horizontal
            // At this point we need to know what walls that need to be destroyed (left or right)
            destroyDirection = dirToSource.x >= 0 ? DestroyDirection.Right : DestroyDirection.Left;
        }

        switch (destroyDirection)
        {
            case DestroyDirection.Front:
                for (int row = 0; row < numOfDimension; row++)
                {
                    if (IsThisRowStillExist(row))
                    {
                        DestroyRow(row);
                        break;
                    }
                }
                break;

            case DestroyDirection.Back:
                for (int row = numOfDimension - 1; row >= 0; row--)
                {
                    if (IsThisRowStillExist(row))
                    {
                        DestroyRow(row);
                        break;
                    }
                }
                break;

            case DestroyDirection.Left:
                for (int col = 0; col < numOfDimension; col++)
                {
                    if (IsThisColStillExist(col))
                    {
                        DestroyCol(col);
                        break;
                    }
                }
                break;

            case DestroyDirection.Right:
                for (int col = numOfDimension - 1; col >= 0; col--)
                {
                    if (IsThisColStillExist(col))
                    {
                        DestroyCol(col);
                        break;
                    }
                }
                break;
        }

        CalculateCurrentCenter();
        Health--;
    }

    private void DestroyRow(int row)
    {
        for (int col = 0; col < numOfDimension; col++)
        {
            if (subWallsGrid[row, col] != null)
            {
                OnSubWallDestroy?.Invoke(row, col);
                subWallsGrid[row, col] = null;
            }
        }
    }

    private void DestroyCol(int col)
    {
        for (int row = 0; row < numOfDimension; row++)
        {
            if (subWallsGrid[row, col] != null)
            {
                OnSubWallDestroy?.Invoke(row, col);
                subWallsGrid[row, col] = null;
            }
        }
    }

    private bool IsThisRowStillExist(int row)
    {
        for (int col = 0; col < numOfDimension; col++)
        {
            if (subWallsGrid[row, col] != null)
                return true;
        }

        return false;
    }

    private bool IsThisColStillExist(int col)
    {
        for (int row = 0; row < numOfDimension; row++)
        {
            if (subWallsGrid[row, col] != null)
                return true;
        }

        return false;
    }

    private void CalculateCurrentCenter()
    {
        int counter = 0;
        currentCenter = Vector3.zero;
        foreach (var subWall in subWallsGrid)
        {
            if (subWall != null)
            {
                currentCenter += subWall.transform.position;
                counter++;
            }
        }

        currentCenter /= counter;
    }
}
