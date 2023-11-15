using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkData", menuName = "ScriptableObjects/ChunkParameters", order = 1)]
public class ChunkParameters : ScriptableObject {
    
    [SerializeField]
    public int size = 256;
    [SerializeField]
    public int amplitude = 20;
    [SerializeField]
    public float spawnDelay = 0.01f;
    [SerializeField]
    public int succesiveDivisions = 4;
    [SerializeField]
    public float persistance = 1.8f;
    [SerializeField]
    public int chunksLoaded = 4;
    [SerializeField]
    public int amountOfTrees = 7;
    [SerializeField]
    public Gradient gradientNormals1;
    [SerializeField]
    public Gradient gradientNormals2;
    [SerializeField]
    public Gradient gradientHeight1;
    [SerializeField]
    public Gradient gradientGrayScale;
}
