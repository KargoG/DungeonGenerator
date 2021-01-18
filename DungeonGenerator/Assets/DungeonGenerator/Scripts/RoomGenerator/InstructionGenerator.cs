using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum OperationSymbols
{
    XY,
    XMY,
    MXY,
    MXMY,
    YZ,
    YMZ,
    MYZ,
    MYMZ,
    ZX,
    ZMX,
    MZX,
    MZMX,
    XYZ,
    XYMZ,
    XMYZ,
    XMYMZ,
    MXYZ,
    MXYMZ,
    MXMYZ,
    MXMYMZ,
}

public class InstructionGenerator : ScriptableObject
{
    private static Dictionary<OperationSymbols, OperationSymbols[]> _reproductionRules;


    public static List<OperationSymbols> GenerateInstructions(int reproductionIterations)
    {
        List<OperationSymbols> instructionString = GenerateStartString(3);

        GenerateReproductionRules();

        for (int i = 0; i < reproductionIterations; i++)
        {
            RunPass(ref instructionString);
        }

        return instructionString;
    }

    private static void RunPass(ref List<OperationSymbols> instructionString)
    {
        List<OperationSymbols> newGeneration = new List<OperationSymbols>();

        foreach (OperationSymbols instruction in instructionString)
        {
            newGeneration.AddRange(_reproductionRules[instruction]);
        }

        instructionString = newGeneration;
    }

    private static List<OperationSymbols> GenerateStartString(int length)
    {
        List<OperationSymbols> startString = new List<OperationSymbols>();

        for (int i = 0; i < length; i++)
        {
            startString.Add((OperationSymbols)Enum.GetValues(typeof(OperationSymbols)).GetValue(Random.Range(0, Enum.GetValues(typeof(OperationSymbols)).Length)));
        }

        return startString;
    }

    private static void GenerateReproductionRules()
    {
        if (_reproductionRules != null)
            return;
        _reproductionRules = new Dictionary<OperationSymbols, OperationSymbols[]>();

        Array roomOperations = Enum.GetValues(typeof(OperationSymbols));
        for (int i = 0; i < roomOperations.Length; i++)
        {
            OperationSymbols[] operations = new OperationSymbols[2];
            for (int j = 0; j < operations.Length; j++)
            {
                OperationSymbols newOperation;
                do
                {
                    newOperation = (OperationSymbols)roomOperations.GetValue(Random.Range(0, roomOperations.Length));
                } while (newOperation == (OperationSymbols)roomOperations.GetValue(i));

                operations[j] = newOperation;
            }

            _reproductionRules.Add((OperationSymbols)roomOperations.GetValue(i), operations);
        }
    }
}
