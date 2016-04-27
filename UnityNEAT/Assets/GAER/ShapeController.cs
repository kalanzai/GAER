﻿using UnityEngine;
using System.Collections;
using SharpNeat.Phenomes;
using System;
using MarchingCubesProject;

public class ShapeController : UnitController
{
    public Material m_material;
    GameObject m_mesh;
    private static readonly int Width = SimpleExperiment.Width;
    private static readonly int Height = SimpleExperiment.Height;
    private static readonly int Length = SimpleExperiment.Length;
    private readonly float[,,] _voxels = new float[Width, Height, Length];
    private int _numVoxels;
    // Use this for initialization
    void Start()
    {
        MarchingCubes.SetTarget(0.5f);
        MarchingCubes.SetWindingOrder(0,1,2);
        MarchingCubes.SetModeToCubes();
    }

    void FixedUpdate() {    }

    public override void Activate(IBlackBox box)
    {
        _numVoxels = 0;
        for (int x = 1; x < Width-1; x++)
        {
            for (int y = 1; y < Height-1; y++)
            {
                for (int z = 1; z < Length-1; z++)
                {
                    box.ResetState();
                    box.InputSignalArray[0] = x;
                    box.InputSignalArray[1] = y;
                    box.InputSignalArray[2] = z;
                    box.Activate();
                    _voxels[x, y, z] = (float)box.OutputSignalArray[0];
                    if (_voxels[x, y, z] > 0.5f)
                        _numVoxels++;
                }
            }
        }

        Mesh mesh = MarchingCubes.CreateMesh(_voxels);

        mesh.uv = new Vector2[mesh.vertices.Length];
        mesh.RecalculateNormals();

        m_mesh = new GameObject("Mesh");
        m_mesh.AddComponent<MeshFilter>();
        m_mesh.AddComponent<MeshRenderer>();
        m_mesh.GetComponent<Renderer>().material = m_material;
        m_mesh.GetComponent<MeshFilter>().mesh = mesh;
        //Center mesh
        m_mesh.transform.position = transform.position;
    }

    public override float GetFitness()
    {
        int target = Length * Height * Width / 3;
        var fit = -Math.Abs(_numVoxels - target) + Length*Height*Width;
        return fit;
    }

    public override void Stop()
    {
    }


}



