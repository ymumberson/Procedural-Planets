using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2,256)]
    public int resolution = 10;
    public bool autoUpdate = true;
    public enum FaceRenderMask { All=0, Top=1, Bottom=2, Left=3, Right=4, Front=5, Back=6 };
    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColourGenerator colourGenerator = new ColourGenerator();

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    private void Initialise()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i=0; i<6; ++i)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);

            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    public void GeneratePlanet()
    {
        Initialise();
        GenerateMesh();
        GenerateColours();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialise();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdates()
    {
        if (autoUpdate)
        {
            Initialise();
            GenerateColours();
        }
    }

    private void GenerateMesh()
    {
        for (int i=0; i<6; ++i)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    public void GenerateColours()
    {
        colourGenerator.UpdateColours();
    }
}
