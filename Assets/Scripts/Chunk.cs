using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    Mesh mesh;
    ChunkParameters chunkParameters;
    float maxHeight;
    float minHeight;
    Material defaultMaterial;
    Vector3[,] vertices;
    Vector3[] vertexLocalNormals;
    MeshRenderer meshRenderer;
    MeshCollider myMeshCollider;
    MeshFilter myMeshFilter;
    float[,,] neighborHeights;
    Chunk[] neighborChunks;
    int[] triadsForTriangles;
    int m;
    public Prefabs prefabs;

    public void init(ChunkParameters chunkParameters, float debug, Material defaultMaterial) {
        this.defaultMaterial = defaultMaterial;
        this.chunkParameters = chunkParameters;

        minHeight = 0f;
        maxHeight = chunkParameters.amplitude * 1.2f;

        neighborChunks = new Chunk[10];

        int succesivePow2 = Mathf.RoundToInt(Mathf.Pow(2, chunkParameters.succesiveDivisions));
        m = Mathf.RoundToInt(succesivePow2 + 1);
        CreateMatrix();
    }
    private void CreateMatrix() {
        vertices = new Vector3[m, m];
        vertexLocalNormals = new Vector3[vertices.Length];

        neighborHeights = new float[chunkParameters.succesiveDivisions + 1, m, m];

        for (int i = 0; i < m; i++) {
            for (int j = 0; j < m; j++) {
                vertices[i, j] = new Vector3(((float)j / (m - 1)) * chunkParameters.size, 0f, ((float)i / (m - 1)) * chunkParameters.size);
            }
        }
        for (int i = 0; i < m; i++) {
            for (int j = 0; j < m; j++) {
                for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                    neighborHeights[k, i, j] = 0f;
                }
            }
        }
    }

    public void createChunk(ref Prefabs prefabs) {
        this.prefabs = prefabs;
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        myMeshFilter = gameObject.AddComponent<MeshFilter>();
        myMeshCollider = gameObject.AddComponent<MeshCollider>();
        mesh = new Mesh();
        myMeshFilter.mesh = mesh;
        meshRenderer.material = defaultMaterial;

        CreateShape();
        UpdateMesh();
        SpawnTrees();
        myMeshCollider.sharedMesh = myMeshFilter.mesh;
    }


    private void CreateShape() {

        int waves = 0;
        while (waves < chunkParameters.succesiveDivisions + 1) {
            float[,] thisWave = new float[m, m];
            int waveIndex = Mathf.RoundToInt(Mathf.Pow(2, chunkParameters.succesiveDivisions - waves));

            //ruidos
            for (int i = 0; i < m; i++) {
                for (int j = 0; j < m; j++) {
                    if (i % waveIndex == 0 && j % waveIndex == 0) {
                        if (neighborHeights[waves, i, j] == 0f) {
                            thisWave[i, j] = Random.Range(0, chunkParameters.amplitude / Mathf.Pow(chunkParameters.persistance, waves));
                            neighborHeights[waves, i, j] = thisWave[i, j];
                        } else thisWave[i, j] = neighborHeights[waves, i, j];
                    }
                }
            }
            //interpolaciones
            for (int i = 0; i < m; i++) {
                for (int j = 0; j < m; j++) {

                    if (i % waveIndex != 0 && j % waveIndex == 0) {
                        int topI = i + 1;
                        while (topI % waveIndex != 0) {
                            topI++;
                        }

                        int bottomI = i - 1;
                        while (bottomI % waveIndex != 0) {
                            bottomI--;
                        }

                        float weighingBottom = ((float)topI - i) / ((float)topI - bottomI);
                        float weighingTop = 1 - weighingBottom;
                        if (thisWave[i, j] == 0f) thisWave[i, j] = thisWave[topI, j] * weighingTop + thisWave[bottomI, j] * weighingBottom;
                    } else if (i % waveIndex == 0 && j % waveIndex != 0) {
                        int rightJ = j + 1;
                        while (rightJ % waveIndex != 0) {
                            rightJ++;
                        }

                        int leftJ = j - 1;
                        while (leftJ % waveIndex != 0) {
                            leftJ--;
                        }

                        float weighingLeft = ((float)rightJ - j) / ((float)rightJ - leftJ);
                        float weighingRight = 1 - weighingLeft;
                        if (thisWave[i, j] == 0f) thisWave[i, j] = thisWave[i, rightJ] * weighingRight + thisWave[i, leftJ] * weighingLeft;
                    } else if (i % waveIndex != 0 && j % waveIndex != 0) {
                        int topI = i + 1;
                        while (topI % waveIndex != 0) {
                            topI++;
                        }
                        int bottomI = i - 1;
                        while (bottomI % waveIndex != 0) {
                            bottomI--;
                        }
                        int rightJ = j + 1;
                        while (rightJ % waveIndex != 0) {
                            rightJ++;
                        }
                        int leftJ = j - 1;
                        while (leftJ % waveIndex != 0) {
                            leftJ--;
                        }

                        float weighingBottom = ((float)topI - i) / ((float)topI - bottomI);
                        float weighingTop = 1 - weighingBottom;
                        float weighingLeft = ((float)rightJ - j) / ((float)rightJ - leftJ);
                        float weighingRight = 1 - weighingLeft;

                        float leftValue = thisWave[topI, leftJ] * weighingTop + thisWave[bottomI, leftJ] * weighingBottom;
                        float rightValue = thisWave[topI, rightJ] * weighingTop + thisWave[bottomI, rightJ] * weighingBottom;
                        float centerValue = leftValue * weighingLeft + rightValue * weighingRight;

                        if (thisWave[i, j] == 0f) thisWave[i, j] = centerValue;

                    }
                }
            }

            //sumarla
            for (int i = 0; i < m; i++) {
                for (int j = 0; j < m; j++) {
                    vertices[i, j].y += thisWave[i, j];
                }
            }
            waves++;
        }

        triadsForTriangles = new int[((m - 1) * (m - 1) * 6)];
        int h = 0;
        int k = 0;
        while (h < triadsForTriangles.Length) {
            triadsForTriangles[h + 0] = k;
            triadsForTriangles[h + 1] = k + m;
            triadsForTriangles[h + 2] = k + 1;

            triadsForTriangles[h + 3] = k + 1;
            triadsForTriangles[h + 4] = k + m;
            triadsForTriangles[h + 5] = k + m + 1;
            h += 6;
            k += 1;
            if ((k + 1) % m == 0) k += 1;
        }

    }
    private void UpdateMesh() {
        mesh.Clear();

        int m = vertices.GetLength(0);
        Vector3[] verticesVector = new Vector3[m * m];
        int index = 0;
        for (int row = 0; row < m; row++) {
            for (int col = 0; col < m; col++) {
                verticesVector[index] = vertices[row, col];
                index++;
            }
        }

        mesh.vertices = verticesVector;
        mesh.triangles = triadsForTriangles;

        calculateLocalNormals();
        pushNormals(false);
    }

    void calculateLocalNormals() {

        int[] triangles = mesh.triangles;
        int triangleCount = triangles.Length / 3;

        for (int i = 0; i < triangleCount; i++) {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexLocalNormals[vertexIndexA] += triangleNormal;
            vertexLocalNormals[vertexIndexB] += triangleNormal;
            vertexLocalNormals[vertexIndexC] += triangleNormal;
        }
        for (int i = 0; i < vertexLocalNormals.Length; i++) {
            vertexLocalNormals[i].Normalize();
        }
    }
    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC) {

        Vector3[] vertices = mesh.vertices;

        Vector3 pointA = vertices[indexA];
        Vector3 pointB = vertices[indexB];
        Vector3 pointC = vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    private void pushNormals(bool dontCallBack) {
        Vector3[] finalNormals = new Vector3[vertexLocalNormals.Length];
        for (int i = 0; i < finalNormals.Length; i++) {
            //finalNormals[i] = Vector3.down;
            finalNormals[i] = vertexLocalNormals[i];
        }

        for (int i = 0; i < finalNormals.Length; i++) {

            int z = Mathf.FloorToInt(i / m);
            int x = i % m;

            if (x == 0 && neighborChunks[4] != null) {
                int neighborI = z * m + (m - 1);
                finalNormals[i] += neighborChunks[4].vertexLocalNormals[neighborI];
            }
            if (x == m - 1 && neighborChunks[6] != null) {
                int neighborI = z * m;
                finalNormals[i] += neighborChunks[6].vertexLocalNormals[neighborI];
            }
            if (z == 0 && neighborChunks[2] != null) {
                int neighborI = m * (m - 1) + x;
                finalNormals[i] += neighborChunks[2].vertexLocalNormals[neighborI];
            }
            if (z == m - 1 && neighborChunks[8] != null) {
                int neighborI = x;
                finalNormals[i] += neighborChunks[8].vertexLocalNormals[neighborI];
            }
            if (x == 0 && z == 0 && neighborChunks[1]) {
                int neighborI = m * m - 1;
                finalNormals[i] += neighborChunks[1].vertexLocalNormals[neighborI];
            }
            if (x == m - 1 && z == 0 && neighborChunks[3]) {
                int neighborI = m * (m - 1);
                finalNormals[i] += neighborChunks[3].vertexLocalNormals[neighborI];
            }
            if (x == 0 && z == m - 1 && neighborChunks[7]) {
                int neighborI = (m - 1);
                finalNormals[i] += neighborChunks[7].vertexLocalNormals[neighborI];
            }
            if (x == m - 1 && z == m - 1 && neighborChunks[9]) {
                int neighborI = 0;
                finalNormals[i] += neighborChunks[9].vertexLocalNormals[neighborI];
            }

            finalNormals[i].Normalize();
        }

        if (neighborChunks[8] != null && !dontCallBack) {
            neighborChunks[8].setNeighborChunk(2, this);
            neighborChunks[8].pushNormals(true);
        }
        if (neighborChunks[4] != null && !dontCallBack) {
            neighborChunks[4].setNeighborChunk(6, this);
            neighborChunks[4].pushNormals(true);
        }
        if (neighborChunks[6] != null && !dontCallBack) {
            neighborChunks[6].setNeighborChunk(4, this);
            neighborChunks[6].pushNormals(true);
        }
        if (neighborChunks[2] != null && !dontCallBack) {
            neighborChunks[2].setNeighborChunk(8, this);
            neighborChunks[2].pushNormals(true);
        }

        if (neighborChunks[1] != null && !dontCallBack) {
            neighborChunks[1].setNeighborChunk(9, this);
            neighborChunks[1].pushNormals(true);
        }
        if (neighborChunks[3] != null && !dontCallBack) {
            neighborChunks[3].setNeighborChunk(7, this);
            neighborChunks[3].pushNormals(true);
        }
        if (neighborChunks[7] != null && !dontCallBack) {
            neighborChunks[7].setNeighborChunk(3, this);
            neighborChunks[7].pushNormals(true);
        }
        if (neighborChunks[9] != null && !dontCallBack) {
            neighborChunks[9].setNeighborChunk(1, this);
            neighborChunks[9].pushNormals(true);
        }

        mesh.SetNormals(finalNormals);
        myMeshFilter.mesh = mesh;
        SetUVs();
    }
    private void SetUVs() {
        Color[] colors = new Color[mesh.vertexCount];

        for (int i = 0; i < mesh.vertexCount; i++) {
            //float t = Mathf.Max(mesh.vertices[i].y / (maxHeight), 1-);
            float t = 1 - Vector3.Dot(Vector3.up, mesh.normals[i]);
            colors[i] = chunkParameters.gradient.Evaluate(7*t);
            //i++;
        }

        mesh.colors = colors;
    }
    private void SpawnTrees() {

        int cant = 0;
        while (cant < chunkParameters.amountOfTrees) {

            int x = Random.Range(0, vertices.GetLength(0) - 1);
            int y = Random.Range(0, vertices.GetLength(0) - 1);
            Instantiate(prefabs.tree, vertices[x, y] + transform.position, Quaternion.identity);
            cant++;
        }

    }

    internal void setNeighborChunk(int i, Chunk chunk) {
        neighborChunks[i] = chunk;
    }

    internal float[,] getSide(int number) {
        float[,] side = new float[chunkParameters.succesiveDivisions + 1, m];

        switch (number) {
            case 1:
                for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                    side[k, 0] = neighborHeights[k, 0, 0];
                }
                break;
            case 2:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                        side[k, i] = neighborHeights[k, 0, i];
                    }
                }
                break;
            case 3:
                for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                    side[k, 0] = neighborHeights[k, 0, m - 1];
                }
                break;
            case 4:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                        side[k, i] = neighborHeights[k, i, 0];
                    }
                }
                break;
            //case 5:
            case 6:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                        side[k, i] = neighborHeights[k, i, m - 1];
                    }
                }
                break;
            case 7:
                for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                    side[k, 0] = neighborHeights[k, m - 1, 0];
                }
                break;
            case 8:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                        side[k, i] = neighborHeights[k, m - 1, i];
                    }
                }
                break;
            case 9:
                for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                    side[k, 0] = neighborHeights[k, m - 1, m - 1];
                }
                break;
        }

        return side;
    }

    internal void setNeighborHeights(int neighbor, float[,] vec) {
        switch (neighbor) {
            case 1:
                for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                    neighborHeights[k, 0, 0] = vec[k, 0];
                }
                break;
            case 2:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                        neighborHeights[k, 0, i] = vec[k, i];
                    }
                }
                break;
            case 3:
                for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                    neighborHeights[k, 0, m - 1] = vec[k, 0];
                }
                break;
            case 4:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                        neighborHeights[k, i, 0] = vec[k, i];
                    }
                }
                break;
            case 6:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                        neighborHeights[k, i, m - 1] = vec[k, i];
                    }
                }
                break;
            case 7:
                for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                    neighborHeights[k, m - 1, 0] = vec[k, 0];
                }
                break;
            case 8:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                        neighborHeights[k, m - 1, i] = vec[k, i];
                    }
                }
                break;
            case 9:
                for (int k = 0; k < chunkParameters.succesiveDivisions + 1; k++) {
                    neighborHeights[k, m - 1, m - 1] = vec[k, 0];
                }
                break;
        }
    }


}
