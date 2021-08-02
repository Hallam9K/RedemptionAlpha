using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ObjectData;
using Terraria.Localization;
using Terraria.Utilities;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace Redemption
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
			ModTranslation name2 = tile.CreateMapEntryName();
			name2.SetDefault(name);
			tile.AddMapEntry(color, name2);			
		}

		public static void SetTileFrame(int x, int y, int tileWidth, int tileHeight, int frame, int tileFrameWidth = 16)
		{
			int type = Main.tile[x, y].type;
			int frameWidth = (tileFrameWidth + 2) * tileWidth;
			for (int x1 = 0; x1 < tileWidth; x1++)
			{
				for (int y1 = 0; y1 < tileHeight; y1++)
				{
					int x2 = x + x1; int y2 = y + y1;
					Main.tile[x2, y2].frameX = (short)((frame * frameWidth) + ((tileFrameWidth + 2) * x1));
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
			Vector2 originalPos = new Vector2(x, y);
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
					Tile tile = Main.tile[x1, y1];
					if (tile != null && tile.IsActive && tile.type == type && (addTile == null || addTile(tile)) && (dist == -1 || Vector2.Distance(originalPos, new Vector2(x1, y1)) < dist))
					{
						dist = Vector2.Distance(originalPos, new Vector2(x1, y1));
                        if (type == 21 || (TileObjectData.GetTileData(tile.type, 0) != null && (TileObjectData.GetTileData(tile.type, 0).Width > 1 || TileObjectData.GetTileData(tile.type, 0).Height > 1)))
						{
							int x2 = x1; int y2 = y1;
							if (type == 21)
							{
								x2 -= tile.frameX / 18 % 2;
								y2 -= tile.frameY / 18 % 2;
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
            Tile tile = Main.tile[x, y]; if (tile == null) return new Vector2(x, y);
            TileObjectData data = TileObjectData.GetTileData(tile.type, 0);
            x -= tile.frameX / 18 % data.Width;
            y -= tile.frameY / 18 % data.Height;
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
			List<Vector2> tilePos = new List<Vector2>();
			for (int x1 = leftX; x1 < rightX; x1++)
			{
				for (int y1 = leftY; y1 < rightY; y1++)
				{
					Tile tile = Main.tile[x1, y1];
					if (tile != null && tile.IsActive && tile.type == type && (addTile == null || addTile(tile)))
					{
						if (type == 21 || TileObjectData.GetTileData(tile).Width > 1 || TileObjectData.GetTileData(tile).Height > 1)
						{
							int x2 = x1; int y2 = y1;
							if (type == 21)
							{
								x2 -= tile.frameX / 18 % 2;
								y2 -= tile.frameY / 18 % 2;
							}else
							{
								Point p = FindTopLeftPoint(x2, y2); x2 = p.X; y2 = p.Y;
							}
							Vector2 topLeft = new Vector2(x2, y2);
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
					Tile tile = Main.tile[x1, y1];
					if (tile != null && tile.LiquidAmount > 0 && (liquidType == 0 ? tile.LiquidType == 1 : liquidType == 1 ? tile.LiquidType == 2 : tile.LiquidType == 3))
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

        public static bool AlchemyFlower(int type) { return type == 82 || type == 83 || type == 84; }

       /*
        * Plays the tile at (tileX, tileY)'s hit sound.
        */       
        public static void PlayTileHitSound(int tileX, int tileY)
        {
            Tile tile = Main.tile[tileX, tileY];
            if(tile != null)
            {
                PlayTileHitSound(tileX * 16, tileY * 16, tile.type);
            }
        }

        /*
         * Plays a specific tile type's hit sound at the given position.
         */
        public static void PlayTileHitSound(float x, float y, int tileType)
        {
			//TODO: FIX
            /*if (TileDef.sound.Length < tileType && TileDef.sound[tileType] > 0)
            {
				int hitSound = TileDef.sound[tileType];
                int list = 0;
				list = TileDef.soundGroup[tileType];
                Main.PlaySound(list, (int)x, (int)y, hitSound);
            }*/
            if (tileType >= 0 && TileLoader.GetTile(tileType) != null)
            {
                ModTile tile = TileLoader.GetTile(tileType);
                SoundEngine.PlaySound(tile.SoundStyle, (int)x, (int)y, tile.SoundType);
            }
            else if (tileType == 127)
                SoundEngine.PlaySound(SoundID.Item, (int)x, (int)y, 27);
            else if (AlchemyFlower(tileType) || tileType == 3 || tileType == 110 || tileType == 24 || tileType == 32 || tileType == 51 || tileType == 52 || tileType == 61 || tileType == 62 || tileType == 69 || tileType == 71 || tileType == 73 || tileType == 74 || tileType == 113 || tileType == 115)
                SoundEngine.PlaySound(SoundID.Grass, (int)x, (int)y, 1);
            else if (tileType == 1 || tileType == 6 || tileType == 7 || tileType == 8 || tileType == 9 || tileType == 22 || tileType == 140 || tileType == 25 || tileType == 37 || tileType == 38 || tileType == 39 || tileType == 41 || tileType == 43 || tileType == 44 || tileType == 45 || tileType == 46 || tileType == 47 || tileType == 48 || tileType == 56 || tileType == 58 || tileType == 63 || tileType == 64 || tileType == 65 || tileType == 66 || tileType == 67 || tileType == 68 || tileType == 75 || tileType == 76 || tileType == 107 || tileType == 108 || tileType == 111 || tileType == 117 || tileType == 118 || tileType == 119 || tileType == 120 || tileType == 121 || tileType == 122)
                SoundEngine.PlaySound(SoundID.Tink, (int)x, (int)y, 1);
            else if (tileType != 138)
                SoundEngine.PlaySound(SoundID.Dig, (int)x, (int)y, 1);
        }

        /*
         * Goes through a square area given by the x, y and width, height params, and returns true if they are all of the type given.
         */
        public static bool IsType(int x, int y, int width, int height, int type)
        {
            for(int x1 = x; x1 < x + width; x1++)
                for (int y1 = y; y1 < y + height; y1++)
                {
                    Tile tile = Main.tile[x1, y1];
                    if(tile == null || !tile.IsActive || tile.type != type)
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
                    Tile tile = Main.tile[x2, y2];
                    if (tile == null) { continue; }
                    addedTile = false;
                    if (tile.IsActive)
                    {
                        foreach (int i in tileTypes)
                        {
                            if (tile.type == i) { tileCount++; addedTile = true; break; }
                        }
                    }
                    if (!addedTile)
                    {
                        foreach (int i in wallTypes)
                        {
                            if (tile.wall == i) { tileCount++; break; }
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
                    Tile tile = Main.tile[x2, y2];
                    if (tile == null) { continue; }
                    foreach (int i in wallTypes)
                    {
                        if (tile.wall == i) { wallCount++; break; }
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
                    Tile tile = Main.tile[x2, y2];
                    if (tile == null || !tile.IsActive) { continue; }
                    foreach (int i in tileTypes)
                    {
                        if (tile.type == i) { tileCount++; break; }
                    }
                }
            }
            return tileCount;
        }
    }
}