using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Redemption.Biomes;
using Redemption.Items.Materials.PostML;
using Redemption.Dusts.Tiles;
using Redemption.Sounds.Custom;

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
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Vessel Fragments");
            AddMapEntry(new Color(210, 200, 191));
        }
        public override bool Drop(int i, int j)
        {
            if (Main.rand.NextBool(8))
                Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 16, ModContent.ItemType<VesselFragment>());
            return true;
        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.rand.NextBool(4000) && Main.LocalPlayer.InModBiome(ModContent.GetInstance<SoullessBiome>()))
                Dust.NewDust(new Vector2(i * 16, j * 16), 0, 0, ModContent.DustType<SoullessScreenDust>());
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}