using System;
using System.Collections.Generic;
using System.Linq;
using GameTools;
using UnityEngine;
using UnityEngine.Rendering;

namespace WaterAndFloating
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Waves : MonoBehaviour
    {
        //variables
        [Header("Parameters"), SerializeField] private int _dimension = 10;
        [SerializeField] private float _UVScale = 2f;
        [SerializeField] private Transform _waterPlacing;

        //mesh
        private MeshFilter _meshFilter;
        private Mesh _mesh;
        private List<Vector3> _vertices = new List<Vector3>();

        private void Start()
        {
            //mesh generation
            _meshFilter = GetComponent<MeshFilter>();
            _mesh = _meshFilter.mesh;
            _mesh.name = gameObject.name;
            _mesh.indexFormat = IndexFormat.UInt32;
            
            _mesh.vertices = GenerateVertices();
            _mesh.triangles = GenerateTriangles();
            _mesh.uv = GenerateUVs();
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
            _mesh.GetVertices(_vertices);
            
            //position & scale
            Transform wavePlacing = _waterPlacing;
            Transform t = transform;
            Vector3 position = wavePlacing.position;
            Vector3 scale = wavePlacing.localScale;
            t.position = position;
            t.localScale = scale;
            _positionDifference = new Vector2(position.x, position.z);
            _scaleDifference = new Vector2(scale.x, scale.z);
            
            for (int i = 0; i < _vertices.Count; i++)
            {
                _indexVerticesDictionary.Add(new Vector2(_vertices[i].x,_vertices[i].z),i);
            }

            _mesh.RecalculateNormals();
        }

        public float GetHeight(Vector3 position)
        {
            Transform waveTransform = transform;
            Vector3 lossyScale = waveTransform.lossyScale;
            
            //scale factor and position in local space
            Vector3 scale = new Vector3(1 / lossyScale.x, 0, 1 / lossyScale.z);
            Vector3 localPos = Vector3.Scale((position - waveTransform.position), scale);

            //get edge points
            Vector3 p1 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Floor(localPos.z));
            Vector3 p2 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Ceil(localPos.z));
            Vector3 p3 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Floor(localPos.z));
            Vector3 p4 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Ceil(localPos.z));

            //clamp if the position is outside the plane
            p1.x = Mathf.Clamp(p1.x, 0, _dimension);
            p1.z = Mathf.Clamp(p1.z, 0, _dimension);
            p2.x = Mathf.Clamp(p2.x, 0, _dimension);
            p2.z = Mathf.Clamp(p2.z, 0, _dimension);
            p3.x = Mathf.Clamp(p3.x, 0, _dimension);
            p3.z = Mathf.Clamp(p3.z, 0, _dimension);
            p4.x = Mathf.Clamp(p4.x, 0, _dimension);
            p4.z = Mathf.Clamp(p4.z, 0, _dimension);

            //get the max distance to one of the edges and take that to compute max - dist
            float max = Mathf.Max(Vector3.Distance(p1, localPos), Vector3.Distance(p2, localPos),
                Vector3.Distance(p3, localPos), Vector3.Distance(p4, localPos) + Mathf.Epsilon);
            float dist = (max - Vector3.Distance(p1, localPos))
                         + (max - Vector3.Distance(p2, localPos))
                         + (max - Vector3.Distance(p3, localPos))
                         + (max - Vector3.Distance(p4, localPos) + Mathf.Epsilon);
            
            //weighted sum
            float height = _vertices[Index(p1.x, p1.z)].y * (max - Vector3.Distance(p1, localPos))
                           + _vertices[Index(p2.x, p2.z)].y * (max - Vector3.Distance(p2, localPos))
                           + _vertices[Index(p3.x, p3.z)].y * (max - Vector3.Distance(p3, localPos))
                           + _vertices[Index(p4.x, p4.z)].y * (max - Vector3.Distance(p4, localPos));

            //scale
            return height * lossyScale.y / dist;
        }

        #region Mesh Generation

        private Vector3[] GenerateVertices()
        {
            Vector3[] vertices = new Vector3[(_dimension + 1) * (_dimension + 1)];

            //equally distribute verts
            for (int x = 0; x <= _dimension; x++)
            {
                for (int z = 0; z <= _dimension; z++)
                {
                    vertices[Index(x, z)] = new Vector3(x, 0, z);
                }
            }

            return vertices;
        }

        private int[] GenerateTriangles()
        {
            int[] triangles = new int[_mesh.vertices.Length * 6];

            //two triangles are one tile
            for (int x = 0; x < _dimension; x++)
            {
                for (int z = 0; z < _dimension; z++)
                {
                    //(0,0) -> (1,1) -> (1,0) -> (0,0) -> (0,1) -> (1,1)
                    triangles[Index(x, z) * 6 + 0] = Index(x, z); 
                    triangles[Index(x, z) * 6 + 1] = Index(x + 1, z + 1); 
                    triangles[Index(x, z) * 6 + 2] = Index(x + 1, z); 
                    triangles[Index(x, z) * 6 + 3] = Index(x, z); 
                    triangles[Index(x, z) * 6 + 4] = Index(x, z + 1); 
                    triangles[Index(x, z) * 6 + 5] = Index(x + 1, z + 1);
                }
            }

            return triangles;
        }

        private Vector2[] GenerateUVs()
        {
            Vector2[] uvs = new Vector2[_mesh.vertices.Length];

            //always set one uv over n tiles than flip the uv and set it again
            for (int x = 0; x <= _dimension; x++)
            {
                for (int z = 0; z <= _dimension; z++)
                {
                    Vector2 vector = new Vector2((x / _UVScale) % 2, (z / _UVScale) % 2);
                    uvs[Index(x, z)] = new Vector2(vector.x <= 1 ? vector.x : 2 - vector.x, vector.y <= 1 ? vector.y : 2 - vector.y);
                }
            }

            return uvs;
        }

        #endregion

        #region Index

        /// <summary>
        /// Get the index of a vertex based on it's position in the mesh triangles array. 
        /// </summary>
        /// <param name="x">the x position of vertex</param>
        /// <param name="z">the z position of vertex</param>
        /// <returns>return the index of the position in the Mesh.triangles int[] array</returns>
        private int Index(float x, float z)
        {
            int index = (int)x * (_dimension + 1) + (int)z;
            return index;
        }

        #endregion
        

        #region Tools

        private Vector2 _positionDifference;
        private Vector2 _scaleDifference;
        private Dictionary<Vector2, int> _indexVerticesDictionary = new Dictionary<Vector2, int>();
        
        private int FindIndexOfVerticeAt(Vector2 worldPosition, bool applyWaveTransform)
        {
            Vector2 rounded = new Vector2(Mathf.Round(worldPosition.x), Mathf.Round(worldPosition.y));
            if (applyWaveTransform)
            {
                rounded = new Vector2(Mathf.Round((worldPosition.x-_positionDifference.x)/_scaleDifference.x), 
                    Mathf.Round((worldPosition.y-_positionDifference.y)/_scaleDifference.y));
            }
            int closestIndex = _indexVerticesDictionary[rounded];
        
            return closestIndex;
        }
    
        

        #endregion
    }

    [Serializable]
    public struct CircularWave
    {
        [Tooltip("The center of the wave, the point where it start")]
        public Vector2 Center { get; set; }

        [Tooltip("The duration of the wave in seconds")]
        public float Duration;

        [Tooltip("The height of the waves")]
        public float Amplitude;
        public float CurrentAmplitude { get; set; }

        [Tooltip("The distance it runs")]
        public float Distance;

        [Tooltip("Number of circular vertices points the wave will manage")]
        public int NumberOfPoints;
    }
    
    [Serializable]
    public struct LinearWave
    {
        [ReadOnly] public Vector2 Center;
        public float Distance;
        public float StartWidth;
        public float EndWidth;
        public float Amplitude;
        [Range(0,360)] public float Angle;
        public float Duration;
        public AnimationCurve AmplitudeCurve;
        public int NumberOfPoints;
    }

    [Serializable]
    public struct Octave
    {
        public Vector2 Speed;
        public Vector2 Scale;
        public float Height;
    }
}