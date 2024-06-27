using Microsoft.Xna.Framework;
using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using Redemption.Items.Materials.PreHM;
using Terraria.DataStructures;

namespace Redemption.Tiles.Natural
{
    public class XenomiteCrystalBigTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = false;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Xenomite Crystals");
            AddMapEntry(new Color(50, 220, 50), name);
            HitSound = SoundID.Item27;
            DustType = DustID.GreenTorch;
            Main.tileLighted[Type] = true;
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
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