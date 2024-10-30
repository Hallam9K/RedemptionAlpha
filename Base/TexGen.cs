using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace Redemption.Base
{
    /// <summary> Data structure that stores texture data, for usage alongside <see cref="TexGen"/> </summary>
    public class TexGenData
    {
        public readonly Color[] Data;
        public readonly int Width;
        public readonly int Height;

        public Color this[int index] => Data[index];

        public int Length => Data.Length;

        public TexGenData(Color[] data, int width, int height)
        {
            Data = data;
            Width = width;
            Height = height;
        }

        public static TexGenData FromStream(Stream stream)
        {
            byte[] colorBytes = ImageIO.ReadRaw(stream, out int width, out int height);
            Color[] colors = new Color[width * height];

            for (int i = 0; i < width * height * 4; i += 4)
            {
                colors[i / 4].PackedValue = (uint)(colorBytes[i + 3] << 24 | colorBytes[i + 2] << 16 | colorBytes[i + 1] << 8 | colorBytes[i]);
            }

            return new(colors, width, height);
        }

        public static TexGenData FromTexture2D(Texture2D texture)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            RedeHelper.InvokeOnMainThread(() => texture.GetData(0, texture.Bounds, data, 0, texture.Width * texture.Height));
            return new(data, texture.Width, texture.Height);
        }
    }

    // Code by GroxTheGreat, made functional on servers by Feldy
    public class TexGen
    {
        private class TileInfo
        {
            public int tileID = -1, tileStyle, wallID = -1, objectID;
            public int liquidType = -1, liquidAmt; //liquidType can be 0 (water), 1 (lava), 2 (honey)
            public int slope = -2, wire = -1;

            public TileInfo(int id, int style, int wid = -1, int lType = -1, int lAmt = 0, int sl = -2, int ob = 0, int w = -1)
            {
                tileID = id; tileStyle = style; wallID = wid; liquidType = lType; liquidAmt = lAmt; slope = sl; objectID = ob; wire = w;
            }
        }

        public static Dictionary<Color, int> colorToLiquid;
        public static Dictionary<Color, int> colorToSlope;

        private int width, height;
        private TileInfo[,] tileGen;
        private int torchStyle = 0, platformStyle = 0;

        public TexGen(int w, int h)
        {
            width = w; height = h;
            tileGen = new TileInfo[width, height];
        }

        /// <summary>
        /// Generate the TexGen. (x,y) are top left coordinates of the generation. 
        /// </summary>
        /// <param name="silent"> Play sound on tile place </param>
        /// <param name="sync"> Sync placement </param>
        public void Generate(int x, int y, bool silent, bool sync, GenerationProgress progress = null)
        {
            for (int x1 = 0; x1 < width; x1++)
            {
                progress?.Set(x1 / (float)width);
                for (int y1 = 0; y1 < height; y1++)
                {
                    int x2 = x + x1;
                    int y2 = y + y1;
                    TileInfo info = tileGen[x1, y1];
                    if (info.tileID == -1 && info.wallID == -1 && info.liquidType == -1 && info.wire == -1)
                        continue;

                    if (info.tileID != -1 || info.wallID > -1 || info.wire > -1)
                    {
                        BaseWorldGen.GenerateTile(x2, y2, info.tileID, info.wallID, info.tileStyle != 0 ? info.tileStyle : info.tileID == TileID.Torches ? torchStyle : info.tileID == TileID.Platforms ? platformStyle : 0, info.tileID > -1, info.liquidAmt == 0, info.slope, silent: silent, sync: sync);
                    }
                    if (info.liquidType != -1)
                    {
                        BaseWorldGen.GenerateLiquid(x2, y2, info.liquidType, false, info.liquidAmt, sync);
                    }
                    if (info.objectID != 0)
                    {
                        WorldGen.PlaceObject(x2, y2, info.objectID);
                        NetMessage.SendObjectPlacement(-1, x2, y2, info.objectID, 0, 0, -1, -1);
                    }
                }
            }
        }

        /// <summary> Reads texture data for world generation. Works on dedicated servers. </summary>
        /// <param name="path"> Path to the texture. </param>
        /// <param name="useAsset"> Whether to load the Texture2D data from the asset repository, or directly from IO. Always false on dedicated servers. </param>
        /// <returns></returns>
        public static TexGenData GetTextureForGen(string path, bool useAsset = false)
        {
            if (Main.dedServ)
                useAsset = false;

            if (useAsset)
            {
                var texture = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
                return TexGenData.FromTexture2D(texture);
            }
            else
            {
                path = path.Replace(nameof(Redemption) + "/", "") + ".rawimg";
                return TexGenData.FromStream(Redemption.Instance.GetFileStream(path));
            }
        }

        /// <summary> 
        /// Creates a <see cref="TexGen"/> from <see cref="TexGenData"/>s. 
        /// NOTE: all textures MUST be the same size or horrible things happen! 
        /// </summary>
        public static TexGen GetTexGenerator(TexGenData tileData, Dictionary<Color, int> colorToTile, TexGenData wallData = null, Dictionary<Color, int> colorToWall = null, TexGenData liquidData = null, TexGenData slopeData = null, TexGenData objectData = null, Dictionary<Color, int> colorToObject = null)
        {
            if (colorToLiquid == null)
            {
                colorToLiquid = new Dictionary<Color, int>
                {
                    [new Color(0, 0, 255)] = LiquidID.Water,
                    [new Color(255, 0, 0)] = LiquidID.Lava,
                    [new Color(255, 255, 0)] = LiquidID.Honey,
                    [new Color(255, 0, 255)] = LiquidID.Shimmer
                };

                colorToSlope = new Dictionary<Color, int>
                {
                    [new Color(255, 0, 0)] = 1,   // |\    Red
                    [new Color(0, 255, 0)] = 2,   // /|    Green
                    [new Color(0, 0, 255)] = 3,   // |/    Blue
                    [new Color(255, 255, 0)] = 4, // \|    Yellow
                    [new Color(255, 255, 255)] = -1, // HALFBRICK // White
                    [new Color(0, 0, 0)] = -2 // FULLBLOCK //
                };
            }

            int x = 0, y = 0;
            TexGen gen = new(tileData.Width, tileData.Height);
            for (int m = 0; m < tileData.Length; m++)
            {
                Color tileColor = tileData[m];
                Color wallColor = wallData != null ? wallData[m] : Color.Black;
                Color liquidColor = liquidData != null ? liquidData[m] : Color.Black;
                Color slopeColor = slopeData != null ? slopeData[m] : Color.Black;
                Color objectColor = objectData != null ? objectData[m] : Color.Black;
                int tileID = colorToTile.ContainsKey(tileColor) ? colorToTile[tileColor] : -1; //if no key assume no action
                int wallID = colorToWall != null && colorToWall.ContainsKey(wallColor) ? colorToWall[wallColor] : -1;
                int liquidID = colorToLiquid != null && colorToLiquid.ContainsKey(liquidColor) ? colorToLiquid[liquidColor] : -1;
                int slopeID = colorToSlope != null && colorToSlope.ContainsKey(slopeColor) ? colorToSlope[slopeColor] : -1;
                int objectID = colorToObject != null && colorToObject.ContainsKey(objectColor) ? colorToObject[objectColor] : 0;
                gen.tileGen[x, y] = new TileInfo(tileID, 0, wallID, liquidID, liquidID == -1 ? 0 : 255, slopeID, objectID);
                x++;
                if (x >= tileData.Width) { x = 0; y++; }
                if (y >= tileData.Height) break; //you've somehow reached the end of the texture! (this shouldn't happen!)
            }
            return gen;
        }

        /// <summary> 
        /// Creates a <see cref="TexGen"/> directly from <see cref="Texture2D"/>s. 
        /// Old implementation, does not work on dedicated servers, prefer to use <see cref="GetTexGenerator(TexGenData, Dictionary{Color, int}, TexGenData?, Dictionary{Color, int}, TexGenData?, TexGenData?, TexGenData?, Dictionary{Color, int})"> GetTexGenerator(TexGenData...) </see> instead 
        /// NOTE: all textures MUST be the same size or horrible things happen! 
        /// </summary>
        public static TexGen GetTexGenerator(Texture2D tileTex, Dictionary<Color, int> colorToTile, Texture2D wallTex = null, Dictionary<Color, int> colorToWall = null, Texture2D liquidTex = null, Texture2D slopeTex = null, Texture2D objectTex = null, Dictionary<Color, int> colorToObject = null)
        {
            if (colorToLiquid == null)
            {
                colorToLiquid = new Dictionary<Color, int>
                {
                    [new Color(0, 0, 255)] = LiquidID.Water,
                    [new Color(255, 0, 0)] = LiquidID.Lava,
                    [new Color(255, 255, 0)] = LiquidID.Honey,
                    [new Color(255, 0, 255)] = LiquidID.Shimmer
                };

                colorToSlope = new Dictionary<Color, int>
                {
                    [new Color(255, 0, 0)] = 1,   // |\    Red
                    [new Color(0, 255, 0)] = 2,   // /|    Green
                    [new Color(0, 0, 255)] = 3,   // |/    Blue
                    [new Color(255, 255, 0)] = 4, // \|    Yellow
                    [new Color(255, 255, 255)] = -1, // HALFBRICK // White
                    [new Color(0, 0, 0)] = -2 // FULLBLOCK //
                };
            }
            Color[] tileData = new Color[tileTex.Width * tileTex.Height];
            tileTex.GetData(0, tileTex.Bounds, tileData, 0, tileTex.Width * tileTex.Height);

            Color[] wallData = wallTex != null ? new Color[wallTex.Width * wallTex.Height] : null;
            if (wallData != null)
                wallTex.GetData(0, wallTex.Bounds, wallData, 0, wallTex.Width * wallTex.Height);

            Color[] liquidData = liquidTex != null ? new Color[liquidTex.Width * liquidTex.Height] : null;
            if (liquidData != null)
                liquidTex.GetData(0, liquidTex.Bounds, liquidData, 0, liquidTex.Width * liquidTex.Height);

            Color[] slopeData = slopeTex != null ? new Color[slopeTex.Width * slopeTex.Height] : null;
            if (slopeData != null)
                slopeTex.GetData(0, slopeTex.Bounds, slopeData, 0, slopeTex.Width * slopeTex.Height);

            Color[] objectData = objectTex != null ? new Color[objectTex.Width * objectTex.Height] : null;
            if (objectData != null)
                objectTex.GetData(0, objectTex.Bounds, objectData, 0, objectTex.Width * objectTex.Height);

            int x = 0, y = 0;
            TexGen gen = new(tileTex.Width, tileTex.Height);
            for (int m = 0; m < tileData.Length; m++)
            {
                Color tileColor = tileData[m], wallColor = wallTex == null ? Color.Black : wallData[m], liquidColor = liquidTex == null ? Color.Black : liquidData[m], slopeColor = slopeTex == null ? Color.Black : slopeData[m], objectColor = objectTex == null ? Color.Black : objectData[m];
                int tileID = colorToTile.ContainsKey(tileColor) ? colorToTile[tileColor] : -1; //if no key assume no action
                int wallID = colorToWall != null && colorToWall.ContainsKey(wallColor) ? colorToWall[wallColor] : -1;
                int liquidID = colorToLiquid != null && colorToLiquid.ContainsKey(liquidColor) ? colorToLiquid[liquidColor] : -1;
                int slopeID = colorToSlope != null && colorToSlope.ContainsKey(slopeColor) ? colorToSlope[slopeColor] : -1;
                int objectID = colorToObject != null && colorToObject.ContainsKey(objectColor) ? colorToObject[objectColor] : 0;
                gen.tileGen[x, y] = new TileInfo(tileID, 0, wallID, liquidID, liquidID == -1 ? 0 : 255, slopeID, objectID);
                x++;
                if (x >= tileTex.Width) { x = 0; y++; }
                if (y >= tileTex.Height) break; //you've somehow reached the end of the texture! (this shouldn't happen!)
            }
            return gen;
        }
    }
}