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
    [SerializeField] private bool _mirrorXYPlane = true;
    [SerializeField] private bool _mirrorYZPlane = true;
    [SerializeField] private bool _mirrorXZPlane = true;

    private Dictionary<OperationSymbols, List<Tuple<Vector3, int>>> _anchorPoints = new Dictionary<OperationSymbols, List<Tuple<Vector3, int>>>();

    private Mesh _toShow;
    private List<Bounds> _RectsInMesh = new List<Bounds>();

    public void CreateRoom(DungeonRoom dungeonRoom) 
    {
        _toShow = CubeGenerator.GenerateCube(_boxSize, 3, new Vector3());
        _RectsInMesh.Add(new Bounds(Vector3.zero, Vector3.one * _boxSize * 2 * 0.99f));

        PrepareAnchors();

        List<OperationSymbols> instructions = InstructionGenerator.GenerateInstructions(1);

        foreach (OperationSymbols instruction in instructions)
        {
            AddCubeAtAnchor(instruction);
        }

        MakeDoubleSided();

        GetComponent<MeshFilter>().mesh = _toShow;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        foreach (KeyValuePair<OperationSymbols, List<Tuple<Vector3, int>>> anchorPoint in _anchorPoints)
        {
            foreach (Tuple<Vector3, int> point in anchorPoint.Value)
            {
                Gizmos.DrawSphere(point.Item1 + transform.position, 0.2f);
            }
        }
    }

    private void MakeDoubleSided()
    {
        List<Vector3> originalVerts = new List<Vector3>(_toShow.vertices);
        List<Vector3> originalNorms = new List<Vector3>(_toShow.normals);
        List<int> originalTris = new List<int>(_toShow.triangles);

        List<Vector3> copiedVerts = new List<Vector3>(_toShow.vertices);
        List<Vector3> copiedNorms = new List<Vector3>(_toShow.normals);
        List<int> copiedTris = new List<int>(_toShow.triangles);

        for (int i = 0; i < copiedNorms.Count; i++)
        {
            copiedNorms[i] = originalNorms[i] * -1;
        }

        for (int i = 0; i < copiedTris.Count; i++)
        {
            copiedTris[i] += originalVerts.Count;
        }

        for (int i = 0; i < copiedTris.Count; i+=3)
        {
            int temp = copiedTris[i];
            copiedTris[i] = copiedTris[i + 2];
            copiedTris[i + 2] = temp;
        }

        originalVerts.AddRange(copiedVerts);
        originalNorms.AddRange(copiedNorms);
        originalTris.AddRange(copiedTris);

        _toShow.vertices = originalVerts.ToArray();
        _toShow.normals = originalNorms.ToArray();
        _toShow.triangles = originalTris.ToArray();
    }

    public void SetUpConnections(GameObject[] roomsToConnectTo)
    {
        print("miau");
    }

    private void AddCubeAtAnchor(OperationSymbols position)
    {
        Tuple<Vector3, int> anchorPos = GetAnchor(position);

        List<OperationSymbols> operationPositions = new List<OperationSymbols>();
        operationPositions.Add(position);


        Vector3 newCubeSize = new Vector3(_boxSize, _boxSize, _boxSize) *
                              Mathf.Pow(_sizeChangePerIteration, anchorPos.Item2);

        Mesh secondMesh = CubeGenerator.GenerateCube(_boxSize * Mathf.Pow(_sizeChangePerIteration, anchorPos.Item2), 3, anchorPos.Item1);

        AddAnchor(position, anchorPos.Item1, newCubeSize, anchorPos.Item2 + 1);
        AddShownMesh(ref secondMesh, new Bounds(anchorPos.Item1, newCubeSize * 2 * 0.99f));

        List<Tuple<Vector3, int>> xMirrorAnchorPos;
        List<Tuple<Vector3, int>> yMirrorAnchorPos;
        List<Tuple<Vector3, int>> zMirrorAnchorPos;

        int nextOperation = 1;
        if (_mirrorXYPlane)
        {
            xMirrorAnchorPos = GetXYMirroredAnchor(ref operationPositions);

            foreach (Tuple<Vector3, int> xMPos in xMirrorAnchorPos)
            {
                newCubeSize = new Vector3(_boxSize, _boxSize, _boxSize) *
                              Mathf.Pow(_sizeChangePerIteration, xMPos.Item2);
                secondMesh = CubeGenerator.GenerateCube(_boxSize * Mathf.Pow(_sizeChangePerIteration, xMPos.Item2), 3, xMPos.Item1);

                AddAnchor(operationPositions[nextOperation++], xMPos.Item1, newCubeSize, xMPos.Item2 + 1);
                AddShownMesh(ref secondMesh, new Bounds(xMPos.Item1, newCubeSize * 2 * 0.99f));
            }
        }
        if (_mirrorXZPlane)
        {
            yMirrorAnchorPos = GetXZMirroredAnchor(ref operationPositions);

            foreach (Tuple<Vector3, int> yMPos in yMirrorAnchorPos)
            {
                newCubeSize = new Vector3(_boxSize, _boxSize, _boxSize) *
                              Mathf.Pow(_sizeChangePerIteration, yMPos.Item2);
                secondMesh = CubeGenerator.GenerateCube(_boxSize * Mathf.Pow(_sizeChangePerIteration, yMPos.Item2), 3, yMPos.Item1);

                AddAnchor(operationPositions[nextOperation++], yMPos.Item1, newCubeSize, yMPos.Item2 + 1);
                AddShownMesh(ref secondMesh, new Bounds(yMPos.Item1, newCubeSize * 2 * 0.99f));
            }
        }
        if (_mirrorYZPlane)
        {
            zMirrorAnchorPos = GetYZMirroredAnchor(ref operationPositions);

            foreach (Tuple<Vector3, int> zMPos in zMirrorAnchorPos)
            {
                newCubeSize = new Vector3(_boxSize, _boxSize, _boxSize) *
                              Mathf.Pow(_sizeChangePerIteration, zMPos.Item2);
                secondMesh = CubeGenerator.GenerateCube(_boxSize * Mathf.Pow(_sizeChangePerIteration, zMPos.Item2), 3, zMPos.Item1);

                AddAnchor(operationPositions[nextOperation++], zMPos.Item1, newCubeSize, zMPos.Item2 + 1);
                AddShownMesh(ref secondMesh, new Bounds(zMPos.Item1, newCubeSize * 2 * 0.99f));
            }
        }
    }

    #region mirrorMethods

    private List<Tuple<Vector3, int>> GetXYMirroredAnchor(ref List<OperationSymbols> positionToMirror)
    {
        List<Tuple<Vector3, int>> mirroredPositions = new List<Tuple<Vector3, int>>();
        List<OperationSymbols> newOperations = new List<OperationSymbols>();

        foreach (OperationSymbols toMirror in positionToMirror)
        {
            bool canMirror = true;
            OperationSymbols mirroredPos = toMirror;
            switch (toMirror)
            {
                case OperationSymbols.XY:
                case OperationSymbols.XMY:
                case OperationSymbols.MXY:
                case OperationSymbols.MXMY:
                    canMirror = false;
                    break;
                case OperationSymbols.YZ:
                    mirroredPos = OperationSymbols.YMZ;
                    break;
                case OperationSymbols.YMZ:
                    mirroredPos = OperationSymbols.YZ;
                    break;
                case OperationSymbols.MYZ:
                    mirroredPos = OperationSymbols.MYMZ;
                    break;
                case OperationSymbols.MYMZ:
                    mirroredPos = OperationSymbols.MYZ;
                    break;
                case OperationSymbols.ZX:
                    mirroredPos = OperationSymbols.MZX;
                    break;
                case OperationSymbols.ZMX:
                    mirroredPos = OperationSymbols.MZMX;
                    break;
                case OperationSymbols.MZX:
                    mirroredPos = OperationSymbols.ZX;
                    break;
                case OperationSymbols.MZMX:
                    mirroredPos = OperationSymbols.ZMX;
                    break;
                case OperationSymbols.XYZ:
                    mirroredPos = OperationSymbols.XYMZ;
                    break;
                case OperationSymbols.XYMZ:
                    mirroredPos = OperationSymbols.XYZ;
                    break;
                case OperationSymbols.XMYZ:
                    mirroredPos = OperationSymbols.XMYMZ;
                    break;
                case OperationSymbols.XMYMZ:
                    mirroredPos = OperationSymbols.XMYZ;
                    break;
                case OperationSymbols.MXYZ:
                    mirroredPos = OperationSymbols.MXYMZ;
                    break;
                case OperationSymbols.MXYMZ:
                    mirroredPos = OperationSymbols.MXYZ;
                    break;
                case OperationSymbols.MXMYZ:
                    mirroredPos = OperationSymbols.MXMYMZ;
                    break;
                case OperationSymbols.MXMYMZ:
                    mirroredPos = OperationSymbols.MXMYZ;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(positionToMirror), positionToMirror, null);
            }

            if (canMirror)
            {
                mirroredPositions.Add(GetAnchor(mirroredPos));
                newOperations.Add(mirroredPos);
            }
        }

        positionToMirror.AddRange(newOperations);
        return mirroredPositions;
    }

    private List<Tuple<Vector3, int>> GetXZMirroredAnchor(ref List<OperationSymbols> positionToMirror)
    {
        List<Tuple<Vector3, int>> mirroredPositions = new List<Tuple<Vector3, int>>();
        List<OperationSymbols> newOperations = new List<OperationSymbols>();

        foreach (OperationSymbols toMirror in positionToMirror)
        {
            bool canMirror = true;
            OperationSymbols mirroredPos = toMirror;
            switch (toMirror)
            {
                case OperationSymbols.XY:
                    mirroredPos = OperationSymbols.XMY;
                    break;
                case OperationSymbols.XMY:
                    mirroredPos = OperationSymbols.XY;
                    break;
                case OperationSymbols.MXY:
                    mirroredPos = OperationSymbols.MXMY;
                    break;
                case OperationSymbols.MXMY:
                    mirroredPos = OperationSymbols.MXY;
                    break;
                case OperationSymbols.YZ:
                    mirroredPos = OperationSymbols.MYZ;
                    break;
                case OperationSymbols.YMZ:
                    mirroredPos = OperationSymbols.MYMZ;
                    break;
                case OperationSymbols.MYZ:
                    mirroredPos = OperationSymbols.YZ;
                    break;
                case OperationSymbols.MYMZ:
                    mirroredPos = OperationSymbols.YMZ;
                    break;
                case OperationSymbols.ZX:
                case OperationSymbols.ZMX:
                case OperationSymbols.MZX:
                case OperationSymbols.MZMX:
                    canMirror = false;
                    break;
                case OperationSymbols.XYZ:
                    mirroredPos = OperationSymbols.XMYZ;
                    break;
                case OperationSymbols.XYMZ:
                    mirroredPos = OperationSymbols.XMYMZ;
                    break;
                case OperationSymbols.XMYZ:
                    mirroredPos = OperationSymbols.XYZ;
                    break;
                case OperationSymbols.XMYMZ:
                    mirroredPos = OperationSymbols.XYMZ;
                    break;
                case OperationSymbols.MXYZ:
                    mirroredPos = OperationSymbols.MXMYZ;
                    break;
                case OperationSymbols.MXYMZ:
                    mirroredPos = OperationSymbols.MXMYMZ;
                    break;
                case OperationSymbols.MXMYZ:
                    mirroredPos = OperationSymbols.MXYZ;
                    break;
                case OperationSymbols.MXMYMZ:
                    mirroredPos = OperationSymbols.MXYMZ;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(positionToMirror), positionToMirror, null);
            }

            if (canMirror)
            {
                mirroredPositions.Add(GetAnchor(mirroredPos));
                newOperations.Add(mirroredPos);
            }
        }

        positionToMirror.AddRange(newOperations);
        return mirroredPositions;
    }

    private List<Tuple<Vector3, int>> GetYZMirroredAnchor(ref List<OperationSymbols> positionToMirror)
    {
        List<Tuple<Vector3, int>> mirroredPositions = new List<Tuple<Vector3, int>>();
        List<OperationSymbols> newOperations = new List<OperationSymbols>();

        foreach (OperationSymbols toMirror in positionToMirror)
        {
            bool canMirror = true;
            OperationSymbols mirroredPos = toMirror;
            switch (toMirror)
            {
                case OperationSymbols.XY:
                    mirroredPos = OperationSymbols.MXY;
                    break;
                case OperationSymbols.XMY:
                    mirroredPos = OperationSymbols.MXMY;
                    break;
                case OperationSymbols.MXY:
                    mirroredPos = OperationSymbols.XY;
                    break;
                case OperationSymbols.MXMY:
                    mirroredPos = OperationSymbols.XMY;
                    break;
                case OperationSymbols.YZ:
                case OperationSymbols.YMZ:
                case OperationSymbols.MYZ:
                case OperationSymbols.MYMZ:
                    canMirror = false;
                    break;
                case OperationSymbols.ZX:
                    mirroredPos = OperationSymbols.ZMX;
                    break;
                case OperationSymbols.ZMX:
                    mirroredPos = OperationSymbols.ZX;
                    break;
                case OperationSymbols.MZX:
                    mirroredPos = OperationSymbols.MZMX;
                    break;
                case OperationSymbols.MZMX:
                    mirroredPos = OperationSymbols.MZX;
                    break;
                case OperationSymbols.XYZ:
                    mirroredPos = OperationSymbols.MXYZ;
                    break;
                case OperationSymbols.XYMZ:
                    mirroredPos = OperationSymbols.MXYMZ;
                    break;
                case OperationSymbols.XMYZ:
                    mirroredPos = OperationSymbols.MXMYZ;
                    break;
                case OperationSymbols.XMYMZ:
                    mirroredPos = OperationSymbols.MXMYMZ;
                    break;
                case OperationSymbols.MXYZ:
                    mirroredPos = OperationSymbols.XYZ;
                    break;
                case OperationSymbols.MXYMZ:
                    mirroredPos = OperationSymbols.XYMZ;
                    break;
                case OperationSymbols.MXMYZ:
                    mirroredPos = OperationSymbols.XMYZ;
                    break;
                case OperationSymbols.MXMYMZ:
                    mirroredPos = OperationSymbols.XMYMZ;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(positionToMirror), positionToMirror, null);
            }

            if (canMirror)
            {
                mirroredPositions.Add(GetAnchor(mirroredPos));
                newOperations.Add(mirroredPos);
            }
        }

        positionToMirror.AddRange(newOperations);
        return mirroredPositions;
    }



    #endregion

    private Tuple<Vector3, int> GetAnchor(OperationSymbols position)
    {
        int anchorIndex = Random.Range(0, _anchorPoints[position].Count);
        Tuple<Vector3, int> anchorPos = _anchorPoints[position][anchorIndex];
        _anchorPoints[position].RemoveAt(anchorIndex);

        return anchorPos;
    }

    private void AddAnchor(OperationSymbols anchorPosition, Vector3 center, Vector3 size, int depth)
    {
        int anchorPointsBeforeAdd = _anchorPoints[anchorPosition].Count;
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

        RemoveOverlappingAnchors(anchorPosition, anchorPointsBeforeAdd);
    }

    private void RemoveOverlappingAnchors(OperationSymbols anchorPosition, int numberOfAnchorsNotToCheck)
    {
        List<Tuple<Vector3, int>> overlappingAnchors = new List<Tuple<Vector3, int>>();

        for (int i = numberOfAnchorsNotToCheck; i < _anchorPoints[anchorPosition].Count; i++)
        {
            foreach (Bounds bounds in _RectsInMesh)
            {
                if (bounds.Contains(_anchorPoints[anchorPosition][i].Item1))
                {
                    overlappingAnchors.Add(_anchorPoints[anchorPosition][i]);
                }
                else if(bounds.SqrDistance(_anchorPoints[anchorPosition][i].Item1) < 0.01f)
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
        List<Vector3> firstMeshNorms = new List<Vector3>(_toShow.normals);
        List<Vector3> secondMeshNorms = new List<Vector3>(secondMesh.normals);
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
                RemoveTrianglesWithIndex(ref firstMeshTris, indexesOfOverlappingVerts[0]);
            }
            else
            {
                indexesOfOverlappingVerts.RemoveAt(0);
            }
        }

        foreach (Bounds bounds in _RectsInMesh)
        {
            if (!bounds.Intersects(newSquareBounds))
                continue;

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
                    RemoveTrianglesWithIndex(ref secondMeshTris, indexesOfOverlappingVerts[0]);
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
        firstMeshNorms.AddRange(secondMeshNorms);
        firstMeshTris.AddRange(secondMeshTris);

        _toShow.vertices = firstMeshVerts.ToArray();
        _toShow.normals = firstMeshNorms.ToArray();
        _toShow.triangles = firstMeshTris.ToArray();
        _RectsInMesh.Add(newSquareBounds);
    }

    private int GetClosestVertexInMesh(Vector3 vertex, List<Vector3> mesh)
    {
        float minDistanceSqr = float.MaxValue;
        int closestVertIndex = -1;

        for (int i = 0; i < mesh.Count; i++)
        {
            if (vertex == mesh[i])
                continue;
            float newSqrMag = (vertex - mesh[i]).sqrMagnitude;
            if (newSqrMag < minDistanceSqr)
            {
                closestVertIndex = i;
                minDistanceSqr = newSqrMag;
            }
        }

        return closestVertIndex;
    }

    void RemoveTrianglesWithIndex(ref List<int> tris, int index)
    {
        int foundPos = tris.FindIndex((int i) => { return index == i; });

        switch (foundPos % 3)
        {
            case 0:
                tris.RemoveAt(foundPos);
                tris.RemoveAt(foundPos);
                tris.RemoveAt(foundPos);
                break;
            case 1:
                tris.RemoveAt(foundPos - 1);
                tris.RemoveAt(foundPos - 1);
                tris.RemoveAt(foundPos - 1);
                break;
            case 2:
                tris.RemoveAt(foundPos - 2);
                tris.RemoveAt(foundPos - 2);
                tris.RemoveAt(foundPos - 2);
                break;
        }
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
