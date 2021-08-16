using System;
using Terraria.ModLoader.IO;

namespace Redemption.StructureHelper
{
    public struct TileSaveData : TagSerializable
    {
        public string Tile;
        public string Wall;
        public short FrameX;
        public short FrameY;
        public byte BHeader1;
        public byte BHeader2;
        public byte BHeader3;
        public ushort SHeader;

        public string TEType;
        public TagCompound TEData;

        public bool Active => (SHeader & 0b00100000) == 0b00100000;

        public TileSaveData(string tile, string wall, short frameX, short frameY, byte bHeader1, byte bHeader2, byte bHeader3, ushort sHeader, string teType = "", TagCompound teData = null)
        {

            Tile = tile;
            Wall = wall;
            FrameX = frameX;
            FrameY = frameY;
            BHeader1 = bHeader1;
            BHeader2 = bHeader2;
            BHeader3 = bHeader3;
            SHeader = sHeader;

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
            
            tag.GetByte("BHeader1"),
            tag.GetByte("BHeader2"),
            tag.GetByte("BHeader3"),
            (ushort)tag.GetShort("SHeader")
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

                ["BHeader1"] = BHeader1,
                ["BHeader2"] = BHeader2,
                ["BHeader3"] = BHeader3,
                ["SHeader"] = SHeader
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
