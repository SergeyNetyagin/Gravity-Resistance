using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent( typeof( MeshFilter ) )]
public class SphereRuntime : MonoBehaviour {

    [SerializeField]
    [Range( 0.1f, 100.0f )]
    [Tooltip( "Радиус сферы в единицах Юнити; по умолчанию = 0.5" )]
    protected float radius = 0.5f;
    public float Radius { get { return radius; } }
    public void SetRadius( float radius ) { this.radius = radius; }

    [SerializeField]
    [Range( 1, 6 )]
    [Tooltip( "Уровень качества от 1 до 6: чем выше качество, тем больше вертексов в меше; по умолчанию = 4" )]
    protected int quality = 4;
    public int Quality { get { return quality; } }
    public void SetQuality( int quality ) { this.quality = quality; }

    [HideInInspector, SerializeField]
    private int detail = -1;

    [HideInInspector, SerializeField]
    private float lastRadius = 0.0f;

    [HideInInspector, SerializeField]
    private int lastDetail = 0;

    // Starting initialization #################################################################################################################################################
    protected virtual void Start() {

        StartingInitialization();
    }

    // Public method need for dynamic creating of sphere #######################################################################################################################
    public void StartingInitialization() {

        if( Application.isPlaying || Is_dirty ) CreateSphere();
    }

    #if UNITY_EDITOR
    // Update for dynamic change of the sphere parameters ######################################################################################################################
    protected virtual void Update() {

        if( !Application.isPlaying && Is_dirty ) CreateSphere();
    }
    #endif
    
    // Check for change radius or quality of the sphere ########################################################################################################################
    private bool Is_dirty { get {

        bool dirty = false;

        detail = quality;

        if( radius != lastRadius ) { lastRadius = radius; dirty = true; }

        if( detail != lastDetail ) { lastDetail = detail; dirty = true; }

        if( GetComponent<MeshFilter>().sharedMesh == null ) dirty = true;

        return dirty;
    } }

    // Create the sphere #######################################################################################################################################################
    private void CreateSphere() {

        detail = quality;

        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = filter.sharedMesh;

        if( mesh == null ) {

                mesh = filter.sharedMesh = new Mesh();
                mesh.name = gameObject.name + "_mesh";
        }

        mesh.Clear();

        int latitudeCount = 10 * detail;
        int longitudeCount = 15 * detail;

        Vector3[] vertices = new Vector3[(longitudeCount + 1) * latitudeCount + 2];
        float _pi = Mathf.PI;
        float _2pi = _pi * 2f;

        vertices[0] = Vector3.up * radius;

        for (int lat = 0; lat < latitudeCount; lat++) {

            float a1 = _pi * (float)(lat + 1) / (latitudeCount + 1);
            float sin1 = Mathf.Sin(a1);
            float cos1 = Mathf.Cos(a1);

            for (int lon = 0; lon <= longitudeCount; lon++) {

                float a2 = _2pi * (float)(lon == longitudeCount ? 0 : lon) / longitudeCount;
                float sin2 = Mathf.Sin(a2);
                float cos2 = Mathf.Cos(a2);

                vertices[lon + lat * (longitudeCount + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
            }
        }

        vertices[vertices.Length - 1] = Vector3.up * (- radius);
	
        Vector3[] normales = new Vector3[vertices.Length];
        for (int n = 0; n < vertices.Length; n++) normales[n] = vertices[n].normalized;

        Vector2[] uvs = new Vector2[vertices.Length];
        uvs[0] = Vector2.up;
        uvs[uvs.Length - 1] = Vector2.zero;

        for (int lat = 0; lat < latitudeCount; lat++)
            for (int lon = 0; lon <= longitudeCount; lon++)
                uvs[lon + lat * (longitudeCount + 1) + 1] = new Vector2((float)lon / longitudeCount, 1f - (float)(lat + 1) / (latitudeCount + 1));

        int nbFaces = vertices.Length;
        int nbTriangles = nbFaces * 2;
        int nbIndexes = nbTriangles * 3;
        int[] triangles = new int[nbIndexes];

        //Top Cap
        int i = 0;
        for (int lon = 0; lon < longitudeCount; lon++) {

            triangles[i++] = lon + 2;
            triangles[i++] = lon + 1;
            triangles[i++] = 0;
        }

        //Middle
        for (int lat = 0; lat < latitudeCount - 1; lat++) {

            for (int lon = 0; lon < longitudeCount; lon++) {

                int current = lon + lat * (longitudeCount + 1) + 1;
                int next = current + longitudeCount + 1;

                triangles[i++] = current;
                triangles[i++] = current + 1;
                triangles[i++] = next + 1;

                triangles[i++] = current;
                triangles[i++] = next + 1;
                triangles[i++] = next;
            }
        }

        //Bottom Cap
        for (int lon = 0; lon < longitudeCount; lon++) {

            triangles[i++] = vertices.Length - 1;
            triangles[i++] = vertices.Length - (lon + 2) - 1;
            triangles[i++] = vertices.Length - (lon + 1) - 1;
        }

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        ;
    }
}