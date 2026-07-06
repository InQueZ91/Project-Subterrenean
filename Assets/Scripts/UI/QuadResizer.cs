using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(MeshFilter))]
    public class QuadResizer : MonoBehaviour
    {
        [SerializeField] private float width = 2f;
        [SerializeField] private float height = 0.2f;

        private void Awake() => SetSize(width, height);
    
        private void OnValidate() => SetSize(width, height); 

        public void SetSize(float w, float h)
        {
            var mesh = GetComponent<MeshFilter>().mesh;
        
            // Rebuild vertices at desired size, UV stays 0-1 untouched
            mesh.vertices = new Vector3[]
            {
                new Vector3(-w / 2, -h / 2, 0),
                new Vector3(w / 2, -h / 2, 0),
                new Vector3(-w / 2, h / 2, 0),
                new Vector3(w / 2, h / 2, 0),
            };
        
            // UVs stay perfectly 0-1 regardless of size
            mesh.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
            };

            mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }
}
