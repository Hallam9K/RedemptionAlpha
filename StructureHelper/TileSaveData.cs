using System;
using Terraria.ModLoader.IO;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.StructureHelper
{
    public struct TileSaveData : TagSerializable
    {
        public string Tile;
        public string Wall;
        public short FrameX;
        public short FrameY;
        public int WallWireData;
        public short PackedLiquidData;

        public string TEType;
        public TagCompound TEData;

        public bool Active => TileDataPacking.GetBit(WallWireData, 0);

        public TileSaveData(string tile, string wall, short frameX, short frameY, int wallWireData, short packedLiquidData, string teType = "", TagCompound teData = null)
        {

            Tile = tile;
            Wall = wall;
            FrameX = frameX;
            FrameY = frameY;
            WallWireData = wallWireData;    
            PackedLiquidData = packedLiquidData;
            TEType = teType;
            TEData = teData;
        }

        public static Func<TagCompound, TileSaveData> DESERIALIZER = s => DeserializeData(s);

        public static TileSaveData DeserializeData(TagCompound tag)
        {
            var output = new TileSaveData(
            tag.GetString("Tile"),
            tag.GetString("Wall"),
            tag.GetShort("FrameX"),
            tag.GetShort("FrameY"),
            
            tag.GetInt("WallWireData"),
            tag.GetShort("PackedLiquidData")
            );

            if(tag.ContainsKey("TEType"))
			{
                output.TEType = tag.GetString("TEType");
                output.TEData = tag.Get<TagCompound>("TEData");
            }

            return output;
        }

        public TagCompound SerializeData()
        {
            var tag = new TagCompound()
            {
                ["Tile"] = Tile,
                ["Wall"] = Wall,
                ["FrameX"] = FrameX,
                ["FrameY"] = FrameY,

                ["WallWireData"] = WallWireData,
                ["PackedLiquidData"] = PackedLiquidData
            };

            if (TEType != "")
            {
                tag.Add("TEType", TEType);
                tag.Add("TEData", TEData);
            }

            return tag;
        }
    }
}
