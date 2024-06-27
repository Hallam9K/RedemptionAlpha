using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Natural
{
    public abstract class XenomiteCrystalTileBase : ModTile
    {
        public override string Texture => "Redemption/Tiles/Natural/XenomiteCrystalTile";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.addTile(Type);

            DustType = DustID.GreenTorch;
            RegisterItemDrop(ModContent.ItemType<XenomiteShard>());
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(54, 193, 59), name);
            HitSound = SoundID.Item27;
        }
        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if ((i % 10) < 4)
                spriteEffects = SpriteEffects.FlipHorizontally;
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0f;
            g = 0.3f;
            b = 0f;
        }
    }
    public class XenomiteCrystalTileFake : XenomiteCrystalTileBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<XenomiteShard>(), Type, 0, 1, 2, 3);
        }
    }
    public class XenomiteCrystalTile : XenomiteCrystalTileBase
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            TileObjectData.GetTileData(Type, 0).LavaDeath = false;
        }
    }
}