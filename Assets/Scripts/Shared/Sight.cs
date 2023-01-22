using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Sight : MonoBehaviour
{
    
    [Header("Performance Settings")]
    // The amount of raycasts used to build the sight mesh.
    [Range(10, 1000)]public int rayCount;
    
    
    [Header("FOV Properties")]
    // How far around the entity it can see.
    [Range(5, 360)]public float fieldOfView;
    
    // How far the entity can see.
    [Range(1, 20)]public float viewDistance;
    
    
    [Header("Sight Extras")]
    // What blocks the sight of this entity.
    public LayerMask sightBlockerMask;
    
    // What material the sight mesh is made of.
    public Material sightMaterial;
    
    // Event that tells other components if this sight component saw something thats tagged.
    public UnityEvent<string> seenTag;

    
    // Private Variables.
    private Mesh _mesh;
    private Renderer _renderer;
    private float _startingAngle;
    [SerializeField]private Transform _entityBody;
    private Vector3 _origin;

    private void Awake()
    {
        // Get the entity body. used as an origin location for raycasts.
        _entityBody = GetComponentInChildren<Rigidbody2D>().transform;
        
        if(!_entityBody)
            _entityBody = GetComponent<Rigidbody2D>().transform;

        // Add Mesh Components and configure them.
        _renderer = gameObject.AddComponent<MeshRenderer>();
        _mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>().mesh = _mesh;
        _renderer.material = sightMaterial;
    }

    private void Update()
    {
        // Set the origin point and direction for the raycasts.
        SetOrigin(_entityBody.position);
        SetDirection(_entityBody.forward);
    }

    private void LateUpdate()
    {
        List<string> tags = new List<string>();
        Vector3 offsetPos = transform.position;
        float angle = _startingAngle;
        float angleIncrement = fieldOfView / rayCount;
        
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = _origin - offsetPos;
        
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
                vertex = _origin - offsetPos + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {   // We hit something! Set it as the point for this vertex.
                vertex = rcHit2D.point - (Vector2)offsetPos;
                
                if (!rcHit2D.collider.CompareTag("Untagged"))
                {
                    if(tags.Count == 0)
                        tags.Add(rcHit2D.collider.tag);
                    // If we saw an object with a tag of interest. 
                    foreach (var item in tags.Where(item => !tags.Contains(rcHit2D.collider.tag)))
                        tags.Add(item);
                }
            }
            vertices[vertexIndex] = vertex;
            if (i > 0)
            {
                triangles[triIndex + 0] = 0;
                triangles[triIndex + 1] = vertexIndex - 1;
                triangles[triIndex + 2] = vertexIndex;
                triIndex += 3;
            }
            
            vertexIndex++;
            angle -= angleIncrement;
        }
        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;
        _mesh.RecalculateBounds();

        foreach (var item in tags) {seenTag?.Invoke(item);}
    }
    // Set the origin point for when we perform raycasts for the FOV mesh
    private void SetOrigin(Vector3 ori) => _origin = ori;

    // Set the direction of the raycasts for the FOV mesh
    private void SetDirection(Vector3 aimDirection) => _startingAngle = GetAngleFromVectorFloat(aimDirection, fieldOfView) - fieldOfView / 2f;

    private Vector3 GetVectorFromAngle(float angle)
    {
        var angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    private float GetAngleFromVectorFloat(Vector3 dir, float fov)
    {
        dir = dir.normalized;
        var n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return (n + fov);
    }
    public void SetFieldOfViewColour(Color colour) => _renderer.material.SetColor("_BaseColor", new Color(colour.r, colour.g, colour.b, .3f));

}
