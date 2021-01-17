using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class CubeGenerator : MonoBehaviour
{
    private static Vector3[] _baseCubeVertices;
    private static int[] _baseCubeTris;

    public static Mesh GenerateCube(float halfSize, int subdivisions, Vector3 offset)
    {
        if (_baseCubeVertices == null)
        {
            CreateBaseCube();
        }

        List<Vector3> wantedCubeVertices = new List<Vector3>(_baseCubeVertices);
        List<int> wantedCubeTris = new List<int>(_baseCubeTris);


        for (int i = 0; i < subdivisions; i++)
        {
            SubdivideCube(ref wantedCubeVertices, ref wantedCubeTris);
        }

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
        baseCube.triangles = wantedCubeTris.ToArray();

        return baseCube;
    }

    private static int GetNewVertex(ref List<Vector3> verticis, Vector3 newVert)
    {
        if (verticis.Contains(newVert))
        {
            return verticis.IndexOf(newVert);
        }

        verticis.Add(newVert);
        return verticis.Count - 1;
    }

    private static void SubdivideCube(ref List<Vector3> cubeVertices, ref List<int> cubeTris)
    {
        List<int> newTris = new List<int>();

        for (int i = 0; i < cubeTris.Count; i+=3)
        {
            int firstVertPos = cubeTris[i];
            int secondVertPos = cubeTris[i+1];
            int thirdVertPos = cubeTris[i+2];

            int firstNewVertPos = GetNewVertex(ref cubeVertices, (cubeVertices[firstVertPos] + cubeVertices[secondVertPos]) / 2);
            int secondNewVertPos = GetNewVertex(ref cubeVertices, (cubeVertices[secondVertPos] + cubeVertices[thirdVertPos]) / 2);
            int thirdNewVertPos = GetNewVertex(ref cubeVertices, (cubeVertices[thirdVertPos] + cubeVertices[firstVertPos]) / 2);


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
