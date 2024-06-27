using Microsoft.Xna.Framework;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Natural
{
    public abstract class XenomiteCrystalBigTileBase : ModTile
    {
        public override string Texture => "Redemption/Tiles/Natural/XenomiteCrystalBigTile";
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.DrawYOffset = 4;

            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(50, 220, 50), name);
            HitSound = SoundID.Item27;
            DustType = DustID.GreenTorch;
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.4f;
            b = 0.1f;
        }
    }
    public class XenomiteCrystalBigTileFake : XenomiteCrystalBigTileBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<XenomiteShard>(), Type, 0);

            RegisterItemDrop(ModContent.ItemType<XenomiteShard>());
        }
    }
    public class XenomiteCrystalBigTile : XenomiteCrystalBigTileBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            TileObjectData.GetTileData(Type, 0).LavaDeath = false;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.hardMode)
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<Xenomite>(), Main.rand.Next(3, 6));
            else
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<XenomiteShard>(), Main.rand.Next(12, 24));
        }
    }
}