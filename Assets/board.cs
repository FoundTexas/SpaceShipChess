using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class board : MonoBehaviour
{
    public int players;
    static board inst;
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
            Vector3 vect = item.gameObject.name.Contains("1") ?
            new Vector3(1, 1, Mathf.RoundToInt(colums / 2)) : new Vector3(rows - 2, 1, Mathf.RoundToInt(colums / 2));

            item.SetSelector(vect, new Vector2(rows - 1, colums - 1));
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
            return null;

        return inst.matrix[(int)cord.x, (int)cord.y];
    }

    static public void UpdateCord(Vector2 cord, Vector2 prevcord, GameObject obj)
    {
        if ((cord.x < 0 || cord.y < 0) || (cord.x >= inst.rows || cord.y >= inst.colums))
            return;

        inst.matrix[(int)prevcord.x, (int)prevcord.y] =  null;

        inst.matrix[(int)cord.x, (int)cord.y] =  obj;
    }
}
