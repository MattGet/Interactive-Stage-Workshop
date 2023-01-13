using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Light))]
public class VolumetricLightMesh : MonoBehaviour
{
    [Range(0, 10)]
    public float FadeValue = 7f;
    //public Texture2D LightEffect;
    public Shape LightShape;
    private string MaterialsFolderPath = "";
    public Material LightMat;
    private Material LightInstance;
    [Tooltip("The object/Mesh that Represents the light/lightbulb surface this is used to make the bulb change colors along with the actual light")]
    public GameObject LightObject;
    private float ShadowRadius;
    private float ShadowRange;

    //[SerializeField]
    private MeshFilter filter;
    //[SerializeField]
    private MeshRenderer mrenderer;
    //[SerializeField]
    private Light spotlight;

    private Mesh mesh;

    public enum Shape { 
        Cone,
    }


    // Use this for initialization
    void Start()
    {
        filter = GetComponent<MeshFilter>();
        mrenderer = GetComponent<MeshRenderer>();
        spotlight = GetComponent<Light>();
        if (spotlight.type != LightType.Spot)
        {
            spotlight.type = LightType.Spot;
        }
        if (LightMat == null)
        {
            LightMat = (Material)Resources.Load("Realistic Light Material");
        }
    }

    private void OnValidate()
    {
        if (LightObject == null)
        {
            if (this.gameObject.transform.parent.gameObject != null) LightObject = this.gameObject.transform.parent.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mrenderer.receiveShadows)
        {
            mrenderer.receiveShadows = false;
            mrenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        float range = spotlight.range * (this.gameObject.transform.localScale.magnitude / this.gameObject.transform.lossyScale.magnitude);
        float radius = Mathf.Tan(spotlight.spotAngle * 0.5f * Mathf.Deg2Rad) * range;
        if (range != ShadowRange || radius != ShadowRadius)
        {
            if (LightShape == Shape.Cone)
            {
                if (mesh != null && Application.isPlaying) Destroy(mesh);
                else if (mesh != null && Application.isEditor) DestroyImmediate(mesh);
                mesh = CreateCone(24, radius, range);
            }
            ShadowRadius = radius;
            ShadowRange = range;
        }


        if (LightMat != null)
        {
            mrenderer.sharedMaterial = LightMat;
        }
        filter.mesh = mesh;
        if (mrenderer != null && mrenderer.sharedMaterial != null)
        {
            mrenderer.sharedMaterial.SetColor("_Color", spotlight.color);
            mrenderer.sharedMaterial.SetFloat("_FadeDist", spotlight.range * 2);
            mrenderer.sharedMaterial.SetFloat("_Power", FadeValue);
            //if (LightEffect != null)
            //{
            //    mrenderer.sharedMaterial.SetTexture("_MainTex", LightEffect);
            //}
        }
        if (Application.isPlaying)
        {
            if (LightObject != null)
            {
                MeshRenderer temp = LightObject.GetComponent<MeshRenderer>();
                temp.sharedMaterial.SetColor("_EmissionColor", spotlight.color);
            }
        }
        

    }

    Mesh CreateCone(int subdivisions, float radius, float height)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[subdivisions + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[(subdivisions * 2) * 3];

        vertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0f);
        for (int i = 0, n = subdivisions - 1; i < subdivisions; i++)
        {
            float ratio = (float)i / n;
            float r = ratio * (Mathf.PI * 2f);
            float x = Mathf.Cos(r) * radius;
            float z = Mathf.Sin(r) * radius;
            vertices[i + 1] = new Vector3(z, x, height);
            //Debug.Log(ratio);
            uv[i + 1] = new Vector2(0f, ratio);
        }
        vertices[subdivisions + 1] = new Vector3(0f, 0f, height);
        uv[subdivisions + 1] = new Vector2(0.5f, 1f);

        // construct bottom

        for (int i = 0, n = subdivisions - 1; i < n; i++)
        {
            int offset = i * 3;
            triangles[offset] = 0;
            triangles[offset + 1] = i + 1;
            triangles[offset + 2] = i + 2;
        }

        // construct sides

        int bottomOffset = subdivisions * 3;
        for (int i = 0, n = subdivisions - 1; i < n; i++)
        {
            int offset = i * 3 + bottomOffset;
            triangles[offset] = i + 1;
            triangles[offset + 1] = subdivisions + 1;
            triangles[offset + 2] = i + 2;
        }
        Color[] colors = new Color[subdivisions + 2];
        colors[0] = new Color(spotlight.color.r, spotlight.color.g, spotlight.color.b, spotlight.color.a);
        for (int i = 0, n = subdivisions - 1; i < subdivisions; i++)
        {
            colors[i + 1] = new Color(spotlight.color.r, spotlight.color.g, spotlight.color.b, 0);
        }
        colors[subdivisions + 1] = new Color(spotlight.color.r, spotlight.color.g, spotlight.color.b, 0);
        //Debug.Log($"Verts = {vertices.Length}, Colors = {colors.Length}");
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }
}
