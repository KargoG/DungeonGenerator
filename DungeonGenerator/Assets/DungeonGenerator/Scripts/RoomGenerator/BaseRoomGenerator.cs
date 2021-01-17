using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class BaseRoomGenerator : MonoBehaviour
{
    [SerializeField] private float _boxSize = 10;
    [SerializeField] private float _sizeChangePerIteration = 0.6f;
    private Mesh _toShow;

    private List<Bounds> _RectsInMesh = new List<Bounds>();

    // Start is called before the first frame update
    void Start()
    {
        _toShow = new Mesh();
        //_toShow = CubeGenerator.GenerateCube(10, 3, new Vector3(0, 0, 0));
        //_RectsInMesh.Add(new Bounds(new Vector3(0, 0, 0), new Vector3(20, 20, 20) * 0.99f));

        AddCubeAtAnchor(0);
        AddCubeAtAnchor(1);

        GetComponent<MeshFilter>().mesh = _toShow;
    }

    private void AddCubeAtAnchor(int iteration) // TODO calculate iteration in anchor method
    {
        Vector3 anchorPos = new Vector3(10, 10, 10); // TODO replace for real anchors

        if(iteration == 0)
            anchorPos = new Vector3(0, 0, 0); // TODO remove later

        Mesh secondMesh = CubeGenerator.GenerateCube(_boxSize * Mathf.Pow(_sizeChangePerIteration, iteration), 3, anchorPos);

        AddShownMesh(ref secondMesh, new Bounds(anchorPos, new Vector3(20, 20, 20) * Mathf.Pow(_sizeChangePerIteration, iteration) * 0.99f));
    }

    private void AddShownMesh(ref Mesh secondMesh, Bounds newSquareBounds)
    {
        List<Vector3> firstMeshVerts = new List<Vector3>(_toShow.vertices);
        List<Vector3> secondMeshVerts = new List<Vector3>(secondMesh.vertices);
        List<int> firstMeshTris = new List<int>(_toShow.triangles);
        List<int> secondMeshTris = new List<int>(secondMesh.triangles);


        List<int> indexesOfOverlappingVerts = new List<int>();

        for (int i = 0; i < firstMeshVerts.Count; i++)
        {
            if (newSquareBounds.Contains(firstMeshVerts[i]))
            {
                indexesOfOverlappingVerts.Add(i);
            }
        }

        while (indexesOfOverlappingVerts.Count > 0)
        {
            if (firstMeshTris.Contains(indexesOfOverlappingVerts[0]))
            {
                int foundPos = firstMeshTris.FindIndex((int i) => { return indexesOfOverlappingVerts[0] == i; });

                switch (foundPos % 3)
                {
                    case 0:
                        firstMeshTris.RemoveAt(foundPos);
                        firstMeshTris.RemoveAt(foundPos);
                        firstMeshTris.RemoveAt(foundPos);
                        break;
                    case 1:
                        firstMeshTris.RemoveAt(foundPos - 1);
                        firstMeshTris.RemoveAt(foundPos - 1);
                        firstMeshTris.RemoveAt(foundPos - 1);
                        break;
                    case 2:
                        firstMeshTris.RemoveAt(foundPos - 2);
                        firstMeshTris.RemoveAt(foundPos - 2);
                        firstMeshTris.RemoveAt(foundPos - 2);
                        break;
                }
            }
            else
            {
                indexesOfOverlappingVerts.RemoveAt(0);
            }
        }

        foreach (Bounds bounds in _RectsInMesh)
        {
            indexesOfOverlappingVerts = new List<int>();

            for (int i = 0; i < secondMeshVerts.Count; i++)
            {
                if (bounds.Contains(secondMeshVerts[i]))
                {
                    indexesOfOverlappingVerts.Add(i);
                }
            }

            while (indexesOfOverlappingVerts.Count > 0)
            {
                if (secondMeshTris.Contains(indexesOfOverlappingVerts[0]))
                {
                    int foundPos = secondMeshTris.FindIndex((int i) => { return indexesOfOverlappingVerts[0] == i; });

                    switch (foundPos % 3)
                    {
                        case 0:
                            secondMeshTris.RemoveAt(foundPos);
                            secondMeshTris.RemoveAt(foundPos);
                            secondMeshTris.RemoveAt(foundPos);
                            break;
                        case 1:
                            secondMeshTris.RemoveAt(foundPos - 1);
                            secondMeshTris.RemoveAt(foundPos - 1);
                            secondMeshTris.RemoveAt(foundPos - 1);
                            break;
                        case 2:
                            secondMeshTris.RemoveAt(foundPos - 2);
                            secondMeshTris.RemoveAt(foundPos - 2);
                            secondMeshTris.RemoveAt(foundPos - 2);
                            break;
                    }
                }
                else
                {
                    indexesOfOverlappingVerts.RemoveAt(0);
                }
            }
        }





        for (int i = 0; i < secondMeshTris.Count; i++)
        {
            secondMeshTris[i] += firstMeshVerts.Count;
        }

        firstMeshVerts.AddRange(secondMeshVerts);
        firstMeshTris.AddRange(secondMeshTris);

        _toShow.vertices = firstMeshVerts.ToArray();
        _toShow.triangles = firstMeshTris.ToArray();
        _RectsInMesh.Add(newSquareBounds);
    }

    void OnDrawGizmosSelected()
    {
        if (!_toShow)
            return;
        //Gizmos.color = Color.blue;
        //foreach (Vector3 vertex in toShow.vertices)
        //{
        //    Gizmos.DrawSphere(vertex, 0.05f);
        //}

        //Gizmos.color = Color.white;
        //for (int i = 0; i < toShow.triangles.Length; i+=3)
        //{
        //    Gizmos.DrawLine(toShow.vertices[toShow.triangles[i]], toShow.vertices[toShow.triangles[i + 1]]);
        //    Gizmos.DrawLine(toShow.vertices[toShow.triangles[i + 1]], toShow.vertices[toShow.triangles[i + 2]]);
        //    Gizmos.DrawLine(toShow.vertices[toShow.triangles[i + 2]], toShow.vertices[toShow.triangles[i]]);
        //}
    }

}
