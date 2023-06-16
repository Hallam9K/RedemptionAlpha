using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using Redemption.Items.Materials.PreHM;
using Terraria.DataStructures;
using Terraria.Enums;
using Redemption.Tiles.Tiles;

namespace Redemption.Tiles.Natural
{
    public class CryoCrystalTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileSpelunker[Type] = true;

            // Attaches to the ground
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorValidTiles = new int[]
            {
                TileID.IceBlock, TileID.SnowBlock, TileID.SnowBrick, TileID.IceBrick, TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce, ModContent.TileType<GathicFroststoneTile>(), ModContent.TileType<GathicFroststoneBrickTile>()
            };

            // Attaches upside down
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.addAlternate(1);

            // Attaches to the left
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.addAlternate(2);

            // Attaches to the right
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.addAlternate(3);
            TileObjectData.addTile(Type);

            HitSound = CustomSounds.CrystalHit;
            DustType = DustID.IceTorch;
            RegisterItemDrop(ModContent.ItemType<GathicCryoCrystal>());
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Gathic Cryo-Crystal");
            AddMapEntry(new Color(159, 188, 215), name);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 6 * 18;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = .1f;
            g = .4f;
            b = .4f;
        }
    }
}