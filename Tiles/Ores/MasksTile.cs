using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Redemption.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Redemption.Biomes;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Ores
{
    public class MasksTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            DustType = ModContent.DustType<MaskDust>();
            MinPick = 300;
            MineResist = 6f;
            HitSound = CustomSounds.MaskBreak;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Vessel Fragments");
            AddMapEntry(new Color(210, 200, 191), name);
        }
        public override bool CanDrop(int i, int j) => Main.rand.NextBool(8);
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.rand.NextBool(4000) && Main.LocalPlayer.InModBiome<SoullessBiome>())
                Dust.NewDust(new Vector2(i * 16, j * 16), 0, 0, ModContent.DustType<SoullessScreenDust>());
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}