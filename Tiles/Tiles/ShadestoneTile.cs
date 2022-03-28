using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Dusts;
using Redemption.Items.Placeable.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts.Tiles;
using Terraria.DataStructures;
using Redemption.Biomes;

namespace Redemption.Tiles.Tiles
{
    public class ShadestoneTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneRubbleTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneSlabTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneMossyTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<ShadestoneBrickMossyTile>()] = true;
            DustType = ModContent.DustType<ShadestoneDust>();
            ItemDrop = ModContent.ItemType<Shadestone>();
            MinPick = 350;
            MineResist = 11f;
            SoundType = SoundID.Tink;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Shadestone");
            AddMapEntry(new Color(30, 30, 30));
        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.rand.NextBool(40000) && Main.LocalPlayer.InModBiome(ModContent.GetInstance<SoullessBiome>()))
                Dust.NewDust(new Vector2(i * 16, j * 16), 0, 0, ModContent.DustType<SoullessScreenDust>());
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}