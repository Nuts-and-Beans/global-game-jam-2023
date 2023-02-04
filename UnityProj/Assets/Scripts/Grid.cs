using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

[Serializable]
public struct Array2D<T>
{
    // --- DATA --- //
    [Serializable]
    public struct InternalArray
    {
        public T[] yData;
    }
    
    public InternalArray[] xData;
    
    // --- CTORS --- //
    public Array2D(int2 length)
    {
        xData = new InternalArray[length.x];

        for (int i = 0; i < length.x; i++)
        {
            xData[i].yData = new T[length.y];
        }
    }
    
    public Array2D(int lengthX, int lengthY)
    {
        xData = new InternalArray[lengthX];

        for (int i = 0; i < lengthX; i++)
        {
            xData[i].yData = new T[lengthY];
        }
    }

    // --- INDEXERS --- //
    public T this[int x, int y]
    {
        get => xData[x].yData[y];
        set => xData[x].yData[y] = value;
    }
    
    public T this[int2 i] 
    {
        get => xData[i.x].yData[i.y];
        set => xData[i.x].yData[i.y] = value;
    }
    
    // --- GETTERS --- //
    public int2 Length => new int2 { x = xData == null ? 0 : xData.Length, y = xData == null || xData.Length <= 0 ? 0 : xData[0].yData.Length };
}

public class Grid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int2 _gridCount;
    [SerializeField] private float2 _cellSize;
    [SerializeField] private float _wallHeight = 1.0f;

    [Header("Grid References")]
    [SerializeField] private GameObject _gridFloor;
    [SerializeField] private Transform _wallParent;
    [SerializeField] private GameObject _wallPrefab;
    
    [Header("Camera")]
    [SerializeField] private Camera _camera;
    [SerializeField] private int2 _cellResolution;

    // NOTE(WSWhitehouse): This is hidden in the inspector as a custom inspector is used to draw it as a grid.
    [HideInInspector] public Array2D<bool> initialGridValues = new Array2D<bool>(0,0);
    
    private Array2D<GameObject> _gridCells;
    private float3 _gridStartPosition;
    
    public static RenderTexture s_CameraRT { get; private set; } = null;
    
    private void Awake()
    {
        _gridCells =  new Array2D<GameObject>(_gridCount);
        
        float2 floorScale = ((float2)_gridCount) * _cellSize;
        _gridFloor.transform.localScale = new Vector3(floorScale.x, floorScale.y, 1.0f);
        
        _gridStartPosition    = _gridFloor.transform.position;
        _gridStartPosition.x -= (floorScale.x * 0.5f) - (_cellSize.x * 0.5f);
        _gridStartPosition.y += (floorScale.y * 0.5f) - (_cellSize.y * 0.5f);
        
        // Transform wallSpawnPos = new GameObject("WallSpawnPos").transform;
        // wallSpawnPos.position = _gridStartPosition;

        SpawnWalls();
        InitCameraRT();
    }

    private void OnDestroy()
    {
        if (s_CameraRT == null) return;
        
        _camera.targetTexture = null;
        DestroyImmediate(s_CameraRT);
    }

    private void InitCameraRT()
    {
        if (s_CameraRT != null)
        {
            Debug.LogError("A camera render texture is already assigned! Please ensure there is only one Grid in the scene!");
            return;
        }
        
        int2 fullRes = _cellResolution * _gridCount;
        s_CameraRT   = new RenderTexture(fullRes.x, fullRes.y, 10, GraphicsFormat.R32G32B32A32_SFloat);
        
        _camera.targetTexture    = s_CameraRT;
        _camera.orthographicSize = math.max(_gridCount.x, _gridCount.y) * 0.5f;
    }

    private float3 GetGridPos(int x, int y)
    {
        return _gridStartPosition + new float3(x, -y, 0);
    }

    private void SpawnWalls()
    {
        for (int x = 0; x < _gridCount.x; x++)
        {
            for (int y = 0; y < _gridCount.y; y++)
            {
                if (!initialGridValues[x,y]) continue;
                
                GameObject wall = Instantiate(_wallPrefab, _wallParent);
                
                wall.name               = $"{x}, {y}";
                wall.transform.position = GetGridPos(x, y);
            }
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor
{
    // private const float GridSize = 25f;
    private static readonly Color32 SelectedColor = new Color32(138, 201, 38, 255);
    
    private SerializedProperty _gridCountProperty;
    
    private Grid Target => target as Grid;
    
    private void OnEnable()
    {
        _gridCountProperty = serializedObject.FindProperty("_gridCount");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grid Layout", new GUIStyle("boldLabel"));
        EditorGUILayout.HelpBox("Any highlighted cells are walls!", MessageType.None, true);
        
        InitInitialGridValueArray();
        DrawButtonGrid();
    }

    private void InitInitialGridValueArray()
    {
        int2 gridCount = (int2)_gridCountProperty.boxedValue;

        int2 existingGridSize = Target.initialGridValues.Length;
        bool2 gridSizeEqual   = existingGridSize == gridCount;
        
        if (!gridSizeEqual.x || !gridSizeEqual.y)
        {
            Target.initialGridValues = new Array2D<bool>(gridCount.x, gridCount.y);
        }
    }

    private void DrawButtonGrid()
    {
        Rect lastRect    = GUILayoutUtility.GetLastRect();
        int2 gridCount   = Target.initialGridValues.Length;
        float gridSize   = 25.0f; //lastRect.width / (float)gridCount.x;
        float gridHeight = gridSize * (float)gridCount.y;

        Rect startPos   = GUILayoutUtility.GetRect(lastRect.width, gridHeight);
        Rect gridRect   = startPos;
        gridRect.width  = gridSize;
        gridRect.height = gridSize;
        
        Color standardCol = GUI.color;

        float startX = gridRect.y;
        
        for (int x = 0; x < gridCount.x; x++)
        {
            for (int y = 0; y < gridCount.y; y++)
            {
                if (Target.initialGridValues[x,y]) GUI.color = SelectedColor;

                if (GUI.Button(gridRect, " "))
                {
                    Target.initialGridValues[x,y] = !Target.initialGridValues[x,y];
                }

                GUI.color   = standardCol;
                gridRect.y += gridRect.height;
            }

            gridRect.y  = startX;
            gridRect.x += gridSize;
        }

        gridRect.width  = startPos.width;
        gridRect.height = startPos.height;
    }
}

#endif // UNITY_EDITOR
