using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    Mesh mesh;
    int chunkSize = 1;
    int successiveDivisions = 5;
    private Material defaultMaterial;
    Vector3[,] vertices;
    float[,,] neighborHeights;
    int[] triadsForTriangles;
    int m;
    private float amplitude = 20;
    public Prefabs prefabs;

    private void CreateMatrix() {
        vertices = new Vector3[m, m];
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

        mesh.RecalculateNormals();
    }


    //Vector3[] CalculateNormals() {

    //    Vector3[] vertexNormals = new Vector3[vertices.Length];
    //    int triangleCount = triangles.Length / 3;
    //    for (int i = 0; i < triangleCount; i++) {
    //        int normalTriangleIndex = i * 3;
    //        int vertexIndexA = triangles[normalTriangleIndex];
    //        int vertexIndexB = triangles[normalTriangleIndex + 1];
    //        int vertexIndexC = triangles[normalTriangleIndex + 2];

    //        Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
    //        vertexNormals[vertexIndexA] += triangleNormal;
    //        vertexNormals[vertexIndexB] += triangleNormal;
    //        vertexNormals[vertexIndexC] += triangleNormal;
    //    }

    //    int borderTriangleCount = borderTriangles.Length / 3;
    //    for (int i = 0; i < borderTriangleCount; i++) {
    //        int normalTriangleIndex = i * 3;
    //        int vertexIndexA = borderTriangles[normalTriangleIndex];
    //        int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
    //        int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

    //        Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
    //        if (vertexIndexA >= 0) {
    //            vertexNormals[vertexIndexA] += triangleNormal;
    //        }
    //        if (vertexIndexB >= 0) {
    //            vertexNormals[vertexIndexB] += triangleNormal;
    //        }
    //        if (vertexIndexC >= 0) {
    //            vertexNormals[vertexIndexC] += triangleNormal;
    //        }
    //    }


    //    for (int i = 0; i < vertexNormals.Length; i++) {
    //        vertexNormals[i].Normalize();
    //    }

    //    return vertexNormals;

    //}

    //Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC) {
    //    Vector3 pointA = (indexA < 0) ? borderVertices[-indexA - 1] : vertices[indexA];
    //    Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
    //    Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];

    //    Vector3 sideAB = pointB - pointA;
    //    Vector3 sideAC = pointC - pointA;
    //    return Vector3.Cross(sideAB, sideAC).normalized;
    //}

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
                            thisWave[i, j] = Random.Range(0, amplitude / Mathf.Pow(2, waves));
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

    internal void setNeighbor(int neighbor, float[,] vec) {
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

    public void init(int chunkSize, Material defaultMaterial) {
        this.chunkSize = chunkSize;
        this.defaultMaterial = defaultMaterial;

        int succesivePow2 = Mathf.RoundToInt(Mathf.Pow(2, successiveDivisions));
        m = Mathf.RoundToInt(succesivePow2 + 1);
        CreateMatrix();
    }

}
