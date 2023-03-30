using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Base
{
    public class BaseTile
    {
        //------------------------------------------------------//
        //-------------------BASE TILE CLASS--------------------//
        //------------------------------------------------------//
        // Contains methods dealing with tiles, except          //
        // generation. (for that, see BaseWorldGen/BaseGoreGen) //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

		public static void AddMapEntry(ModTile tile, Color color)
		{
			tile.AddMapEntry(color);			
		}		
		
		public static void AddMapEntry(ModTile tile, Color color, string name)
		{
			LocalizedText name2 = tile.CreateMapEntryName();
			// name2.SetDefault(name);
			tile.AddMapEntry(color, name2);			
		}

		public static void SetTileFrame(int x, int y, int tileWidth, int tileHeight, int frame, int tileFrameWidth = 16)
		{
			int type = Main.tile[x, y].TileType;
			int frameWidth = (tileFrameWidth + 2) * tileWidth;
			for (int x1 = 0; x1 < tileWidth; x1++)
			{
				for (int y1 = 0; y1 < tileHeight; y1++)
				{
					int x2 = x + x1; int y2 = y + y1;
					Main.tile[x2, y2].TileFrameX = (short)(frame * frameWidth + (tileFrameWidth + 2) * x1);
				}
			}
		}

		/*
         * Returns all tiles of the given type nearby using the given distance.
		 * 
		 * distance: how far from the x, y coordinates in tiles to check.
		 * addTile : action that can be used to have custom check parameters.
         */
		public static Vector2 GetClosestTile(int x, int y, int type, int distance = 25, Func<Tile, bool> addTile = null)
		{
			Vector2 originalPos = new(x, y);
			int leftX = Math.Max(10, x - distance);
			int leftY = Math.Max(10, y - distance);
			int rightX = Math.Min(Main.maxTilesX - 10, x + distance);
			int rightY = Math.Min(Main.maxTilesY - 10, y + distance);
			Vector2 pos = default;
			float dist = -1;
			for (int x1 = leftX; x1 < rightX; x1++)
			{
				for (int y1 = leftY; y1 < rightY; y1++)
				{
					Tile tile = Framing.GetTileSafely(x1, y1);
					if (tile is {HasTile: true} && tile.TileType == type && (addTile == null || addTile(tile)) && (dist == -1 || Vector2.Distance(originalPos, new Vector2(x1, y1)) < dist))
					{
						dist = Vector2.Distance(originalPos, new Vector2(x1, y1));
                        if (type == 21 || TileObjectData.GetTileData(tile.TileType, 0) != null && (TileObjectData.GetTileData(tile.TileType, 0).Width > 1 || TileObjectData.GetTileData(tile.TileType, 0).Height > 1))
						{
							int x2 = x1; int y2 = y1;
							if (type == 21)
							{
								x2 -= tile.TileFrameX / 18 % 2;
								y2 -= tile.TileFrameY / 18 % 2;
							}else
							{
								Vector2 top = FindTopLeft(x2, y2);
                                x2 = (int)top.X; y2 = (int)top.Y;
							}
							pos = new Vector2(x2, y2);
						}else
						{
							pos = new Vector2(x1, y1);
						}
					}
				}
			}
			return pos;
		}

        public static Point FindTopLeftPoint(int x, int y)
        {
            Vector2 v2 = FindTopLeft(x, y);
            return new Point((int)v2.X, (int)v2.Y);
        }

        public static Vector2 FindTopLeft(int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y); if (tile == null) return new Vector2(x, y);
            TileObjectData data = TileObjectData.GetTileData(tile.TileType, 0);
            x -= tile.TileFrameX / 18 % data.Width;
            y -= tile.TileFrameY / 18 % data.Height;
            return new Vector2(x, y);
        }

		/*
         * Returns all tiles of the given type nearby using the given distance.
		 * 
		 * distance: how far from the x, y coordinates in tiles to check.
		 * addTile : action that can be used to have custom check parameters.
         */
		public static Vector2[] GetTiles(int x, int y, int type, int distance = 25, Func<Tile, bool> addTile = null)
		{
			int leftX = Math.Max(10, x - distance);
			int leftY = Math.Max(10, y - distance);
			int rightX = Math.Min(Main.maxTilesX - 10, x + distance);
			int rightY = Math.Min(Main.maxTilesY - 10, y + distance);
			List<Vector2> tilePos = new();
			for (int x1 = leftX; x1 < rightX; x1++)
			{
				for (int y1 = leftY; y1 < rightY; y1++)
				{
					Tile tile = Framing.GetTileSafely(x1, y1);
					if (tile is {HasTile: true} && tile.TileType == type && (addTile == null || addTile(tile)))
					{
						if (type == 21 || TileObjectData.GetTileData(tile).Width > 1 || TileObjectData.GetTileData(tile).Height > 1)
						{
							int x2 = x1; int y2 = y1;
							if (type == 21)
							{
								x2 -= tile.TileFrameX / 18 % 2;
								y2 -= tile.TileFrameY / 18 % 2;
							}else
							{
								Point p = FindTopLeftPoint(x2, y2); x2 = p.X; y2 = p.Y;
							}
							Vector2 topLeft = new(x2, y2);
							if (tilePos.Contains(topLeft)) { continue; }
							tilePos.Add(topLeft);
						}else
						{
							tilePos.Add(new Vector2(x1, y1));
						}
					}
				}
			}
			return tilePos.ToArray();
		}

		/*
         * Returns the total count of the given liquid within the distance provided.
         */
		public static int LiquidCount(int x, int y, int distance = 25, int liquidType = 0)
		{
			int liquidAmt = 0;
			int leftX = Math.Max(10, x - distance);
			int leftY = Math.Max(10, y - distance);
			int rightX = Math.Min(Main.maxTilesX - 10, x + distance);
			int rightY = Math.Min(Main.maxTilesY - 10, y + distance);
			for (int x1 = leftX; x1 < rightX; x1++)
			{
				for (int y1 = leftY; y1 < rightY; y1++)
				{
					Tile tile = Framing.GetTileSafely(x1, y1);
					if (tile is {LiquidAmount: > 0} && (liquidType == 0 ? tile.LiquidType == 1 : liquidType == 1 ? tile.LiquidType == 2 : tile.LiquidType == 3))
					{
						liquidAmt += tile.LiquidAmount;
					}
				}
			}
			return liquidAmt;
		}

		/*
         * Returns true if the tile type acts similarly to a platform.
         */
		public static bool IsPlatform(int type)
		{
			return Main.tileSolid[type] && Main.tileSolidTop[type];
		}

        public static bool AlchemyFlower(int type) { return type is 82 or 83 or 84; }

        /*
         * Goes through a square area given by the x, y and width, height params, and returns true if they are all of the type given.
         */
        public static bool IsType(int x, int y, int width, int height, int type)
        {
            for(int x1 = x; x1 < x + width; x1++)
                for (int y1 = y; y1 < y + height; y1++)
                {
                    Tile tile = Framing.GetTileSafely(x1, y1);
                    if(tile is not {HasTile: true} || tile.TileType != type)
                    {
                        return false;
                    }
                }
            return true;
        }
        
        /**
         * Return the count of tiles and walls of the given types within the given distance.
         * If a location has a tile and a wall it is only counted once.
         */
        public static int GetTileAndWallCount(Vector2 tileCenter, int[] tileTypes, int[] wallTypes, int distance = 35)
        {
            int tileCount = 0;
            bool addedTile = false;
            for (int x = -distance; x < distance + 1; x++)
            {
                for (int y = -distance; y < distance + 1; y++)
                {
                    int x2 = (int)tileCenter.X + x;
                    int y2 = (int)tileCenter.Y + y;
                    if (x2 < 0 || y2 < 0 || x2 > Main.maxTilesX || y2 > Main.maxTilesY) { continue; }
                    Tile tile = Framing.GetTileSafely(x2, y2);
                    if (tile == null) { continue; }
                    addedTile = false;
                    if (tile.HasTile)
                    {
                        foreach (int i in tileTypes)
                        {
                            if (tile.TileType == i) { tileCount++; addedTile = true; break; }
                        }
                    }
                    if (!addedTile)
                    {
                        foreach (int i in wallTypes)
                        {
                            if (tile.WallType == i) { tileCount++; break; }
                        }
                    }
                    addedTile = false;
                }
            }
            return tileCount;
        }

        /**
         * Return the count of walls of the given types within the given distance.
         */
        public static int GetWallCount(Vector2 tileCenter, int[] wallTypes, int distance = 35)
        {
            int wallCount = 0;
            for (int x = -distance; x < distance + 1; x++)
            {
                for (int y = -distance; y < distance + 1; y++)
                {
                    int x2 = (int)tileCenter.X + x;
                    int y2 = (int)tileCenter.Y + y;
                    if (x2 < 0 || y2 < 0 || x2 > Main.maxTilesX || y2 > Main.maxTilesY) { continue; }
                    Tile tile = Framing.GetTileSafely(x2, y2);
                    if (tile == null) { continue; }
                    foreach (int i in wallTypes)
                    {
                        if (tile.WallType == i) { wallCount++; break; }
                    }
                }
            }
            return wallCount;
        }

        /**
         * Return the count of tiles of the given types within the given distance.
         */
        public static int GetTileCount(Vector2 tileCenter, int[] tileTypes, int distance = 35)
        {
            int tileCount = 0;
            for (int x = -distance; x < distance + 1; x++)
            {
                for (int y = -distance; y < distance + 1; y++)
                {
                    int x2 = (int)tileCenter.X + x;
                    int y2 = (int)tileCenter.Y + y;
                    if (x2 < 0 || y2 < 0 || x2 > Main.maxTilesX || y2 > Main.maxTilesY) { continue; }
                    Tile tile = Framing.GetTileSafely(x2, y2);
                    if (tile is not {HasTile: true}) { continue; }
                    foreach (int i in tileTypes)
                    {
                        if (tile.TileType == i) { tileCount++; break; }
                    }
                }
            }
            return tileCount;
        }
    }
}