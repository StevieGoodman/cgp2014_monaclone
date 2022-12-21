using UnityEngine;

public class Sight : MonoBehaviour
{
    [Header("FOV Properties")]
    [Range(5, 360)]public float fieldOfView;
    [Range(10, 1000)]public int rayCount;
    [Range(1, 20)]public float viewDistance;
    public LayerMask sightBlockerMask;
    public Material sightMaterial;
    
    private Mesh _mesh;
    private Renderer _renderer;
    private float _startingAngle;
    private Transform _entityBody;
    private Vector3 _origin;
    private AIController _aiController;
    
    private void Awake()
    {
        _entityBody = GetComponentInChildren<Rigidbody2D>().transform;
        _aiController = GetComponent<AIController>();
        

        // Add Mesh Components and configure them.
        _renderer = gameObject.AddComponent<MeshRenderer>();
        _mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>().mesh = _mesh;
        _renderer.material = sightMaterial;
    }

    private void Update()
    {
        SetOrigin(_entityBody.position);
        SetDirection(_entityBody.forward);
    }

    private void LateUpdate()
    {   
        bool playerDetected = false; // AI detection stuff
        
        float angle = _startingAngle;
        float angleIncrement = fieldOfView / rayCount;
        
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = _origin;
        
        int vertexIndex = 1;
        int triIndex = 0;
        for (int i = 0; i <= rayCount; i++) 
        {
            Vector3 vertex;
            // Cast a ray from the origin point of the entity. In the direction they are facing.
            RaycastHit2D rcHit2D = Physics2D.Raycast(_origin, GetVectorFromAngle(angle), viewDistance, sightBlockerMask);
            if (!rcHit2D.collider) // If the raycast didnt hit anything:
            {
                // set this vertex to be as far as our view distance is.
                vertex = _origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {   // We hit something! Set it as the point for this vertex.
                vertex = rcHit2D.point;
            }
            vertices[vertexIndex] = vertex;
            if (i > 0)
            {
                triangles[triIndex + 0] = 0;
                triangles[triIndex + 1] = vertexIndex - 1;
                triangles[triIndex + 2] = vertexIndex;
                triIndex += 3;
            }
            // AI PLAYER DETECTION STUFF //
            if (_aiController && rcHit2D.collider)
                if (rcHit2D.collider.CompareTag("Player"))
                    playerDetected = true;
            // AI PLAYER DETECTION STUFF //
            
            vertexIndex++;
            angle -= angleIncrement;
        }
        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;
        
        // AI Detection Stuff.
        if (_aiController) _aiController.playerDetected = playerDetected;
    }
    
    private void SetOrigin(Vector3 ori) // Set the origin point for when we perform raycasts for the FOV mesh
    {
        _origin = ori;
    }
    
    private void SetDirection(Vector3 aimDirection) // Set the direction of the raycasts for the FOV mesh
    {
        _startingAngle = GetAngleFromVectorFloat(aimDirection, fieldOfView) - fieldOfView / 2f;
    }

    private static Vector3 GetVectorFromAngle(float angle)
    {
        var angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    private static float GetAngleFromVectorFloat(Vector3 dir, float fov)
    {
        dir = dir.normalized;
        var n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return (n + fov);
    }

    public void SetFieldOfViewColour(Color colour)
    {
        var col = new Color(colour.r, colour.g, colour.b, .3f);
        _renderer.material.SetColor("_BaseColor", col);
    }
}
