using UnityEngine;

public class ForceFieldCustomized : MonoBehaviour {

    private new Transform transform;

    // Force Field component cache variables
    [SerializeField]
    [Tooltip( "Материал для визуализации момента удара о даное тело" )]
    protected Material hit_material;
    public void SetHitMaterial( Material material ) { hit_material = material; }

    [SerializeField]
    [Tooltip( "Меш, по которому рассчитывается эффект удара" )]
    protected MeshFilter hit_mesh_filter; 
    public void SetHitFilter( MeshFilter filter ) { hit_mesh_filter = filter; }

    // Force Field reaction speed
    [SerializeField]
    [Range( 0f, 1f )]
    [Tooltip( "Скорость реакции на контакт с телом" )]
    protected float reaction_speed = 0.1f;
    public void SetReactionSpeed( float speed ) { reaction_speed = speed; }
    
    // Speed at which interpolators will fade
    [SerializeField]
    [Range( 0f, 2f )]
    [Tooltip( "Скорость затухания реакции" )]
    protected float decay_speed = 2f;
    public void SetDecaySpeed( float speed ) { decay_speed = speed; }
    
    // Number of controllable interpolators (impact points)
    [SerializeField]
    [Range( 10, 30 )]
    [Tooltip( "Количество интерполяций: по умолчанию для мобильной платформы = 6, для стандартной = 24" )]
    protected int interpolators = 24;
    public void SetInterpolations( int amount ) { interpolators = amount; }

    [SerializeField]
    [Tooltip( "Температурный коэффициент для определения текущей температуры нагретого тела; по умолчанию = 1100 градусов по Цельсию" )]
    [Range( 0f, 10000f )]
    private float t_rate = 1000f;
    public void SetTemperatureRate( float rate ) { t_rate = rate; }
    public float Body_temperature { get { float t = 0f; for( int i = 0; i < interpolators; i++ ) t += shaderPos[i].w; return (t * t_rate); } }
    public void CoolBody() { for( int i = 0; i < interpolators; i++ ) shaderPos[i].w = 0f; }
    
    // Unique shader propIDs (see http://docs.unity3d.com/ScriptReference/Shader.PropertyToID.html)
    // Used to access shader variables with int id instead of string name
    private int[] shaderPosID, shaderPowID;    

    // Data containing xyz coordinate of impact and alpha in w for each interpolator    
    private Vector4[] shaderPos;   

    // Current active interpolator
    private int curProp = 0;
    // Timer used to advance trough interpolators
    private float curTime = 0f;

    public void AssignMaterial( Material mat ) { hit_material = mat; }
    public void AssignMeshFilter( MeshFilter mesh_filter ) { hit_mesh_filter = mesh_filter; }

	// INITIALIZATION ##########################################################################################################################################################
	protected virtual void Start () {

        if( transform == null ) Initialization();
    }

    // INITIALIZATION ##########################################################################################################################################################
    public void Initialization() {

        transform = GetComponent<Transform>();
        
        // Generate unique IDs for optimised performance
        // since script has to access them each frame
        shaderPosID = new int[interpolators];
        shaderPowID = new int[interpolators];

        for (int i = 0; i < interpolators; i++)
        {
            shaderPosID[i] = Shader.PropertyToID("_Pos_" + i.ToString());
            shaderPowID[i] = Shader.PropertyToID("_Pow_" + i.ToString());
        }

        // Initialize data array
        shaderPos = new Vector4[interpolators];      
	}

    // Use this method to send impact coordinates from any other script ########################################################################################################
    public void OnHit(Vector3 hitPoint, float hitPower = 0.0f, float hitAlpha = 1.0f) {  

        // Check reaction interval
        if (curTime >= reaction_speed )
        {
            // Hit point coordinates are transformed into local space
            Vector4 newHitPoint = hit_mesh_filter.transform.InverseTransformPoint(hitPoint);

            // Clamp alpha
            newHitPoint.w = Mathf.Clamp(hitAlpha, 0.0f, 1.0f); 

            // Store new hit point data using current counter
            shaderPos[curProp] = newHitPoint;

            // Send hitPower into a shader
            hit_material.SetFloat(shaderPowID[curProp], hitPower);

            // Reset timer and advance counter
            curTime = 0.0f;
            curProp++;
            if (curProp == interpolators) curProp = 0;
        }
    }

    // Called each frame to pass values into a shader ##########################################################################################################################
    private void FadeMask()
    {
        for (int i = 0; i < interpolators; i++)
        {
            if (shaderPos[i].w > 0f)
            {
                // Lerp alpha value for current interpolator
                shaderPos[i].w = Mathf.Lerp(shaderPos[i].w, -0.0001f, Time.deltaTime * decay_speed);
                shaderPos[i].w = Mathf.Clamp(shaderPos[i].w, 0f, 1f);
                // Assign new value to a shader variable
                hit_material.SetVector(shaderPosID[i], shaderPos[i]);
            }
        }
    }
        
	// UPDATE ##################################################################################################################################################################
	protected virtual void Update() {

        // Advance response timer
        curTime += Time.deltaTime;

        // Update shader each frame
        FadeMask();
	}
}
