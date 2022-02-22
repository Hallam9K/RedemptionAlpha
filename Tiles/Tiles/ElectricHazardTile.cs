using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class ElectricHazardTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = false;
            Main.tileLighted[Type] = true;
            TileID.Sets.IsBeam[Type] = true;
            DustType = DustID.Electric;
            MinPick = 310;
            MineResist = 7f;
            SoundType = SoundID.Tink;
            AddMapEntry(new Color(200, 255, 255));
            AnimationFrameHeight = 90;
            ItemDrop = ModContent.ItemType<ElectricHazard>();
        }
        public override bool IsTileDangerous(int i, int j, Player player) => true;
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 4)
            {
                frameCounter = 0;
                frame++;
                if (frame > 2)
                    frame = 0;
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            float dist = Vector2.Distance(player.Center / 16f, new Vector2(i + 0.5f, j + 0.5f));
            if (dist <= 1)
            {
                player.AddBuff(BuffID.Electrified, 120);
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.3f;
            g = 0.3f;
            b = 0.5f;
        }
        public override bool CanExplode(int i, int j) => false;
    }
}