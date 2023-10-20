using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;


struct coordsGameObject {
    public int x;
    public int z;
    public GameObject gameObject;
}

public class ChunkGenerator : MonoBehaviour {

    private List<coordsGameObject> chunks;
    private Vector3Int playerChunk;
    
    [SerializeField]
    int chunkSize = 256;
    [SerializeField]
    int chunckAmplitude = 20;
    [SerializeField]
    Material defaultMaterial;
    public Prefabs prefabs;

    private Transform cameraTransform;


    void Start() {
        playerChunk = new Vector3Int();
        chunks = new List<coordsGameObject>();

        cameraTransform = GameObject.Find("Main Camera").transform;
    }

    void Update() {

        findChunk();
        loadChunks();
    }
     
    private void loadChunks() {
        bool chunk1 = false, chunk2 = false, chunk3 = false, chunk4 = false, chunk5 = false, chunk6 = false, chunk7 = false, chunk8 = false, chunk9 = false;

        for (int i = 0; i < chunks.Count; i++) {
            int xChunk = chunks[i].x;
            int zChunk = chunks[i].z;
            if (xChunk == playerChunk.x - 1 && zChunk == playerChunk.z + 1) chunk7 = true;
            if (xChunk == playerChunk.x     && zChunk == playerChunk.z + 1) chunk8 = true;
            if (xChunk == playerChunk.x + 1 && zChunk == playerChunk.z + 1) chunk9 = true;
            if (xChunk == playerChunk.x - 1 && zChunk == playerChunk.z)     chunk4 = true;
            if (xChunk == playerChunk.x     && zChunk == playerChunk.z)     chunk5 = true;
            if (xChunk == playerChunk.x + 1 && zChunk == playerChunk.z)     chunk6 = true;
            if (xChunk == playerChunk.x - 1 && zChunk == playerChunk.z - 1) chunk1 = true;
            if (xChunk == playerChunk.x     && zChunk == playerChunk.z - 1) chunk2 = true;
            if (xChunk == playerChunk.x + 1 && zChunk == playerChunk.z - 1) chunk3 = true;
        }

        /*
         Hay que implementar funciones que seteen los vecinos para calcular las normales
         */
        if (!chunk5) createChunk(new Vector3Int(playerChunk.x,       playerChunk.y,      playerChunk.z));

        if (!chunk8) createChunk(new Vector3Int(playerChunk.x,       playerChunk.y,      playerChunk.z + 1));
        if (!chunk6) createChunk(new Vector3Int(playerChunk.x + 1,   playerChunk.y,      playerChunk.z));
        if (!chunk4) createChunk(new Vector3Int(playerChunk.x - 1,   playerChunk.y,      playerChunk.z));
        if (!chunk2) createChunk(new Vector3Int(playerChunk.x,       playerChunk.y,      playerChunk.z - 1));

        if (!chunk9) createChunk(new Vector3Int(playerChunk.x + 1,   playerChunk.y, playerChunk.z + 1));
        if (!chunk7) createChunk(new Vector3Int(playerChunk.x - 1,   playerChunk.y, playerChunk.z + 1));
        if (!chunk3) createChunk(new Vector3Int(playerChunk.x + 1,   playerChunk.y, playerChunk.z - 1));
        if (!chunk1) createChunk(new Vector3Int(playerChunk.x - 1,   playerChunk.y, playerChunk.z - 1));
    }


    private Chunk createChunk(Vector3Int newChunkKeyInt) {
        //armamos el struct
        coordsGameObject newChunk = new coordsGameObject();

        //armamos el GameObject
        GameObject newChunkGob = new GameObject($"{newChunkKeyInt.x}~{newChunkKeyInt.z}");
        newChunkGob.transform.position = new Vector3(chunkSize * newChunkKeyInt.x, 0f, chunkSize * newChunkKeyInt.z);
        Chunk chunk = newChunkGob.AddComponent<Chunk>();

        chunk.init(chunkSize, chunckAmplitude, defaultMaterial);
        
        for (int i = 0; i < chunks.Count; i++) {
            int xChunk = chunks[i].x;
            int zChunk = chunks[i].z;
            if (xChunk == newChunkKeyInt.x - 1 && zChunk == newChunkKeyInt.z + 1) {
                chunk.setNeighbor(7, chunks[i].gameObject.GetComponent<Chunk>().getSide(3));
            }
            if (xChunk == newChunkKeyInt.x && zChunk == newChunkKeyInt.z + 1) {
                chunk.setNeighbor(8, chunks[i].gameObject.GetComponent<Chunk>().getSide(2));
            }
            if (xChunk == newChunkKeyInt.x + 1 && zChunk == newChunkKeyInt.z + 1) {
                chunk.setNeighbor(9, chunks[i].gameObject.GetComponent<Chunk>().getSide(1));
            }
            if (xChunk == newChunkKeyInt.x - 1 && zChunk == newChunkKeyInt.z) {
                chunk.setNeighbor(4, chunks[i].gameObject.GetComponent<Chunk>().getSide(6));
            }
            //if (xChunk == newChunkKeyInt.x && zChunk == newChunkKeyInt.z) {
            //    chunk.setNeighbor(5, chunks[i].gameObject.GetComponent<Chunk>().getSide(5));
            //}
            if (xChunk == newChunkKeyInt.x + 1 && zChunk == newChunkKeyInt.z) {
                chunk.setNeighbor(6, chunks[i].gameObject.GetComponent<Chunk>().getSide(4));
            }
            if (xChunk == newChunkKeyInt.x - 1 && zChunk == newChunkKeyInt.z - 1) {
                chunk.setNeighbor(1, chunks[i].gameObject.GetComponent<Chunk>().getSide(9));
            }
            if (xChunk == newChunkKeyInt.x && zChunk == newChunkKeyInt.z - 1) {
                chunk.setNeighbor(2, chunks[i].gameObject.GetComponent<Chunk>().getSide(8));
            }
            if (xChunk == newChunkKeyInt.x + 1 && zChunk == newChunkKeyInt.z - 1) {
                chunk.setNeighbor(3, chunks[i].gameObject.GetComponent<Chunk>().getSide(7));
            }
        }
        
        chunk.createChunk(ref prefabs);
        //seteamos
        newChunk.gameObject = newChunkGob;
        newChunk.x = newChunkKeyInt.x;
        newChunk.z = newChunkKeyInt.z;
        //pusheamos
        chunks.Add(newChunk);
        return chunk;
    }

    private void findChunk() {
        playerChunk.x = Mathf.FloorToInt(cameraTransform.position.x / chunkSize);
        playerChunk.z = Mathf.FloorToInt(cameraTransform.position.z / chunkSize);
    }

}

