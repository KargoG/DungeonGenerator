using System;
using System.Collections;
using System.Collections.Generic;
using DungeonGenerator;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class BaseRoomGenerator : MonoBehaviour, IRoomCreator
{
    [SerializeField] private float _boxSize = 10;
    [SerializeField] private float _sizeChangePerIteration = 0.6f;

    private Dictionary<OperationSymbols, List<Tuple<Vector3, int>>> _anchorPoints = new Dictionary<OperationSymbols, List<Tuple<Vector3, int>>>();

    private Mesh _toShow;
    private List<Bounds> _RectsInMesh = new List<Bounds>();

    public void CreateRoom(DungeonRoom dungeonRoom) 
    {
        _toShow = CubeGenerator.GenerateCube(_boxSize * Mathf.Pow(_sizeChangePerIteration, 0), 3, new Vector3());
        PrepareAnchors();

        List<OperationSymbols> instructions = InstructionGenerator.GenerateInstructions(2);

        foreach (OperationSymbols instruction in instructions)
        {
            AddCubeAtAnchor(instruction);
        }

        GetComponent<MeshFilter>().mesh = _toShow;
    }

    public void SetUpConnections(GameObject[] roomsToConnectTo)
    {
        print("miau");
    }

    private void AddCubeAtAnchor(OperationSymbols position)
    {
        Tuple<Vector3, int> anchorPos = GetAnchor(position);

        Vector3 newCubeSize = new Vector3(_boxSize, _boxSize, _boxSize) *
                              Mathf.Pow(_sizeChangePerIteration, anchorPos.Item2);

        Mesh secondMesh = CubeGenerator.GenerateCube(_boxSize * Mathf.Pow(_sizeChangePerIteration, anchorPos.Item2), 3, anchorPos.Item1);

        AddAnchor(position, anchorPos.Item1, newCubeSize, anchorPos.Item2 + 1);
        AddShownMesh(ref secondMesh, new Bounds(anchorPos.Item1, newCubeSize * 2 * 0.99f));
    }

    private Tuple<Vector3, int> GetAnchor(OperationSymbols position)
    {
        int anchorIndex = Random.Range(0, _anchorPoints[position].Count);
        Tuple<Vector3, int> anchorPos = _anchorPoints[position][anchorIndex];
        _anchorPoints[position].RemoveAt(anchorIndex);

        return anchorPos;
    }

    private void AddAnchor(OperationSymbols anchorPosition, Vector3 center, Vector3 size, int depth)
    {
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(size.x, size.y, 0), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(size.x, -size.y, 0), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(-size.x, size.y, 0), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(-size.x, -size.y, 0), depth));

        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(0, size.y, size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(0, size.y, -size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(0, -size.y, size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(0, -size.y, -size.z), depth));

        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(size.x, 0, size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(size.x, 0, -size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(-size.x, 0, size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(-size.x, 0, -size.z), depth));

        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(size.x, size.y, size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(size.x, size.y, -size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(size.x, -size.y, size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(size.x, -size.y, -size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(-size.x, size.y, size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(-size.x, size.y, -size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(-size.x, -size.y, size.z), depth));
        _anchorPoints[anchorPosition].Add(new Tuple<Vector3, int>(center + new Vector3(-size.x, -size.y, -size.z), depth));

        RemoveOverlappingAnchors(anchorPosition);

        switch (anchorPosition)
        {
            case OperationSymbols.XY:
                break;
            case OperationSymbols.XMY:
                break;
            case OperationSymbols.MXY:
                break;
            case OperationSymbols.MXMY:
                break;
            case OperationSymbols.YZ:
                break;
            case OperationSymbols.YMZ:
                break;
            case OperationSymbols.MYZ:
                break;
            case OperationSymbols.MYMZ:
                break;
            case OperationSymbols.ZX:
                break;
            case OperationSymbols.ZMX:
                break;
            case OperationSymbols.MZX:
                break;
            case OperationSymbols.MZMX:
                break;
            case OperationSymbols.XYZ:
                break;
            case OperationSymbols.XYMZ:
                break;
            case OperationSymbols.XMYZ:
                break;
            case OperationSymbols.XMYMZ:
                break;
            case OperationSymbols.MXYZ:
                break;
            case OperationSymbols.MXYMZ:
                break;
            case OperationSymbols.MXMYZ:
                break;
            case OperationSymbols.MXMYMZ:
                break;
        }
    }

    private void RemoveOverlappingAnchors(OperationSymbols anchorPosition)
    {
        List<Tuple<Vector3, int>> overlappingAnchors = new List<Tuple<Vector3, int>>();

        for (int i = 0; i < _anchorPoints[anchorPosition].Count; i++)
        {
            foreach (Bounds bounds in _RectsInMesh)
            {
                if (bounds.Contains(_anchorPoints[anchorPosition][i].Item1))
                {
                    overlappingAnchors.Add(_anchorPoints[anchorPosition][i]);
                }
            }
        }

        foreach (Tuple<Vector3, int> overlappingAnchor in overlappingAnchors)
        {
            _anchorPoints[anchorPosition].Remove(overlappingAnchor);
        }
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
    

    void PrepareAnchors()
    {
        for (int i = 0; i < Enum.GetValues(typeof(OperationSymbols)).Length; i++)
        {
            _anchorPoints.Add((OperationSymbols)i, new List<Tuple<Vector3, int>>());
        }

        _anchorPoints[OperationSymbols.XY].Add(new Tuple<Vector3, int>(new Vector3(_boxSize, _boxSize, 0), 1));
        _anchorPoints[OperationSymbols.XMY].Add(new Tuple<Vector3, int>(new Vector3(_boxSize, -_boxSize, 0), 1));
        _anchorPoints[OperationSymbols.MXY].Add(new Tuple<Vector3, int>(new Vector3(-_boxSize, _boxSize, 0), 1));
        _anchorPoints[OperationSymbols.MXMY].Add(new Tuple<Vector3, int>(new Vector3(-_boxSize, -_boxSize, 0), 1));

        _anchorPoints[OperationSymbols.YZ].Add(new Tuple<Vector3, int>(new Vector3(0, _boxSize, _boxSize), 1));
        _anchorPoints[OperationSymbols.YMZ].Add(new Tuple<Vector3, int>(new Vector3(0, _boxSize, -_boxSize), 1));
        _anchorPoints[OperationSymbols.MYZ].Add(new Tuple<Vector3, int>(new Vector3(0, -_boxSize, _boxSize), 1));
        _anchorPoints[OperationSymbols.MYMZ].Add(new Tuple<Vector3, int>(new Vector3(0, -_boxSize, -_boxSize), 1));

        _anchorPoints[OperationSymbols.ZX].Add(new Tuple<Vector3, int>(new Vector3(_boxSize, 0, _boxSize), 1));
        _anchorPoints[OperationSymbols.ZMX].Add(new Tuple<Vector3, int>(new Vector3(-_boxSize, 0, _boxSize), 1));
        _anchorPoints[OperationSymbols.MZX].Add(new Tuple<Vector3, int>(new Vector3(_boxSize, 0, -_boxSize), 1));
        _anchorPoints[OperationSymbols.MZMX].Add(new Tuple<Vector3, int>(new Vector3(-_boxSize, 0, -_boxSize), 1));

        _anchorPoints[OperationSymbols.XYZ].Add(new Tuple<Vector3, int>(new Vector3(_boxSize, _boxSize, _boxSize), 1));
        _anchorPoints[OperationSymbols.XYMZ].Add(new Tuple<Vector3, int>(new Vector3(_boxSize, _boxSize, -_boxSize), 1));
        _anchorPoints[OperationSymbols.XMYZ].Add(new Tuple<Vector3, int>(new Vector3(_boxSize, -_boxSize, _boxSize), 1));
        _anchorPoints[OperationSymbols.XMYMZ].Add(new Tuple<Vector3, int>(new Vector3(_boxSize, -_boxSize, -_boxSize), 1));
        _anchorPoints[OperationSymbols.MXYZ].Add(new Tuple<Vector3, int>(new Vector3(-_boxSize, _boxSize, _boxSize), 1));
        _anchorPoints[OperationSymbols.MXYMZ].Add(new Tuple<Vector3, int>(new Vector3(-_boxSize, _boxSize, -_boxSize), 1));
        _anchorPoints[OperationSymbols.MXMYZ].Add(new Tuple<Vector3, int>(new Vector3(-_boxSize, -_boxSize, _boxSize), 1));
        _anchorPoints[OperationSymbols.MXMYMZ].Add(new Tuple<Vector3, int>(new Vector3(-_boxSize, -_boxSize, -_boxSize), 1));
    }

}
