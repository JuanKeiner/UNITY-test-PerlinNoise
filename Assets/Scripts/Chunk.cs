using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    Mesh mesh;
    int chunkSize = 64;
    int successiveDivisions = 5;
    float amplitude = 20;
    Material defaultMaterial;
    Vector3[,] vertices;
    Vector3[] vertexNormalsLocal;
    bool leftVertexSet;
    Vector3[] myVertexNormalsLeft;
    bool topVertexSet;
    Vector3[] myVertexNormalsTop;
    bool rightVertexSet;
    Vector3[] myVertexNormalsRight;
    bool bottomVertexSet;
    Vector3[] myVertexNormalsBottom;
    float[,,] neighborHeights;
    Chunk[] neighborChunks;
    int[] triadsForTriangles;
    int m;
    public Prefabs prefabs;


    private void CreateMatrix() {
        vertices = new Vector3[m, m];
        vertexNormalsLocal = new Vector3[vertices.Length];

        neighborHeights = new float[successiveDivisions + 1, m, m];

        for (int i = 0; i < m; i++) {
            for (int j = 0; j < m; j++) {
                vertices[i, j] = new Vector3(((float)j / (m - 1)) * chunkSize, 0f, ((float)i / (m - 1)) * chunkSize);
            }
        }
        for (int i = 0; i < m; i++) {
            for (int j = 0; j < m; j++) {
                for (int k = 0; k < successiveDivisions + 1; k++) {
                    neighborHeights[k, i, j] = 0f;
                }
            }
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

        //mesh.RecalculateNormals();
        calculateNormals();
        //calculateSideNormals();
        pushNormals();

    }

    internal Vector3[] getSideNormal(int number) {
        Vector3[] side = new Vector3[m];

        switch (number) {
            case 2:
                for (int i = 0; i < mesh.normals.Length; i++) {
                    int z = Mathf.FloorToInt(i / m);
                    int x = i % m;
                    if (z == 0) {
                        side[x] = mesh.normals[i];
                    }
                }
                break;
            case 4:
                for (int i = 0; i < mesh.normals.Length; i++) {
                    int z = Mathf.FloorToInt(i / m);
                    int x = i % m;
                    if (x == 0) {
                        side[z] = mesh.normals[i];
                    }
                }
                break;
            case 6:
                for (int i = 0; i < mesh.normals.Length; i++) {
                    int z = Mathf.FloorToInt(i / m);
                    int x = i % m;
                    if (x == m - 1) {
                        side[z] = mesh.normals[i];
                    }
                }
                break;
            case 8:
                for (int i = 0; i < mesh.normals.Length; i++) {
                    int z = Mathf.FloorToInt(i / m);
                    int x = i % m;
                    if (z == m - 1) {
                        side[x] = mesh.normals[i];
                    }
                }
                break;
        }

        return side;
    }

    void calculateNormals() {
        myVertexNormalsLeft = new Vector3[m];
        myVertexNormalsTop = new Vector3[m];
        myVertexNormalsRight = new Vector3[m];
        myVertexNormalsBottom = new Vector3[m];


        int[] triangles = mesh.triangles;
        int triangleCount = triangles.Length / 3;


        for (int i = 0; i < triangleCount; i++) {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormalsLocal[vertexIndexA] += triangleNormal;
            vertexNormalsLocal[vertexIndexB] += triangleNormal;
            vertexNormalsLocal[vertexIndexC] += triangleNormal;

        }

    }

    void calculateSideNormals() {

        Vector3[] _2side, _4side, _6side, _8side;
        bool bottom = neighborChunks[2] != null;
        bool left = neighborChunks[4] != null;
        bool right = neighborChunks[6] != null;
        bool top = neighborChunks[8] != null;
        if (bottom) _2side = neighborChunks[2].getSideNormal(2);
        else _2side = new Vector3[0];

        if (left) _4side = neighborChunks[4].getSideNormal(4);
        else _4side = new Vector3[0];

        if (right) _6side = neighborChunks[6].getSideNormal(6);
        else _6side = new Vector3[0];

        if (top) _8side = neighborChunks[8].getSideNormal(8);
        else _8side = new Vector3[0];

        for (int i = 0; i < vertexNormalsLocal.Length; i++) {

            int z = Mathf.FloorToInt(i / m);
            int x = i % m;

            if (x == 0 && left && !leftVertexSet) {
                myVertexNormalsLeft[z] = _4side[z];
                leftVertexSet = true;
                neighborChunks[4].setNeighborChunk(6, GetComponent<Chunk>());
                neighborChunks[4].calculateSideNormals();
            }
            if (x == m - 1 && right && !rightVertexSet) {
                myVertexNormalsRight[z] = _6side[z];
                rightVertexSet = true;
                neighborChunks[6].setNeighborChunk(4, GetComponent<Chunk>());
                neighborChunks[6].calculateSideNormals();
            }
            if (z == 0 && bottom && !bottomVertexSet) {
                myVertexNormalsBottom[x] = _2side[z];
                bottomVertexSet = true;
                neighborChunks[2].setNeighborChunk(8, GetComponent<Chunk>());
                neighborChunks[2].calculateSideNormals();
            }
            if (z == m - 1 && top && !topVertexSet) {
                myVertexNormalsTop[x] = _8side[z];
                topVertexSet = true;
                neighborChunks[8].setNeighborChunk(2, GetComponent<Chunk>());
                neighborChunks[8].calculateSideNormals();
            }
        }
    }

    private void pushNormals() {

        Vector3[] finalNormals = vertexNormalsLocal;

        for (int i = 0; i < finalNormals.Length; i++) {

            int z = Mathf.FloorToInt(i / m);
            int x = i % m;


            if (x == 0 && leftVertexSet) {
                finalNormals[i] = Vector3.down;
                //finalNormals[i] += myVertexNormalsLeft[z];
            }
            if (x == m - 1 && rightVertexSet) {
                finalNormals[i] += myVertexNormalsRight[z];
            }
            if (z == 0 && bottomVertexSet) {
                finalNormals[i] += myVertexNormalsBottom[z];
            }
            if (z == m - 1 && topVertexSet) {
                finalNormals[i] += myVertexNormalsTop[z];
            }


            finalNormals[i].Normalize();
        }

        mesh.normals = finalNormals;
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

    private void CreateShape() {

        int waves = 0;
        while (waves < successiveDivisions + 1) {
            float[,] thisWave = new float[m, m];
            int waveIndex = Mathf.RoundToInt(Mathf.Pow(2, successiveDivisions - waves));

            //ruidos
            for (int i = 0; i < m; i++) {
                for (int j = 0; j < m; j++) {
                    if (i % waveIndex == 0 && j % waveIndex == 0) {
                        if (neighborHeights[waves, i, j] == 0f) {
                            thisWave[i, j] = Random.Range(0, amplitude / Mathf.Pow(2.5f, waves));
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

    internal void setNeighborChunk(int i, Chunk chunk) {
        neighborChunks[i] = chunk;
    }

    internal float[,] getSide(int number) {
        float[,] side = new float[successiveDivisions + 1, m];

        switch (number) {
            case 1:
                for (int k = 0; k < successiveDivisions + 1; k++) {
                    side[k, 0] = neighborHeights[k, 0, 0];
                }
                break;
            case 2:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < successiveDivisions + 1; k++) {
                        side[k, i] = neighborHeights[k, 0, i];
                    }
                }
                break;
            case 3:
                for (int k = 0; k < successiveDivisions + 1; k++) {
                    side[k, 0] = neighborHeights[k, 0, m - 1];
                }
                break;
            case 4:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < successiveDivisions + 1; k++) {
                        side[k, i] = neighborHeights[k, i, 0];
                    }
                }
                break;
            //case 5:
            case 6:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < successiveDivisions + 1; k++) {
                        side[k, i] = neighborHeights[k, i, m - 1];
                    }
                }
                break;
            case 7:
                for (int k = 0; k < successiveDivisions + 1; k++) {
                    side[k, 0] = neighborHeights[k, m - 1, 0];
                }
                break;
            case 8:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < successiveDivisions + 1; k++) {
                        side[k, i] = neighborHeights[k, m - 1, i];
                    }
                }
                break;
            case 9:
                for (int k = 0; k < successiveDivisions + 1; k++) {
                    side[k, 0] = neighborHeights[k, m - 1, m - 1];
                }
                break;
        }

        return side;
    }

    internal void setNeighborHeights(int neighbor, float[,] vec) {
        switch (neighbor) {
            case 1:
                for (int k = 0; k < successiveDivisions + 1; k++) {
                    neighborHeights[k, 0, 0] = vec[k, 0];
                }
                break;
            case 2:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < successiveDivisions + 1; k++) {
                        neighborHeights[k, 0, i] = vec[k, i];
                    }
                }
                break;
            case 3:
                for (int k = 0; k < successiveDivisions + 1; k++) {
                    neighborHeights[k, 0, m - 1] = vec[k, 0];
                }
                break;
            case 4:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < successiveDivisions + 1; k++) {
                        neighborHeights[k, i, 0] = vec[k, i];
                    }
                }
                break;
            case 6:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < successiveDivisions + 1; k++) {
                        neighborHeights[k, i, m - 1] = vec[k, i];
                    }
                }
                break;
            case 7:
                for (int k = 0; k < successiveDivisions + 1; k++) {
                    neighborHeights[k, m - 1, 0] = vec[k, 0];
                }
                break;
            case 8:
                for (int i = 0; i < m; i++) {
                    for (int k = 0; k < successiveDivisions + 1; k++) {
                        neighborHeights[k, m - 1, i] = vec[k, i];
                    }
                }
                break;
            case 9:
                for (int k = 0; k < successiveDivisions + 1; k++) {
                    neighborHeights[k, m - 1, m - 1] = vec[k, 0];
                }
                break;
        }
    }

    public void createChunk(ref Prefabs prefabs) {
        this.prefabs = prefabs;
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        MeshFilter myMeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshCollider myMeshCollider = gameObject.AddComponent<MeshCollider>();
        mesh = new Mesh();
        myMeshFilter.mesh = mesh;
        meshRenderer.material = defaultMaterial;

        CreateShape();
        UpdateMesh();
        SpawnTrees();
        myMeshCollider.sharedMesh = myMeshFilter.mesh;
    }

    private void SpawnTrees() {
        int x = Random.Range(0, vertices.GetLength(0) - 1);
        int y = Random.Range(0, vertices.GetLength(0) - 1);

        Instantiate(prefabs.tree, vertices[x, y] + transform.position, Quaternion.identity);
        x = Random.Range(0, vertices.GetLength(0) - 1);
        y = Random.Range(0, vertices.GetLength(0) - 1);

        Instantiate(prefabs.tree, vertices[x, y] + transform.position, Quaternion.identity);
        x = Random.Range(0, vertices.GetLength(0) - 1);
        y = Random.Range(0, vertices.GetLength(0) - 1);

        Instantiate(prefabs.tree, vertices[x, y] + transform.position, Quaternion.identity);
        x = Random.Range(0, vertices.GetLength(0) - 1);
        y = Random.Range(0, vertices.GetLength(0) - 1);

        Instantiate(prefabs.tree, vertices[x, y] + transform.position, Quaternion.identity);
    }

    public ref Mesh getMesh() {
        return ref mesh;
    }

    public void init(int chunkSize, int amplitude, Material defaultMaterial) {
        this.chunkSize = chunkSize;
        this.defaultMaterial = defaultMaterial;
        this.amplitude = amplitude;

        neighborChunks = new Chunk[10];

        int succesivePow2 = Mathf.RoundToInt(Mathf.Pow(2, successiveDivisions));
        m = Mathf.RoundToInt(succesivePow2 + 1);
        CreateMatrix();
    }

}
