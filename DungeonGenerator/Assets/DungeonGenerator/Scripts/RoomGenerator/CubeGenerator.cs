using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class CubeGenerator : MonoBehaviour
{
    private static Vector3[] _baseCubeVertices;
    private static Vector3[] _baseCubeNormals;
    private static int[] _baseCubeTris;

    public static Mesh GenerateCube(float halfSize, int subdivisions, Vector3 offset)
    {
        if (_baseCubeVertices == null)
        {
            CreateBaseCube();
        }

        List<Vector3> wantedCubeVertices = new List<Vector3>(_baseCubeVertices);
        List<Vector3> wantedCubeNormals = new List<Vector3>(_baseCubeNormals);
        List<int> wantedCubeTris = new List<int>(_baseCubeTris);


        for (int i = 0; i < subdivisions; i++)
        {
            SubdivideCube(ref wantedCubeVertices, ref wantedCubeTris, ref wantedCubeNormals);
        }

        //CalculateNormals(ref wantedCubeNormals, ref wantedCubeVertices, ref wantedCubeTris);

        for (int i = 0; i < wantedCubeVertices.Count; i++)
        {
            wantedCubeVertices[i] = wantedCubeVertices[i] * halfSize;
        }

        for (int i = 0; i < wantedCubeVertices.Count; i++)
        {
            wantedCubeVertices[i] = wantedCubeVertices[i] + offset;
        }


        Mesh baseCube = new Mesh();
        baseCube.vertices = wantedCubeVertices.ToArray();
        baseCube.normals = wantedCubeNormals.ToArray();
        baseCube.triangles = wantedCubeTris.ToArray();

        return baseCube;
    }

    private static void CalculateNormals(ref List<Vector3> cubeNormals, ref List<Vector3> cubeVertices, ref List<int> cubeTris)
    {
        cubeNormals.Clear();
        foreach (Vector3 vertex in cubeVertices)
        {
            Vector3 newNormal = new Vector3();
            int toDivide = 0;

            for (int i = 0; i < cubeTris.Count; i++)
            {
                if (cubeVertices[cubeTris[i]] == vertex)
                {
                    switch (i % 3)
                    {
                        case 0:
                            newNormal += Vector3.Cross(cubeVertices[cubeTris[i + 1]] - vertex, cubeVertices[cubeTris[i + 2]] - vertex);
                            break;
                        case 1:
                            newNormal += Vector3.Cross(cubeVertices[cubeTris[i + 1]] - vertex, cubeVertices[cubeTris[i - 1]] - vertex);
                            break;
                        case 2:
                            newNormal += Vector3.Cross(cubeVertices[cubeTris[i - 2]] - vertex, cubeVertices[cubeTris[i - 1]] - vertex);
                            break;
                    }

                    toDivide++;
                }
            }

            cubeNormals.Add(newNormal / toDivide);
        }
    }

    private static int GetNewVertex(ref List<Vector3> verticis, Vector3 newVert, ref List<Vector3> cubeNormals)
    {
        if (verticis.Contains(newVert))
        {
            return verticis.IndexOf(newVert);
        }

        verticis.Add(newVert);
        cubeNormals.Add(Vector3.up);
        return verticis.Count - 1;
    }

    private static void SubdivideCube(ref List<Vector3> cubeVertices, ref List<int> cubeTris, ref List<Vector3> cubeNormals)
    {
        List<int> newTris = new List<int>();

        for (int i = 0; i < cubeTris.Count; i+=3)
        {
            int firstVertPos = cubeTris[i];
            int secondVertPos = cubeTris[i+1];
            int thirdVertPos = cubeTris[i+2];

            int firstNewVertPos = GetNewVertex(ref cubeVertices, (cubeVertices[firstVertPos] + cubeVertices[secondVertPos]) / 2, ref cubeNormals);
            int secondNewVertPos = GetNewVertex(ref cubeVertices, (cubeVertices[secondVertPos] + cubeVertices[thirdVertPos]) / 2, ref cubeNormals);
            int thirdNewVertPos = GetNewVertex(ref cubeVertices, (cubeVertices[thirdVertPos] + cubeVertices[firstVertPos]) / 2, ref cubeNormals);

            cubeNormals[firstNewVertPos] = (cubeNormals[firstVertPos] + cubeNormals[secondVertPos]) / 2;
            cubeNormals[secondNewVertPos] = (cubeNormals[secondVertPos] + cubeNormals[thirdVertPos]) / 2;
            cubeNormals[thirdNewVertPos] = (cubeNormals[thirdVertPos] + cubeNormals[firstVertPos]) / 2;


            newTris.Add(firstVertPos); newTris.Add(firstNewVertPos); newTris.Add(thirdNewVertPos);

            newTris.Add(firstNewVertPos); newTris.Add(secondVertPos); newTris.Add(secondNewVertPos);

            newTris.Add(thirdNewVertPos); newTris.Add(firstNewVertPos); newTris.Add(secondNewVertPos);

            newTris.Add(thirdVertPos); newTris.Add(thirdNewVertPos); newTris.Add(secondNewVertPos);
        }

        cubeTris = newTris;
    }

    private static void CreateBaseCube()
    {
        _baseCubeVertices = new Vector3[]
        {
            new Vector3(-1, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1),

            new Vector3(-1, 1, -1),
            new Vector3(1, 1, -1),
            new Vector3(-1, -1, -1),
            new Vector3(1, -1, -1)
        };

        _baseCubeNormals = new Vector3[]
        {
            new Vector3(1, -1, -1),
            new Vector3(-1, -1, -1),
            new Vector3(1, 1, -1),
            new Vector3(-1, 1, -1),

            new Vector3(1, -1, 1),
            new Vector3(-1, -1, 1),
            new Vector3(1, 1, 1),
            new Vector3(-1, 1, 1)
        };

        _baseCubeTris = new int[]
        {
            // back
            0, 1, 2,
            2, 1, 3,

            // front
            6, 5, 4,
            7, 5, 6,

            // left
            4, 0, 6,
            6, 0, 2,

            // right
            1, 5, 3,
            3, 5, 7,

            // top
            0, 4, 5,
            0, 5, 1,

            // bottom
            6, 2, 3,
            6, 3, 7

        };
    }
}
