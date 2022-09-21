using UnityEngine;

public class SmokeFX : MonoBehaviourBase
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material smokeMaterial;
    [SerializeField] private Gradient smokeColor;
    [SerializeField] [Range(0f, 1f)] private float defaultAlpha = 1f;
    [SerializeField] private int subdivisions = 20;

    Vector3[] vertices;
    Vector2[] uvs;
    int[] indices;
    Color[] colors;

    int alphaShaderId;
    bool isInit = false;
    Material rendererMat = null;

    #region UNITY AND CORE

    protected override void Awake()
    {
        base.Awake();

        init();
    }

    #endregion

    #region PRIVATE

    private void init()
    {
        if (true == isInit)
            return;

        meshRenderer.material = smokeMaterial;

        vertices = new Vector3[(subdivisions + 1) * 2];
        colors = new Color[vertices.Length];
        uvs = new Vector2[(subdivisions + 1) * 2];
        indices = new int[subdivisions * 4];

        createMesh(meshFilter.mesh);

        meshRenderer.material.SetFloat("_Wind", Random.Range(2f, 3f));

        alphaShaderId = Shader.PropertyToID("_Alpha");

        rendererMat = meshRenderer.material;

        rendererMat.SetFloat(alphaShaderId, defaultAlpha);

        isInit = true;
    }

    void createMesh(Mesh mesh)
    {
        for (int i = 0; i < subdivisions + 1; i++)
        {
            float lerp = (float)i / (float)subdivisions;

            Color col = smokeColor.Evaluate(lerp);

            colors[i * 2 + 0] = col;
            colors[i * 2 + 1] = col;

            vertices[i * 2 + 0] = new Vector3(-0.5f, 10f * lerp, 0);
            vertices[i * 2 + 1] = new Vector3(0.5f, 10f * lerp, 0);

            uvs[i * 2 + 0] = new Vector2(0f, lerp);
            uvs[i * 2 + 1] = new Vector2(1f, lerp);
        }

        for (int i = 0; i < subdivisions; i++)
        {
            indices[i * 4 + 0] = i * 2 + 0;
            indices[i * 4 + 1] = i * 2 + 1;
            indices[i * 4 + 2] = i * 2 + 3;
            indices[i * 4 + 3] = i * 2 + 2;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.SetIndices(indices, MeshTopology.Quads, 0);
    }

    #endregion





}