using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Enums;

using Vector2d = Mapbox.Utils.Vector2d;

public class ProceduralGenerator : MonoBehaviour
{
    public GameObject rockPrefab;
    public int subdivs = 32;

    private AbstractMap map;
    private HashSet<UnityTile> tiles;
    // Start is called before the first frame update
    void Start()
    {
        map = Object.FindObjectOfType<AbstractMap>();
        map.SetCenterLatitudeLongitude(new Vector2d(60.670680f, 5.595375f));
        map.SetZoom(14);

        tiles = new HashSet<UnityTile>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (UnityTile tile in Object.FindObjectsOfType<UnityTile>())
        {
            if(tile.HeightDataState != TilePropertyState.Loaded) continue;
            if(tiles.Add(tile))
            {
                float size = 100f;
                float exaggeration = 3f;
                float baseScale = size / subdivs;

                tile.OnHeightDataChanged += (t) =>
                {
                    Debug.Log("SDAFSDFA");
                    foreach (Transform child in t.transform)
                    {
                        if (child == t.transform) continue;
                        GameObject.Destroy(child.gameObject);
                    }
                    // GenerateCubes(tile, size, exaggeration, baseScale);
                    Debug.Log(tiles.Remove(t));
                };
                GenerateCubes(tile, size, exaggeration, baseScale);
            }
        }
    }

    private void GenerateCubes(UnityTile tile, float size, float exaggeration, float baseScale)
    {
        for (int i = 0; i < subdivs; i++)
        {
            for (int j = 0; j < subdivs; j++)
            {
                float y = tile.QueryHeightData((float)i / subdivs, (float)j / subdivs);
                if (y < 0.01f) continue;
                Vector2 gradient = new Vector2(
                    tile.QueryHeightData((float)i / subdivs + 1f / subdivs, (float)j / subdivs) -
                    tile.QueryHeightData((float)i / subdivs - 1f / subdivs, (float)j / subdivs)
                    ,
                    tile.QueryHeightData((float)i / subdivs, (float)j / subdivs + 1f / subdivs) -
                    tile.QueryHeightData((float)i / subdivs, (float)j / subdivs - 1f / subdivs)
                );
                // if(gradient.magnitude < 1.5f && y > 2f) continue; // possible optimization
                int cliffs = 1 + (int)(gradient.magnitude / 1.5f);
                float rot = Random.Range(0, 90);
                for (int k = 0; k < cliffs; k++)
                {
                    GameObject cube = Instantiate(rockPrefab, tile.transform.position, tile.transform.rotation);
                    cube.transform.SetParent(tile.transform);
                    cube.transform.localPosition = new Vector3(
                        -size / 2 + (float)i / subdivs * size,
                        y * exaggeration,
                        -size / 2 + (float)j / subdivs * size
                    );
                    cube.transform.localScale = new Vector3(
                        baseScale * 1.5f + k * 0.5f * baseScale,
                        (1f + gradient.magnitude * 2f) * exaggeration / (1 + k),
                        baseScale * 1.5f + k * 0.5f * baseScale
                    );
                    cube.transform.Rotate(0, rot, 0);
                }
            }
        }
    }
}
