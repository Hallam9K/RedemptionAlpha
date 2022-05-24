using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Projectiles.Misc;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Natural
{
    public class ShadeCyst : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.Origin = new Point16(2, 2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.AncientLight;
            HitSound = SoundID.NPCHit13;
            MineResist = 10;
            AddMapEntry(new Color(151, 147, 161));
            AnimationFrameHeight = 48;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 15)
            {
                frameCounter = 0;
                frame++;
                if (frame > 1)
                    frame = 0;
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            for (int k = 0; k < Main.rand.Next(6, 12); k++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int p = Projectile.NewProjectile(new EntitySource_TileBreak(i, j), new Vector2((i + Main.rand.NextFloat(1, 4)) * 16f, (j + Main.rand.NextFloat(1, 2.5f)) * 16f), RedeHelper.PolarVector(Main.rand.Next(3, 14), Main.rand.NextFloat(0, MathHelper.TwoPi)), ModContent.ProjectileType<Echo_Friendly>(), 300, 0, Main.myPlayer);
                    Main.projectile[p].DamageType = DamageClass.Default;
                    Main.projectile[p].netUpdate2 = true;
                }
            }
        }
    }
    public class ShadeCystItem : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shade Cyst");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<ShadeCyst>();
        }
    }
}