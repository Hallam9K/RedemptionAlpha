using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.StructureHelper
{
    internal static class Saver
    {
        public static void SaveToFile(Rectangle target, string targetPath = null)
        {
            string path = ModLoader.ModPath.Replace("Mods", "SavedStructures");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string thisPath = targetPath ?? Path.Combine(path, "SavedStructure_" + DateTime.Now.ToString("d-M-y----H-m-s-f"));

            Main.NewText("Structure saved as " + thisPath, Color.Yellow);
            FileStream stream = File.Create(thisPath);
            stream.Close();

            var tag = SaveStructure(target);
            TagIO.ToFile(tag, thisPath);
        }

        public static void SaveMultistructureToFile(ref List<TagCompound> toSave, string targetPath = null)
        {
            string path = ModLoader.ModPath.Replace("Mods", "SavedStructures");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string thisPath = targetPath ?? Path.Combine(path, "SavedMultiStructure_" + DateTime.Now.ToString("d-M-y----H-m-s-f"));

            Main.NewText("Structure saved as " + thisPath, Color.Yellow);
            FileStream stream = File.Create(thisPath);
            stream.Close();

            TagCompound tag = new TagCompound();
            tag.Add("Structures", toSave);
            tag.Add("Version", Redemption.Instance.Version.ToString());

            TagIO.ToFile(tag, thisPath);

            toSave.Clear();
        }

        public unsafe static TagCompound SaveStructure(Rectangle target)
        {
            TagCompound tag = new TagCompound();
            tag.Add("Version", Redemption.Instance.Version.ToString());
            tag.Add("Width", target.Width);
            tag.Add("Height", target.Height);

            List<TileSaveData> data = new List<TileSaveData>();
            for (int x = target.X; x <= target.X + target.Width; x++)
            {
                for (int y = target.Y; y <= target.Y + target.Height; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    string tileName;
                    string wallName;
                    string teName;
                    if (tile.TileType >= TileID.Count) tileName = ModContent.GetModTile(tile.TileType).Mod.Name + " " + ModContent.GetModTile(tile.TileType).Name;
                    else tileName = tile.TileType.ToString();
                    if (tile.WallType >= WallID.Count) wallName = ModContent.GetModWall(tile.WallType).Mod.Name + " " + ModContent.GetModWall(tile.WallType).Name;
                    else wallName = tile.WallType.ToString();

                    TileEntity teTarget = null; //grabbing TE data
                    TagCompound entityTag = new TagCompound();

                    if (TileEntity.ByPosition.ContainsKey(new Point16(x, y)))
                        teTarget = TileEntity.ByPosition[new Point16(x, y)];

                    if (teTarget != null)
                    {
                        if (teTarget.type < 2)
                            teName = teTarget.type.ToString();
                        else
                        {
                            ModTileEntity entityTarget = teTarget as ModTileEntity;

                            if (entityTarget != null)
                            {
                                teName = entityTarget.Mod.Name + " " + entityTarget.Name;
                                (teTarget as ModTileEntity).SaveData(entityTag);
                            }
                            else
                                teName = "";
                        }
                    }
                    else
                        teName = "";

                    int wallWireData;
                    short packedLiquidData;

                    fixed (void* ptr = &tile.Get<TileWallWireStateData>())
                    {
                        var intPtr = (int*)(ptr);
                        intPtr++;

                        wallWireData = *intPtr;
                    }

                    fixed (void* ptr = &tile.Get<LiquidData>())
                    {
                        var shortPtr = (short*)ptr;

                        packedLiquidData = *shortPtr;
                    }

                    data.Add(
                        new TileSaveData(
                            tileName,
                            wallName,
                            tile.TileFrameX,
                            tile.TileFrameY,
                            wallWireData,
                            packedLiquidData,
                            teName,
                            entityTag
                            ));
                }
            }

            tag.Add("TileData", data);
            return tag;
        }
    }
}

