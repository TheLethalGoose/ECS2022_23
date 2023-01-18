using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledCS;

namespace ECS2022_23.Core;

public static class WorldLoader
{
    private static ContentManager _content;
    private static string executingPath;
    
    public static Dictionary<string, TiledMap> Tilemaps =  new();
    public static Dictionary<string, TiledTileset> Tilesets =  new();
    public static Dictionary<string, Texture2D> TilesetTextures = new ();
    
    public static void Load(ContentManager content)
    {
        
        executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (executingPath != null) Directory.SetCurrentDirectory(executingPath);

        if (!Directory.Exists(content.RootDirectory))
        {
            throw new DirectoryNotFoundException();
        }

        _content = content;
        
        LoadTilemaps();
        LoadTilesets();
    }

    public static void Unload(ContentManager content)
    {
        Tilemaps.Clear();
        Tilesets.Clear();
        TilesetTextures.Clear();
    }

    private static void LoadTilemaps()
    {
        LoadRooms();
    }
    private static void LoadTilesets()
    {
        var tilesets = Directory.GetFiles("Content/world/tilesets","tileset???_*",SearchOption.AllDirectories);
        
        foreach (var tileset in tilesets)
        {
            var fileName = Path.GetFileNameWithoutExtension(tileset);

            if (fileName.Contains("_tileset_"))
            {
                Tilesets.Add(fileName,_content.Load<TiledTileset>("world/tilesets/tilesets/" + fileName));
            }
            if (fileName.Contains("_image_"))
            {
                TilesetTextures.Add(fileName,_content.Load<Texture2D>("world/tilesets/images/" + fileName));
            }
            
            //TODO: Make sure tileset picture is there
        }
    }
    private static void LoadRooms()
    {
        var rooms = Directory.GetFiles("Content/world/rooms","room*.xnb");
        var starts = Directory.GetFiles("Content/world/rooms","start*.xnb");
        var bosses = Directory.GetFiles("Content/world/rooms","boss*.xnb");

        foreach (var room in rooms)
        {
            var fileName = Path.GetFileNameWithoutExtension(room);
            Tilemaps.Add(fileName, _content.Load<TiledMap>("world/rooms/" + Path.GetFileNameWithoutExtension(fileName)));
        }
        foreach (var start in starts)
        {
            var fileName = Path.GetFileNameWithoutExtension(start);
            Tilemaps.Add(fileName, _content.Load<TiledMap>("world/rooms/" + Path.GetFileNameWithoutExtension(fileName)));
        }
        foreach (var boss in bosses)
        {
            var fileName = Path.GetFileNameWithoutExtension(boss);
            Tilemaps.Add(fileName, _content.Load<TiledMap>("world/rooms/" + Path.GetFileNameWithoutExtension(fileName)));
        }
        
    }
    
}