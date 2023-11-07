using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    ChunkParameters chunkParameters;
    [SerializeField]
    float debug = 2f;
    [SerializeField]
    float chunkGenerationDelay = 0.01f;
    [SerializeField]
    bool increasedChunkSpawn = false;
    [SerializeField]
    Material defaultMaterial;
    public Prefabs prefabs;



    private Transform cameraTransform;
    private float timerForChunksSpawn=0f;


    void Start() {
        playerChunk = new Vector3Int();
        chunks = new List<coordsGameObject>();

        cameraTransform = GameObject.Find("Main Camera").transform;
    }

    void Update() {
        timerForChunksSpawn -= Time.deltaTime;
        findChunk();
        loadChunks();
    }

    private void loadChunks() {
        bool chunk1 = false, chunk2 = false, chunk3 = false, chunk4 = false, chunk5 = false, chunk6 = false, chunk7 = false, chunk8 = false, chunk9 = false;
        bool chunk11 = false, chunk12 = false, chunk22 = false, chunk32 = false, chunk33 = false, chunk36 = false, chunk66 = false, chunk96 = false, chunk99 = false, chunk98 = false, chunk88 = false, chunk78 = false, chunk77 = false, chunk74 = false, chunk44 = false, chunk14 = false;

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

            if (increasedChunkSpawn) {
                if (xChunk == playerChunk.x - 2 && zChunk == playerChunk.z + 2) chunk77 = true;
                if (xChunk == playerChunk.x - 1 && zChunk == playerChunk.z + 2) chunk78 = true;
                if (xChunk == playerChunk.x - 0 && zChunk == playerChunk.z + 2) chunk88 = true;
                if (xChunk == playerChunk.x + 1 && zChunk == playerChunk.z + 2) chunk98 = true;
                if (xChunk == playerChunk.x + 2 && zChunk == playerChunk.z + 2) chunk99 = true;

                if (xChunk == playerChunk.x - 2 && zChunk == playerChunk.z - 2) chunk11 = true;
                if (xChunk == playerChunk.x - 1 && zChunk == playerChunk.z - 2) chunk12 = true;
                if (xChunk == playerChunk.x - 0 && zChunk == playerChunk.z - 2) chunk22 = true;
                if (xChunk == playerChunk.x + 1 && zChunk == playerChunk.z - 2) chunk32 = true;
                if (xChunk == playerChunk.x + 2 && zChunk == playerChunk.z - 2) chunk33 = true;

                if (xChunk == playerChunk.x - 2 && zChunk == playerChunk.z + 1) chunk74 = true;
                if (xChunk == playerChunk.x - 2 && zChunk == playerChunk.z + 0) chunk44 = true;
                if (xChunk == playerChunk.x - 2 && zChunk == playerChunk.z - 1) chunk14 = true;

                if (xChunk == playerChunk.x + 2 && zChunk == playerChunk.z + 1) chunk96 = true;
                if (xChunk == playerChunk.x + 2 && zChunk == playerChunk.z + 0) chunk66 = true;
                if (xChunk == playerChunk.x + 2 && zChunk == playerChunk.z - 1) chunk36 = true;
            }
        }

        /*
         Hay que implementar funciones que seteen los vecinos para calcular las normales
         */
        if (!chunk5 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x, playerChunk.y, playerChunk.z));
            timerForChunksSpawn = chunkGenerationDelay;
        }

        if (!chunk8 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x, playerChunk.y, playerChunk.z + 1));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk6 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x + 1, playerChunk.y, playerChunk.z));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk4 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x - 1, playerChunk.y, playerChunk.z));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk2 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x, playerChunk.y, playerChunk.z - 1));
            timerForChunksSpawn = chunkGenerationDelay;
        }

        if (!chunk9 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x + 1, playerChunk.y, playerChunk.z + 1));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk7 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x - 1, playerChunk.y, playerChunk.z + 1));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk3 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x + 1, playerChunk.y, playerChunk.z - 1));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk1 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x - 1, playerChunk.y, playerChunk.z - 1));
            timerForChunksSpawn = chunkGenerationDelay;
        }

        if (!increasedChunkSpawn) return;

        if (!chunk88 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x, playerChunk.y, playerChunk.z + 2));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk78 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x - 1, playerChunk.y, playerChunk.z + 2));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk98 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x + 1, playerChunk.y, playerChunk.z + 2));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk77 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x - 2, playerChunk.y, playerChunk.z + 2));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk99 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x + 2, playerChunk.y, playerChunk.z + 2));
            timerForChunksSpawn = chunkGenerationDelay;
        }

        if (!chunk22 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x, playerChunk.y, playerChunk.z - 2));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk12 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x - 1, playerChunk.y, playerChunk.z - 2));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk32 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x + 1, playerChunk.y, playerChunk.z - 2));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk11 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x - 2, playerChunk.y, playerChunk.z - 2));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk33 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x + 2, playerChunk.y, playerChunk.z - 2));
            timerForChunksSpawn = chunkGenerationDelay;
        }

        if (!chunk74 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x - 2, playerChunk.y, playerChunk.z + 1));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk44 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x - 2, playerChunk.y, playerChunk.z));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk14 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x - 2, playerChunk.y, playerChunk.z - 1));
            timerForChunksSpawn = chunkGenerationDelay;
        }

        if (!chunk96 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x + 2, playerChunk.y, playerChunk.z + 1));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk66 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x + 2, playerChunk.y, playerChunk.z));
            timerForChunksSpawn = chunkGenerationDelay;
        }
        if (!chunk36 && timerForChunksSpawn < 0f) {
            createChunk(new Vector3Int(playerChunk.x + 2, playerChunk.y, playerChunk.z - 1));
            timerForChunksSpawn = chunkGenerationDelay;
        }

    }


    private Chunk createChunk(Vector3Int newChunkKeyInt) {

        //armamos el GameObject
        GameObject newChunkGob = new GameObject($"{newChunkKeyInt.x}~{newChunkKeyInt.z}");
        newChunkGob.transform.position = new Vector3(chunkParameters.size * newChunkKeyInt.x, 0f, chunkParameters.size * newChunkKeyInt.z);
        Chunk chunk = newChunkGob.AddComponent<Chunk>();
        int[] neighborsForNormalsSharing = new int[10];

        chunk.init(chunkParameters, debug, defaultMaterial);
        
        for (int i = 0; i < chunks.Count; i++) {
            int xChunk = chunks[i].x;
            int zChunk = chunks[i].z;
            if (xChunk == newChunkKeyInt.x - 1 && zChunk == newChunkKeyInt.z + 1) {
                chunk.setNeighborHeights(7, chunks[i].gameObject.GetComponent<Chunk>().getSide(3));
                chunk.setNeighborChunk(7, chunks[i].gameObject.GetComponent<Chunk>());
            }
            if (xChunk == newChunkKeyInt.x  && zChunk == newChunkKeyInt.z + 1) {
                chunk.setNeighborHeights(8, chunks[i].gameObject.GetComponent<Chunk>().getSide(2));
                chunk.setNeighborChunk(8, chunks[i].gameObject.GetComponent<Chunk>());
            }
            if (xChunk == newChunkKeyInt.x + 1 && zChunk == newChunkKeyInt.z + 1) {
                chunk.setNeighborHeights(9, chunks[i].gameObject.GetComponent<Chunk>().getSide(1));
                chunk.setNeighborChunk(9, chunks[i].gameObject.GetComponent<Chunk>());
            }
            if (xChunk == newChunkKeyInt.x - 1 && zChunk == newChunkKeyInt.z) {
                chunk.setNeighborHeights(4, chunks[i].gameObject.GetComponent<Chunk>().getSide(6));
                chunk.setNeighborChunk(4, chunks[i].gameObject.GetComponent<Chunk>());
            }
            //5!
            if (xChunk == newChunkKeyInt.x + 1 && zChunk == newChunkKeyInt.z) {
                chunk.setNeighborHeights(6, chunks[i].gameObject.GetComponent<Chunk>().getSide(4));
                chunk.setNeighborChunk(6, chunks[i].gameObject.GetComponent<Chunk>());
            }
            if (xChunk == newChunkKeyInt.x - 1 && zChunk == newChunkKeyInt.z - 1) {
                chunk.setNeighborHeights(1, chunks[i].gameObject.GetComponent<Chunk>().getSide(9));
                chunk.setNeighborChunk(1, chunks[i].gameObject.GetComponent<Chunk>());
            }
            if (xChunk == newChunkKeyInt.x && zChunk == newChunkKeyInt.z - 1) {
                chunk.setNeighborHeights(2, chunks[i].gameObject.GetComponent<Chunk>().getSide(8));
                chunk.setNeighborChunk(2, chunks[i].gameObject.GetComponent<Chunk>());
            }
            if (xChunk == newChunkKeyInt.x + 1 && zChunk == newChunkKeyInt.z - 1) {
                chunk.setNeighborHeights(3, chunks[i].gameObject.GetComponent<Chunk>().getSide(7));
                chunk.setNeighborChunk(3, chunks[i].gameObject.GetComponent<Chunk>());
            }
        }
        
        chunk.createChunk(ref prefabs);


        //armamos el struct
        coordsGameObject newChunk = new coordsGameObject();
        //seteamos
        newChunk.gameObject = newChunkGob;
        newChunk.x = newChunkKeyInt.x;
        newChunk.z = newChunkKeyInt.z;
        //pusheamos
        chunks.Add(newChunk);
        return chunk;
    }

    private void findChunk() {
        playerChunk.x = Mathf.FloorToInt(cameraTransform.position.x / chunkParameters.size);
        playerChunk.z = Mathf.FloorToInt(cameraTransform.position.z / chunkParameters.size);
    }

}

