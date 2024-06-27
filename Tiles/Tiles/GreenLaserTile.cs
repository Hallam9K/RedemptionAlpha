using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class GreenLaserTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            DustType = DustID.Electric;
            MinPick = 5000;
            MineResist = 3f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(0, 246, 83));
        }
        public override bool Slope(int i, int j) => false;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            RedeTileHelper.SimpleGlowmask(i, j, Color.White, Texture);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.3f;
            b = 0.0f;
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class GreenLaserTileSafe : GreenLaserTile
    {
        public override string Texture => "Redemption/Tiles/Tiles/GreenLaserTile";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            TileID.Sets.DisableSmartCursor[Type] = false;
            MinPick = 200;
        }
    }
}