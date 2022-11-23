using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ECS2022_23.Enums;
using ECS2022_23.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledCS;

namespace MonoGameLevelGenerator.Core;

public class Room
{
    private Dictionary<int, TiledTileset> _tilesets = new();
    
    public TiledMap _map;
    public string roomMapName; 
    public Point _renderPos;
    
    public Point Position
    {
        //TODO fix usage of _renderPos
        get { return _renderPos; }
    }
    
    public Rectangle Rectangle
    {
        get { return new Rectangle(_renderPos.X, _renderPos.Y, _map.Width, _map.Height);  }
    }
    
    public List<Rectangle> GroundLayer
    {
        get
        { 
            var groundLayer = _map.Layers.First(l => l.name == "Ground");
            var collisionLayer = new List<Rectangle>();

            foreach (var obj in groundLayer.objects)
            {
                var x = (int) obj.x + _renderPos.X * _map.TileWidth;
                var y = (int) obj.y + _renderPos.Y * _map.TileHeight;
                
                collisionLayer.Add(new Rectangle(x,y,(int) obj.width,(int) obj.height));
            }

            return collisionLayer;
        }
        
    }
    
    public Room(string mapName, Point renderPos)
    {
        _map = Helper.CreateDeepCopy(ContentLoader.Tilemaps[mapName]);
        _renderPos = renderPos;
        roomMapName = mapName;

        getTiledTilesets();
    }
    
    
    public List<Door> Doors
    {
        get
        {
            var doorsObjects = _map.Layers.First(x => x.name == "Doors").objects;
            var doors = new List<Door>();
            
            foreach (var door in doorsObjects)
            {
                var doorX = (int) (Math.Floor(door.x / _map.TileWidth)) + _renderPos.X;
                var doorY = (int) (Math.Floor(door.y / _map.TileHeight)) + _renderPos.Y;
                var marker = new Point((int) doorX * _map.TileWidth, (int) doorY * _map.TileHeight);
                
                var doorDirection = Enum.Parse<Direction>(door.name);
                
                doors.Add(new Door(this,marker,doorDirection,doorX,doorY));
            }
            
            return doors;
        }
    }

    public int doorCount => Doors.Count;
    private void getTiledTilesets()
    {
        foreach (var mapTileset in _map.Tilesets)
        {
            if (mapTileset.source != null)
            {
                var filename = Path.GetFileNameWithoutExtension(mapTileset.source);
                _tilesets.Add(mapTileset.firstgid, ContentLoader.Tilesets[filename]);
            }
        }
    }

    public void ChangeTile(int x, int y, int newGID, string layerName)
    {
        var layer = _map.Layers.First(layer => layer.name == layerName);
        var index = (y * layer.width) + x;
        layer.data[index] = newGID+1;
    }

    public int getTileGid(int x, int y, string layerName)
    {
        var layer = _map.Layers.First(layer => layer.name == layerName);
        var index = (y * layer.width) + x;

        return layer.data[index];
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        var tileLayers = _map.Layers.Where(x => x.type == TiledLayerType.TileLayer);
        
        foreach (var layer in tileLayers)
        {
            for (var y = 0; y < layer.height; y++)
            {
                for (var x = 0; x < layer.width; x++)
                {
                    var index = (y * layer.width) + x; // Assuming the default render order is used which is from right to bottom
                    var gid = layer.data[index]; // The tileset tile index
                    
                    var tileX = (x + _renderPos.X) * _map.TileWidth;
                    var tileY = (y + _renderPos.Y) * _map.TileHeight;
                    
                    var effects = SpriteEffects.None;
                    float rotation = 0f;
                    
                    if (gid == 0)
                    {
                        continue;
                    }

                    var mapTileset = _map.GetTiledMapTileset(gid);
                    var tileset = _tilesets[mapTileset.firstgid];
                    var tilesetFilename = Path.GetFileNameWithoutExtension(mapTileset.source);
                    
                    var tilesetImageName = tilesetFilename.Replace("_tileset", "_image");
                    var tilesetTexture = ContentLoader.TilesetTextures[tilesetImageName];
                    
                    var rect = _map.GetSourceRect(mapTileset, tileset, gid);
                    var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                    
                    var destination = new Rectangle(tileX, tileY, _map.TileWidth, _map.TileHeight);
                    
                    Trans tileTrans = Trans.None;
                    if (_map.IsTileFlippedHorizontal(layer, x, y)) tileTrans |= Trans.Flip_H;
                    if (_map.IsTileFlippedVertical(layer, x, y)) tileTrans |= Trans.Flip_V;
                    if (_map.IsTileFlippedDiagonal(layer, x, y)) tileTrans |= Trans.Flip_D;
                    
                    switch (tileTrans)
                    {
                        case Trans.Flip_H: effects = SpriteEffects.FlipHorizontally; break;
                        case Trans.Flip_V: effects = SpriteEffects.FlipVertically; break;

                        case Trans.Rotate_90:
                            rotation = (float) Math.PI * .5f;
                            destination.X += _map.TileWidth;
                            break;

                        case Trans.Rotate_180:
                            rotation = (float) Math.PI;
                            destination.X += _map.TileWidth;
                            destination.Y += _map.TileHeight;
                            break;

                        case Trans.Rotate_270:
                            rotation = (float) Math.PI * 3 / 2;
                            destination.Y += _map.TileHeight;
                            break;

                        case Trans.Rotate_90AndFlip_H:
                            effects = SpriteEffects.FlipHorizontally;
                            rotation = (float) Math.PI * .5f;
                            destination.X += _map.TileWidth;
                            break;
                    }
                    
                    spriteBatch.Draw(tilesetTexture, destination, source, Color.White, rotation, Vector2.Zero, effects, 0);
                    
                }
            }
        }
    }
}

[Flags] internal enum Trans
{
    None = 0,
    Flip_H = 1 << 0,
    Flip_V = 1 << 1,
    Flip_D = 1 << 2,

    Rotate_90 = Flip_D | Flip_H,
    Rotate_180 = Flip_H | Flip_V,
    Rotate_270 = Flip_V | Flip_D,

    Rotate_90AndFlip_H = Flip_H | Flip_V | Flip_D,
}