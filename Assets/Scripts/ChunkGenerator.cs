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
    private float spawnTimer=0f;

    [SerializeField]
    ChunkParameters chunkParameters;
    [SerializeField]
    float debug = 2f;
    [SerializeField]
    Material defaultMaterial;
    public Prefabs prefabs;



    private Transform cameraTransform;


    void Start() {
        playerChunk = new Vector3Int();
        chunks = new List<coordsGameObject>();

        cameraTransform = GameObject.Find("Main Camera").transform;
        loadChunks(false);
    }

    void Update() {
        findChunk();
        loadChunks(true);
    }

    private void loadChunks(bool delayed) {
        spawnTimer -= Time.deltaTime;
        for (int i = -chunkParameters.chunksLoaded; i <= chunkParameters.chunksLoaded; i++) {       //esto es porque se asume que todos tienen que ser spawneados
            for (int j = -chunkParameters.chunksLoaded; j <= chunkParameters.chunksLoaded; j++) {   //a menos que se diga lo contrario abajo
                bool flag = true;
                for (int k = 0; k < chunks.Count; k++) {
                    if((chunks[k].x == i + playerChunk.x) && (chunks[k].z == j + playerChunk.z)) {
                        flag = false;
                    }
                }

                if (flag && spawnTimer < 0f) {
                    createChunk(new Vector3Int(playerChunk.x + i, playerChunk.y, playerChunk.z + j));
                    if(delayed) spawnTimer = chunkParameters.spawnDelay;
                }
            }
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

