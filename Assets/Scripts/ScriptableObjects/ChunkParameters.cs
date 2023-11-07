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
    public int succesiveDivisions = 4;
    [SerializeField]
    public float persistance = 1.8f;
    [SerializeField]
    public int amountOfTrees = 7;
    [SerializeField]
    public Gradient gradient;
    [SerializeField]
    public Gradient gradientNormals;
}
