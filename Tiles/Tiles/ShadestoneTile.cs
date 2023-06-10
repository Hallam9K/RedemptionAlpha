using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Redemption.Dusts;
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
            Main.tileBrick[Type] = true;
            DustType = ModContent.DustType<ShadestoneDust>();
            MinPick = 350;
            MineResist = 11f;
            HitSound = CustomSounds.StoneHit;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Shadestone");
            AddMapEntry(new Color(30, 30, 30));
        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.rand.NextBool(40000) && Main.LocalPlayer.InModBiome<SoullessBiome>())
                Dust.NewDust(new Vector2(i * 16, j * 16), 0, 0, ModContent.DustType<SoullessScreenDust>());
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}