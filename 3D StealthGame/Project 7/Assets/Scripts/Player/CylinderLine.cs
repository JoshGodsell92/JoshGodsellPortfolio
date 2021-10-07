//////////////////////////////////////////////////////////////////////////
///File name: CylinderLine.cs
///Date Created: 24/03/2021
///Created by: JG
///Brief: Class for Drawing cylindrical line
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderLine : MonoBehaviour
{
    private Vector3[] m_aPositions;
    [SerializeField] private int m_iSides;
    [SerializeField] private float m_fRadiusOne;
    [SerializeField] private float m_fRadiusTwo;

    [SerializeField] private bool m_bUseWorldSpace = true;
    [SerializeField] private bool m_bUseBothRadius = false;
    [SerializeField] private bool m_bUseShadows = false;

    private Vector3[] m_aVertices;
    private Mesh m_mMesh;
    private MeshFilter m_mfMeshFilter;
    private MeshRenderer m_mrMeshRenderer;

    public Material material
    {
        get { return m_mrMeshRenderer.material; }
        set { m_mrMeshRenderer.material = value; }
    }

    void Awake()
    {
        m_mfMeshFilter = GetComponent<MeshFilter>();
        if (m_mfMeshFilter == null)
        {
            m_mfMeshFilter = gameObject.AddComponent<MeshFilter>();
        }

        m_mrMeshRenderer = GetComponent<MeshRenderer>();
        if (m_mrMeshRenderer == null)
        {
            m_mrMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        m_mMesh = new Mesh();
        m_mfMeshFilter.mesh = m_mMesh;

        if (!m_bUseShadows)
        {
            m_mrMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    private void OnEnable()
    {
        m_mrMeshRenderer.enabled = true;
    }

    private void OnDisable()
    {
        m_mrMeshRenderer.enabled = false;
    }

    void Update()
    {
        GenerateMesh();
    }

    private void OnValidate()
    {
        m_iSides = Mathf.Max(3, m_iSides);
    }

    public void SetPositions(Vector3[] positions)
    {
        m_aPositions = positions;
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        if (m_mMesh == null || m_aPositions == null || m_aPositions.Length <= 1)
        {
            m_mMesh = new Mesh();
            return;
        }

        var verticesLength = m_iSides * m_aPositions.Length;
        if (m_aVertices == null || m_aVertices.Length != verticesLength)
        {
            m_aVertices = new Vector3[verticesLength];

            var indices = GenerateIndices();
            var uvs = GenerateUVs();

            if (verticesLength > m_mMesh.vertexCount)
            {
                m_mMesh.vertices = m_aVertices;
                m_mMesh.triangles = indices;
                m_mMesh.uv = uvs;
            }
            else
            {
                m_mMesh.triangles = indices;
                m_mMesh.vertices = m_aVertices;
                m_mMesh.uv = uvs;
            }
        }

        var currentVertIndex = 0;

        for (int i = 0; i < m_aPositions.Length; i++)
        {
            var circle = CalculateCircle(i);
            foreach (var vertex in circle)
            {
                m_aVertices[currentVertIndex++] = m_bUseWorldSpace ? transform.InverseTransformPoint(vertex) : vertex;
            }
        }

        m_mMesh.vertices = m_aVertices;
        m_mMesh.RecalculateNormals();
        m_mMesh.RecalculateBounds();

        m_mfMeshFilter.mesh = m_mMesh;
    }

    private Vector2[] GenerateUVs()
    {
        var uvs = new Vector2[m_aPositions.Length * m_iSides];

        for (int segment = 0; segment < m_aPositions.Length; segment++)
        {
            for (int side = 0; side < m_iSides; side++)
            {
                var vertIndex = (segment * m_iSides + side);
                var u = side / (m_iSides - 1f);
                var v = segment / (m_aPositions.Length - 1f);

                uvs[vertIndex] = new Vector2(u, v);
            }
        }

        return uvs;
    }

    private int[] GenerateIndices()
    {
        // Two triangles and 3 vertices
        var indices = new int[m_aPositions.Length * m_iSides * 2 * 3];

        var currentIndicesIndex = 0;
        for (int segment = 1; segment < m_aPositions.Length; segment++)
        {
            for (int side = 0; side < m_iSides; side++)
            {
                var vertIndex = (segment * m_iSides + side);
                var prevVertIndex = vertIndex - m_iSides;

                // Triangle one
                indices[currentIndicesIndex++] = prevVertIndex;
                indices[currentIndicesIndex++] = (side == m_iSides - 1) ? (vertIndex - (m_iSides - 1)) : (vertIndex + 1);
                indices[currentIndicesIndex++] = vertIndex;


                // Triangle two
                indices[currentIndicesIndex++] = (side == m_iSides - 1) ? (prevVertIndex - (m_iSides - 1)) : (prevVertIndex + 1);
                indices[currentIndicesIndex++] = (side == m_iSides - 1) ? (vertIndex - (m_iSides - 1)) : (vertIndex + 1);
                indices[currentIndicesIndex++] = prevVertIndex;
            }
        }

        return indices;
    }

    private Vector3[] CalculateCircle(int index)
    {
        var dirCount = 0;
        var forward = Vector3.zero;

        // If not first index
        if (index > 0)
        {
            forward += (m_aPositions[index] - m_aPositions[index - 1]).normalized;
            dirCount++;
        }

        // If not last index
        if (index < m_aPositions.Length - 1)
        {
            forward += (m_aPositions[index + 1] - m_aPositions[index]).normalized;
            dirCount++;
        }

        // Forward is the average of the connecting edges directions
        forward = (forward / dirCount).normalized;
        var side = Vector3.Cross(forward, forward + new Vector3(.123564f, .34675f, .756892f)).normalized;
        var up = Vector3.Cross(forward, side).normalized;

        var circle = new Vector3[m_iSides];
        var angle = 0f;
        var angleStep = (2 * Mathf.PI) / m_iSides;

        var t = index / (m_aPositions.Length - 1f);
        var radius = m_bUseBothRadius ? Mathf.Lerp(m_fRadiusOne, m_fRadiusTwo, t) : m_fRadiusOne;

        for (int i = 0; i < m_iSides; i++)
        {
            var x = Mathf.Cos(angle);
            var y = Mathf.Sin(angle);

            circle[i] = m_aPositions[index] + side * x * radius + up * y * radius;

            angle += angleStep;
        }

        return circle;
    }
}
