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


        //for (int i = 0; i < subdivisions; i++)
        {
            SubdivideCube(ref wantedCubeVertices, ref wantedCubeTris, ref wantedCubeNormals, subdivisions);
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
        //if (verticis.Contains(newVert))
        //{
        //    return verticis.IndexOf(newVert);
        //}

        verticis.Add(newVert);
        cubeNormals.Add(Vector3.zero);
        return verticis.Count - 1;
    }

    private static void SubdivideCube(ref List<Vector3> cubeVertices, ref List<int> cubeTris, ref List<Vector3> cubeNormals, int subdivisions)
    {
        List<int> newTris = new List<int>();

        for (int i = 0; i < cubeTris.Count; i+=3)
        {
            SubdivideTriangle(cubeTris[i], cubeTris[i + 1], cubeTris[i + 2], ref cubeVertices, ref newTris, ref cubeNormals, subdivisions);
        }

        cubeTris = newTris;
    }

    static void SubdivideTriangle(int firstIndex, int secondIndex, int thirdIndex, ref List<Vector3> cubeVertices, ref List<int> newTris, ref List<Vector3> cubeNormals, int subdivisions)
    {
        List<int> aSideVerts = new List<int>();
        List<int> bSideVerts = new List<int>();
        List<int> cSideVerts = new List<int>();

        aSideVerts.Add(firstIndex);
        bSideVerts.Add(secondIndex);
        cSideVerts.Add(thirdIndex);

        Vector3 aToBVec = (cubeVertices[bSideVerts[0]] - cubeVertices[aSideVerts[0]]);
        Vector3 bToCVec = (cubeVertices[cSideVerts[0]] - cubeVertices[bSideVerts[0]]);
        Vector3 cToAVec = (cubeVertices[aSideVerts[0]] - cubeVertices[cSideVerts[0]]);

        for (int i = 0; i < subdivisions; i++)
        {
            int aNewVertPos = GetNewVertex(ref cubeVertices, cubeVertices[aSideVerts[0]] + (aToBVec / (subdivisions + 1f) * (i + 1f)), ref cubeNormals);
            int bNewVertPos = GetNewVertex(ref cubeVertices, cubeVertices[bSideVerts[0]] + (bToCVec / (subdivisions + 1f) * (i + 1f)), ref cubeNormals);
            int cNewVertPos = GetNewVertex(ref cubeVertices, cubeVertices[cSideVerts[0]] + (cToAVec / (subdivisions + 1f) * (i + 1f)), ref cubeNormals);

            aSideVerts.Add(aNewVertPos);
            bSideVerts.Add(bNewVertPos);
            cSideVerts.Add(cNewVertPos);

            cubeNormals[aNewVertPos] = cubeNormals[firstIndex];
            cubeNormals[bNewVertPos] = cubeNormals[firstIndex];
            cubeNormals[cNewVertPos] = cubeNormals[firstIndex];
        }


        List<int> centerVerts = new List<int>();

        for (int i = 2; i < aSideVerts.Count; i++)
        {
            Vector3 edgeToEdgeVec = (cubeVertices[aSideVerts[i]] - cubeVertices[cSideVerts[cSideVerts.Count - i]]);
            for (int j = 0; j < i-1; j++)
            {
                int newVertPos = GetNewVertex(ref cubeVertices, cubeVertices[cSideVerts[cSideVerts.Count - i]] + (edgeToEdgeVec / i * (j + 1)), ref cubeNormals);
                centerVerts.Add(newVertPos);
                cubeNormals[newVertPos] = cubeNormals[firstIndex];
            }
        }

        List<List<int>> triangles = new List<List<int>>();

        triangles.Add(new List<int>());
        triangles[0].Add(aSideVerts[0]);
        int currentCenterVert = 0;

        for (int i = 1; i < aSideVerts.Count; i++)
        {
            triangles.Add(new List<int>());
            triangles[i].Add(cSideVerts[cSideVerts.Count - i]);

            for (int j = 0; j < i - 1; j++)
            {
                triangles[i].Add(centerVerts[currentCenterVert++]);
            }

            triangles[i].Add(aSideVerts[i]);
        }
        triangles.Add(new List<int>());
        triangles[triangles.Count - 1].Add(cSideVerts[0]);
        for (int i = bSideVerts.Count -1; i >= 0; i--)
        {
            triangles[triangles.Count - 1].Add(bSideVerts[i]);
        }

        // Create Triangles
        for (int i = 1; i < triangles.Count; i++)
        {
            for (int j = 0; j < triangles[i].Count - 1; j++)
            {
                newTris.Add(triangles[i-1][j]); newTris.Add(triangles[i][j + 1]); newTris.Add(triangles[i][j]);
                if (i < triangles.Count - 1)
                {
                    newTris.Add(triangles[i][j]);
                    newTris.Add(triangles[i][j + 1]);
                    newTris.Add(triangles[i + 1][j + 1]);
                }
            }
        }


        // TODO fallback
        //int firstNewVertPos = GetNewVertex(ref cubeVertices, (cubeVertices[firstIndex] + cubeVertices[secondIndex]) / 2, ref cubeNormals);
        //int secondNewVertPos = GetNewVertex(ref cubeVertices, (cubeVertices[secondIndex] + cubeVertices[thirdIndex]) / 2, ref cubeNormals);
        //int thirdNewVertPos = GetNewVertex(ref cubeVertices, (cubeVertices[thirdIndex] + cubeVertices[firstIndex]) / 2, ref cubeNormals);


        //cubeNormals[firstNewVertPos] = cubeNormals[firstIndex];
        //cubeNormals[secondNewVertPos] = cubeNormals[firstIndex];
        //cubeNormals[thirdNewVertPos] = cubeNormals[firstIndex];


        //newTris.Add(firstIndex); newTris.Add(firstNewVertPos); newTris.Add(thirdNewVertPos);

        //newTris.Add(firstNewVertPos); newTris.Add(secondIndex); newTris.Add(secondNewVertPos);

        //newTris.Add(thirdNewVertPos); newTris.Add(firstNewVertPos); newTris.Add(secondNewVertPos);

        //newTris.Add(thirdIndex); newTris.Add(thirdNewVertPos); newTris.Add(secondNewVertPos);
    }

    private static void CreateBaseCube()
    {
        _baseCubeVertices = new Vector3[]
        {
            // Front
            new Vector3(-1, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1),
            
            // Back
            new Vector3(1, 1, -1),
            new Vector3(-1, 1, -1),
            new Vector3(1, -1, -1),
            new Vector3(-1, -1, -1),
            
            // Left
            new Vector3(-1, 1, -1),
            new Vector3(-1, 1, 1),
            new Vector3(-1, -1, -1),
            new Vector3(-1, -1, 1),
            
            // Right
            new Vector3(1, 1, 1),
            new Vector3(1, 1, -1),
            new Vector3(1, -1, 1),
            new Vector3(1, -1, -1),
            
            // Top
            new Vector3(-1, 1, -1),
            new Vector3(1, 1, -1),
            new Vector3(-1, 1, 1),
            new Vector3(1, 1, 1),
            
            // Bottom
            new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1),
            new Vector3(-1, -1, -1),
            new Vector3(1, -1, -1)
        };

        _baseCubeNormals = new Vector3[]
        {
            // Front
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),

            // Back
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),

            // Left
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),

            // Right
            new Vector3(-1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(-1, 0, 0),

            // Top
            new Vector3(0, -1, 0),
            new Vector3(0, -1, 0),
            new Vector3(0, -1, 0),
            new Vector3(0, -1, 0),

            // Bottom
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0)
        };

        _baseCubeTris = new int[]
        {
            // back
            0, 1, 2,
            2, 1, 3,

            // front
            4, 5, 6,
            6, 5, 7,

            // left
            8, 9, 10,
            10, 9, 11,

            // right
            12, 13, 14,
            14, 13, 15,

            // top
            16, 17, 18,
            18, 17, 19,

            // bottom
            20, 21, 22,
            22, 21, 23
        };
    }
}
