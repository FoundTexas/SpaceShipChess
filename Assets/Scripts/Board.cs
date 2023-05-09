using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    public int players;
    static Board inst;
    [Range(4, 100)] public int rows = 10, colums = 10;

    GameObject[,] matrix;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
        rows += 4;
        matrix = new GameObject[rows, colums];

        BoxCollider collider = GetComponent<BoxCollider>();

        collider.size = new Vector3((rows / 2) + 1, 40f, colums / 2);
        collider.center = new Vector3((rows - 1) / 2f, 1f, (colums - 1) / 2f);
        collider.isTrigger = true;

        Player[] players = FindObjectsOfType<Player>();

        foreach (var item in players)
        {
            Vector3Int vect = item.gameObject.name.Contains("1") ?
            new Vector3Int(1, 1, Mathf.RoundToInt(colums / 2)) : new Vector3Int(rows - 2, 1, Mathf.RoundToInt(colums / 2));

            item.SetSelector(vect, new Vector2Int(rows - 1, colums - 1));
        }

        GameObject cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject ObstacleObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ObstacleObject.GetComponent<Renderer>().material.color = Color.red;
        GameObject HPObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        HPObject.GetComponent<Renderer>().material.color = Color.green;

        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < colums; y++)
            {
                if (x > 1 && x < rows - 2)
                {
                    float val = Random.Range(0, 6);
                    matrix[x, y] = (val > 4) ? (Instantiate(ObstacleObject, new Vector3(x, 1, y), Quaternion.identity, transform)) :
                    ((val == 4) ? Instantiate(HPObject, new Vector3(x, 1, y), Quaternion.identity, transform) : null);
                    if (matrix[x, y])
                        matrix[x, y].name = "(" + x + ", " + y + ")";
                }

                GameObject g = Instantiate(cubeObject, new Vector3(x, 0, y), Quaternion.identity, transform);
                g.GetComponent<Renderer>().material.color = ((x + y) % 2 == 0) ? Color.black : Color.white;
                g.name = "(" + x + ", " + y + ")";
            }
        }

        Destroy(cubeObject);
        Destroy(ObstacleObject);
        Destroy(HPObject);
    }

    static public bool tryAdd(GameObject add, Vector3 cord)
    {
        if (!checkCord(new Vector2(cord.x, cord.z)))
        {
            inst.matrix[(int)cord.x, (int)cord.z] = add;
            add.transform.position = cord;
            return true;
        }

        return false;
    }

    static public GameObject checkCord(Vector2 cord)
    {
        if ((cord.x < 0 || cord.y < 0) || (cord.x >= inst.rows || cord.y >= inst.colums))
            return inst.gameObject;

        return inst.matrix[(int)cord.x, (int)cord.y];
    }

    public static Vector2Int FindObject(GameObject obj)
    {
        int row = -1;
        int column = -1;
        var list = inst.matrix.Cast<GameObject>().ToList();
        var index = list.IndexOf(obj);
        if (index == -1)
            return new Vector2Int(row, column);
        row = index / inst.matrix.GetLength(1);
        column = index % inst.matrix.GetLength(1);
        return new Vector2Int(row, column);
    }

    static public bool UpdateCord(Vector2Int cord, GameObject obj)
    {
        if ((cord.x < 0 || cord.y < 0) || (cord.x >= inst.rows || cord.y >= inst.colums))
            return false;

        Vector2Int remove = FindObject(obj);

        if(remove.x >= 0)
        {
            inst.matrix[remove.x, remove.y] = null;
        }

        inst.matrix[cord.x, cord.y] = obj;

        return true;
    }
}
