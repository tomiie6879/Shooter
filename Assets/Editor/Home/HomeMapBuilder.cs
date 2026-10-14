using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

// Editor-only authoring tool. The saved scene contains real Tilemaps and assets.
public static class HomeMapBuilder
{
    const string Root = "Assets/Art/Home";
    const string ScenePath = "Assets/Scenes/HomeScene.unity";
    static Material material;
    static readonly string[] PropNames = {
        "TreeLarge", "TreeSmall", "FlowerBush", "MossRock",
        "CareHouse", "AncientPortal", "RopeBridge", "Well",
        "MarketStall", "Crate", "Barrel", "Fence",
        "Sign", "Lantern", "Flowers", "SmallIsland"
    };


    [MenuItem("Shooter/Home/Build or refresh floating island map")]
    public static void BuildInteractive()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (!EditorUtility.DisplayDialog("Home map", "Rebuild only the HomeMap_Generated root and its art assets? Manual edits inside that root will be replaced.", "Build", "Cancel")) return;
        Build();
    }

    public static void BuildBatch()
    {
        try { Build(); EditorApplication.Exit(0); }
        catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
    }

    static void Build()
    {
        foreach (string p in new[] { Root + "/Tiles/Terrain", Root + "/Tiles/Ground", Root + "/Tiles/Props", Root + "/Tiles/Sky", Root + "/Palettes", Root + "/Materials", "Assets/Prefabs/Home", "Documentation/HomeMap" })
            Directory.CreateDirectory(p);
        AssetDatabase.Refresh();

        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) throw new Exception("Sprite shader missing");
        material = AssetDatabase.LoadAssetAtPath<Material>(Root + "/Materials/HomeUnlit.mat");
        if (material == null) { material = new Material(shader); AssetDatabase.CreateAsset(material, Root + "/Materials/HomeUnlit.mat"); }

        var terrain = SliceGrid(Root + "/Source/HomeTerrain.png", "Terrain", 64);
        var sky = SliceGrid(Root + "/Source/HomeSky.png", "Sky", 256);
        NormalizeGround();
        var ground = SliceGrid(Root + "/HomeGround64.png", "Ground", 64);
        var props = SliceProps();

        var terrainTiles = MakeTiles(terrain, "Terrain");
        var skyTiles = MakeTiles(sky, "Sky");
        var groundTiles = MakeTiles(ground, "Ground");
        var propTiles = MakeTiles(props, "Props");
        MakePalette("Home_01_TerrainLayout", terrainTiles, 34, 1);
        MakePalette("Home_02_GroundPaths", groundTiles, 4, 1);
        MakePalette("Home_03_Props", propTiles, 4, 7);
        MakePalette("Home_04_SkyClouds", skyTiles, 7, 4);

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject existing = scene.GetRootGameObjects().FirstOrDefault(o => o.name == "HomeMap_Generated");
        if (existing != null) Object.DestroyImmediate(existing);
        var root = new GameObject("HomeMap_Generated");
        var grid = new GameObject("Grid - paint using Home palettes", typeof(Grid));
        grid.transform.SetParent(root.transform);

        var background = Layer(grid.transform, "00_SkyClouds", -100);
        background.transform.localPosition = new Vector3(-21, -10, 0);
        background.transform.localScale = Vector3.one * 1.62f;
        PaintAtlas(background, skyTiles, 7, 4);

        var terrainMap = Layer(grid.transform, "10_IslandTerrain", -20);
        terrainMap.transform.localPosition = new Vector3(-17, -5.5f, 0);
        PaintAtlas(terrainMap, terrainTiles, 34, 1);

        Layer(grid.transform, "20_GroundEdits - optional grass and paths", -10);
        var bridges = Layer(grid.transform, "30_Bridges", 0);
        var decor = Layer(grid.transform, "40_TreesBuildingsProps", 10);
        decor.GetComponent<TilemapRenderer>().mode = TilemapRenderer.Mode.Individual;
        decor.GetComponent<TilemapRenderer>().sortOrder = TilemapRenderer.SortOrder.TopRight;
        var distant = Layer(grid.transform, "05_DistantIslands", -50);

        // Props are tile assets too; each cell stores its placement/scale matrix.
        var names = propTiles.ToDictionary(t => t.name, t => t);
        Place(bridges, names["RopeBridge"], -7.65f, 0.55f, 2.8f);
        Place(bridges, names["RopeBridge"], 5.8f, 0.3f, 2.8f);

        Place(decor, names["TreeLarge"], -13.8f, 2.3f, 5.7f);
        Place(decor, names["AncientPortal"], -12.25f, 0.45f, 4.4f);
        Place(decor, names["TreeSmall"], -9.2f, 2.1f, 2.0f);
        Place(decor, names["Crate"], -13.5f, -0.75f, .8f);
        Place(decor, names["FlowerBush"], -15.25f, -0.55f, 1.8f);
        Place(decor, names["FlowerBush"], -10.0f, -1.0f, 1.55f);
        Place(decor, names["Fence"], -14.4f, -1.15f, 1.8f);
        Place(decor, names["Sign"], -9.0f, 0.85f, .75f);

        Place(decor, names["MossRock"], 1.25f, 2.5f, 1.6f);
        Place(decor, names["FlowerBush"], -3.5f, 2.7f, 1.75f);
        Place(decor, names["FlowerBush"], -.6f, -2.2f, 1.6f);
        Place(decor, names["FlowerBush"], -5.5f, -1.15f, 1.9f);
        Place(decor, names["Flowers"], -3.3f, -.75f, .65f);
        Place(decor, names["Flowers"], 1.8f, -.9f, .6f);
        Place(decor, names["Crate"], -4.8f, -2.0f, .7f);
        Place(decor, names["Sign"], -1.3f, 3.05f, .8f);
        Place(decor, names["Lantern"], 3.4f, 1.0f, .8f);
        Place(decor, names["Sign"], 2.7f, 2.5f, .7f);

        Place(decor, names["CareHouse"], 12.6f, 1.1f, 4.7f);
        Place(decor, names["TreeLarge"], 10.6f, 3.3f, 3.6f);
        Place(decor, names["TreeSmall"], 15.3f, 2.3f, 2.8f);
        Place(decor, names["MarketStall"], 13.6f, -.55f, 1.9f);
        Place(decor, names["Well"], 10.25f, -.35f, 1.15f);
        Place(decor, names["MossRock"], 8.0f, -.85f, 1.55f);
        Place(decor, names["FlowerBush"], 13.8f, -1.85f, 1.7f);
        Place(decor, names["TreeSmall"], 11.8f, -2.0f, 1.7f);
        Place(decor, names["Fence"], 8.65f, 1.35f, 1.6f);
        Place(decor, names["Crate"], 15.4f, .0f, .65f);
        Place(decor, names["Barrel"], 14.8f, -.8f, .6f);
        Place(decor, names["Flowers"], 9.3f, -1.75f, .65f);

        Place(distant, names["SmallIsland"], -18.3f, -4.3f, 2.4f);
        Place(distant, names["SmallIsland"], -8.0f, 7.2f, 2.2f);
        Place(distant, names["SmallIsland"], -5.3f, 8.3f, .95f);
        Place(distant, names["SmallIsland"], 2.8f, 7.7f, 1.15f);
        Place(distant, names["SmallIsland"], 7.1f, -4.8f, 1.55f);
        Place(distant, names["SmallIsland"], 18.2f, -3.8f, 1.5f);
        Place(distant, names["SmallIsland"], -9.1f, -4.0f, .8f);

        var points = new GameObject("GameplayAnchors - positions only").transform;
        points.SetParent(root.transform);
        Anchor(points, "PlayerSpawn", new Vector2(-.5f, .3f));
        Anchor(points, "PortalInteraction", new Vector2(-12.3f, .15f));
        Anchor(points, "CareHouseInteraction", new Vector2(12.5f, .5f));
        Anchor(points, "MarketInteraction", new Vector2(13.5f, -.9f));
        var wander = new GameObject("SoulWanderPoints").transform;
        wander.SetParent(points);
        for (int i = 0; i < 6; i++) Anchor(wander, "Point_" + i, new Vector2(-4 + i * 1.4f, i % 2 == 0 ? .3f : 1.5f));
        AddBoundaries(root.transform);

        Camera camera = scene.GetRootGameObjects().SelectMany(o => o.GetComponentsInChildren<Camera>(true)).FirstOrDefault();
        if (camera == null) camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
        camera.tag = "MainCamera";
        camera.orthographic = true;
        camera.orthographicSize = 11.0f;
        camera.transform.position = new Vector3(0, 1.8f, -30);
        camera.transform.rotation = Quaternion.identity;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(.28f, .66f, .98f);
        camera.transparencySortMode = TransparencySortMode.CustomAxis;
        camera.transparencySortAxis = new Vector3(0, 1, 0);

        PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/Home/MAP_HomeFloatingIslands.prefab");
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Validate();
        Capture(camera);
        Debug.Log("HOME_MAP_BUILD_OK");
    }

    static void Anchor(Transform parent, string name, Vector2 position)
    {
        var go = new GameObject(name); go.transform.SetParent(parent); go.transform.position = position;
    }

    static void AddBoundaries(Transform parent)
    {
        var bounds = new GameObject("WalkBoundary - edit points for gameplay");
        bounds.transform.SetParent(parent);
        var edge = bounds.AddComponent<EdgeCollider2D>();
        edge.points = new[] {
            new Vector2(-16,.4f),new Vector2(-15,2.4f),new Vector2(-13,3.5f),new Vector2(-10,3.0f),
            new Vector2(-8.4f,1.0f),new Vector2(-6.8f,1.0f),new Vector2(-5,2.1f),new Vector2(-3.4f,3.8f),
            new Vector2(0,3.0f),new Vector2(3.3f,1.7f),new Vector2(5.0f,.8f),new Vector2(6.9f,.8f),
            new Vector2(7.6f,2.5f),new Vector2(10,3.5f),new Vector2(14.2f,3.4f),new Vector2(16,1.2f),
            new Vector2(15.7f,-1),new Vector2(13,-2),new Vector2(9,-1.5f),new Vector2(7.2f,-.5f),
            new Vector2(5,-.3f),new Vector2(3,-1.7f),new Vector2(0,-2.8f),new Vector2(-3,-2),
            new Vector2(-5.7f,-1.1f),new Vector2(-6.8f,.1f),new Vector2(-8.4f,.1f),
            new Vector2(-10,-1.6f),new Vector2(-14,-1.3f),new Vector2(-16,.4f)
        };
        edge.edgeRadius = .05f;
    }

    static Tilemap Layer(Transform parent, string name, int order)
    {
        var go = new GameObject(name, typeof(Tilemap), typeof(TilemapRenderer));
        go.transform.SetParent(parent, false);
        var map = go.GetComponent<Tilemap>(); map.tileAnchor = Vector3.zero;
        var r = go.GetComponent<TilemapRenderer>(); r.sharedMaterial = material; r.sortingOrder = order;
        return map;
    }

    static void PaintAtlas(Tilemap map, List<Tile> tiles, int columns, int step)
    {
        // Sprite names encode bottom-up row/column; omit transparent cells safely.
        foreach (var tile in tiles)
        {
            var parts = tile.name.Split('_');
            int x = int.Parse(parts[parts.Length - 2]);
            int y = int.Parse(parts[parts.Length - 1]);
            map.SetTile(new Vector3Int(x * step, y * step, 0), tile);
        }
    }

    static void Place(Tilemap map, Tile tile, float x, float y, float width)
    {
        var cell = new Vector3Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y), 0);
        // Avoid overwriting an occupied cell; matrix preserves the requested world position.
        while (map.HasTile(cell)) cell.x++;
        map.SetTile(cell, tile); map.SetTileFlags(cell, TileFlags.None);
        float scale = width / tile.sprite.bounds.size.x;
        map.SetTransformMatrix(cell, Matrix4x4.TRS(new Vector3(x - cell.x, y - cell.y, 0), Quaternion.identity, Vector3.one * scale));
    }

    static Texture2D Import(string path, bool multiple)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
        var imp = (TextureImporter)AssetImporter.GetAtPath(path);
        imp.textureType = TextureImporterType.Sprite;
        imp.spriteImportMode = multiple ? SpriteImportMode.Multiple : SpriteImportMode.Single;
        imp.spritePixelsPerUnit = 64;
        imp.filterMode = FilterMode.Point;
        imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.mipmapEnabled = false; imp.alphaIsTransparency = true; imp.isReadable = true;
        imp.maxTextureSize = 4096; imp.npotScale = TextureImporterNPOTScale.None;
        imp.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    static List<Sprite> SliceGrid(string path, string prefix, int cell)
    {
        var tex = Import(path, true);
        var rects = new List<SpriteMetaData>();
        var pixels = tex.GetPixels32();
        for (int y = 0; y < tex.height; y += cell)
        for (int x = 0; x < tex.width; x += cell)
        {
            int w = Mathf.Min(cell, tex.width - x), h = Mathf.Min(cell, tex.height - y);
            bool visible = false;
            for (int yy = y; yy < y + h && !visible; yy++)
                for (int xx = x; xx < x + w; xx++) if (pixels[yy * tex.width + xx].a > 20) { visible = true; break; }
            if (!visible) continue;
            rects.Add(new SpriteMetaData { name = prefix + "_" + (x/cell).ToString("00") + "_" + (y/cell).ToString("00"), rect = new Rect(x,y,w,h), alignment = 9, pivot = Vector2.zero });
        }
        return ApplyRects(path, rects);
    }

    static List<Sprite> SliceProps()
    {
        string path = Root + "/Source/HomeProps.png";
        var tex = Import(path, true);
        // The generated atlas has variable row heights: use measured rectangles.
        float[,] rect = {
            {0,0,340,374},{345,30,300,344},{647,85,315,289},{972,90,282,280},
            {0,370,342,343},{344,375,302,341},{648,465,345,244},{995,408,259,302},
            {0,710,340,276},{347,734,292,252},{646,737,290,249},{936,735,318,251},
            {40,991,283,263},{361,987,265,267},{655,1005,305,249},{980,990,274,264}
        };
        var list = new List<SpriteMetaData>();
        for (int i = 0; i < 16; i++)
        {
            float sx = tex.width / 1254f, sy = tex.height / 1254f;
            var r = new Rect(Mathf.Round(rect[i,0]*sx), Mathf.Round(tex.height-(rect[i,1]+rect[i,3])*sy), Mathf.Round(rect[i,2]*sx), Mathf.Round(rect[i,3]*sy));
            r.width = Mathf.Min(r.width, tex.width-r.x); r.height = Mathf.Min(r.height, tex.height-r.y);
            list.Add(new SpriteMetaData { name = PropNames[i], rect = r, alignment = 9, pivot = new Vector2(.5f, .05f) });
        }
        return ApplyRects(path,list);
    }

    static List<Sprite> ApplyRects(string path, List<SpriteMetaData> rects)
    {
        var imp = (TextureImporter)AssetImporter.GetAtPath(path);
#pragma warning disable 618
        imp.spritesheet = rects.ToArray();
#pragma warning restore 618
        imp.SaveAndReimport();
        return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s=>s.name).ToList();
    }

    static void NormalizeGround()
    {
        var input = Import(Root + "/Source/HomeGround.png", false);
        var output = new Texture2D(256,256,TextureFormat.RGBA32,false);
        // Nearest-neighbour packing: exactly 16 native 64px cells, avoiding generated gutters.
        for (int y=0;y<256;y++) for(int x=0;x<256;x++)
        {
            int col=x/64,row=y/64;
            float cellW=input.width/4f,cellH=input.height/4f;
            int sx=Mathf.FloorToInt(col*cellW+3+(x%64+.5f)/64f*(cellW-6));
            int sy=Mathf.FloorToInt(row*cellH+3+(y%64+.5f)/64f*(cellH-6));
            output.SetPixel(x,y,input.GetPixel(sx,sy));
        }
        output.Apply(); File.WriteAllBytes(Root+"/HomeGround64.png",output.EncodeToPNG()); Object.DestroyImmediate(output);
        AssetDatabase.ImportAsset(Root+"/HomeGround64.png",ImportAssetOptions.ForceSynchronousImport);
    }

    static List<Tile> MakeTiles(List<Sprite> sprites, string folder)
    {
        var list = new List<Tile>();
        foreach(var sprite in sprites)
        {
            string path=Root+"/Tiles/"+folder+"/"+sprite.name+".asset";
            var tile=AssetDatabase.LoadAssetAtPath<Tile>(path);
            if(tile==null){tile=ScriptableObject.CreateInstance<Tile>();AssetDatabase.CreateAsset(tile,path);}
            tile.name=sprite.name; tile.sprite=sprite; tile.colliderType=Tile.ColliderType.None;
            tile.color=Color.white; tile.transform=Matrix4x4.identity; tile.flags=TileFlags.LockColor;
            EditorUtility.SetDirty(tile); list.Add(tile);
        }
        return list;
    }

    static void MakePalette(string name,List<Tile> tiles,int columns,int step)
    {
        string path=Root+"/Palettes/"+name+".prefab";
        if(!File.Exists(path)) GridPaletteUtility.CreateNewPalette(Root+"/Palettes",name,GridLayout.CellLayout.Rectangle,GridPalette.CellSizing.Manual,Vector3.one,GridLayout.CellSwizzle.XYZ);
        var prefab=PrefabUtility.LoadPrefabContents(path);
        var map=prefab.GetComponentInChildren<Tilemap>();map.ClearAllTiles();map.tileAnchor=Vector3.zero;
        map.GetComponent<TilemapRenderer>().sharedMaterial=material;
        if(name.Contains("Props"))
        {
            for(int i=0;i<tiles.Count;i++) map.SetTile(new Vector3Int(i%columns*step,-i/columns*step,0),tiles[i]);
        }
        else PaintAtlas(map,tiles,columns,step);
        PrefabUtility.SaveAsPrefabAsset(prefab,path);PrefabUtility.UnloadPrefabContents(prefab);
    }

    [MenuItem("Shooter/Home/Validate saved map")]
    public static void Validate()
    {
        var maps=Object.FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        int total=0;
        foreach(var map in maps) foreach(var pos in map.cellBounds.allPositionsWithin)
        {
            if(!map.HasTile(pos))continue;
            if(map.GetSprite(pos)==null)throw new Exception("Missing sprite: "+map.name+" "+pos);
            total++;
        }
        if(total<100)throw new Exception("Home scene not populated");
        var palettes=AssetDatabase.FindAssets("t:Prefab",new[]{Root+"/Palettes"});
        if(palettes.Length!=4)throw new Exception("Expected four Home palettes");
        File.WriteAllText("Documentation/HomeMap/validation.txt","Tilemap cells with valid sprites: "+total+"\nPalettes: "+palettes.Length+"\nScene: "+ScenePath+"\nValidated by Unity Editor; gameplay not implemented by this map builder.\n");
    }

    static void Capture(Camera camera)
    {
        var rt=new RenderTexture(1920,1080,24); camera.targetTexture=rt;
        camera.Render(); RenderTexture.active=rt;
        var tex=new Texture2D(1920,1080,TextureFormat.RGB24,false);
        tex.ReadPixels(new Rect(0,0,1920,1080),0,0);tex.Apply();
        File.WriteAllBytes("Documentation/HomeMap/HomeScene-preview.png",tex.EncodeToPNG());
        camera.targetTexture=null;RenderTexture.active=null;rt.Release();Object.DestroyImmediate(rt);Object.DestroyImmediate(tex);
    }
}
