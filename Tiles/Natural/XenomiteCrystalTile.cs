using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;

namespace Redemption.Tiles.Natural
{
    public class XenomiteCrystalTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = true;
            DustType = DustID.GreenTorch;
            RegisterItemDrop(ModContent.ItemType<XenomiteShard>());
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Xenomite");
            AddMapEntry(new Color(54, 193, 59), name);
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
            TileObjectData.newTile.DrawYOffset = 4;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            HitSound = SoundID.Item27;
            TileObjectData.newTile.AnchorValidTiles = new int[]
            {
                ModContent.TileType<IrradiatedGrassTile>(),
                ModContent.TileType<IrradiatedCorruptGrassTile>(),
                ModContent.TileType<IrradiatedCrimsonGrassTile>(),
                ModContent.TileType<IrradiatedIceTile>()
            };
            TileObjectData.addTile(Type);
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if ((i % 10) < 4)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        
        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 4 * 18;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0f;
            g = 0.3f;
            b = 0f;
        }
        
    }
}