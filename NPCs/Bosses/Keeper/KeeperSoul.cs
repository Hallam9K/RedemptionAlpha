using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Dusts;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Keeper
{
    public class KeeperSoul : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Keeper");
        }
        public override void SetDefaults()
        {
            Projectile.width = 106;
            Projectile.height = 140;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 400;
        }

        public Vector2 vector;
        public override void AI()
        {
            RedeSystem.Silence = true;
            Player player = Main.player[Projectile.owner];
            if (Projectile.timeLeft < 180)
            {
                for (int k = 0; k < 6; k++)
                {
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    vector.X = (float)(Math.Sin(angle) * 100);
                    vector.Y = (float)(Math.Cos(angle) * 100);
                    Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center + vector, 2, 2, ModContent.DustType<VoidFlame>(), 0f, 0f, 100, default, 3f)];
                    dust2.noGravity = true;
                    dust2.velocity = -Projectile.DirectionTo(dust2.position) * 10f;
                }
                for (int k = 0; k < 2; k++)
                {
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    vector.X = (float)(Math.Sin(angle) * 150);
                    vector.Y = (float)(Math.Cos(angle) * 150);
                    Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center + vector, 2, 2, DustID.AncientLight, 0f, 0f, 100, default, 3f)];
                    dust2.noGravity = true;
                    dust2.velocity = -Projectile.DirectionTo(dust2.position) * 10f;
                }
            }
            if (Projectile.timeLeft == 180)
            {
                player.GetModPlayer<ScreenPlayer>().Rumble(180, 3);
                RedeSystem.Instance.DialogueUIElement.DisplayDialogue("Octavia...", 120, 30, 0.6f, null, 2, Color.DarkGray);
            }
        }
        public override void Kill(int timeleft)
        {
            int dustType = DustID.AncientLight;
            int pieCut = 40;
            for (int m = 0; m < pieCut; m++)
            {
                int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, dustType, 0f, 0f, 100, Color.White, 2f);
                Main.dust[dustID].velocity = BaseUtility.RotateVector(default, new Vector2(6f, 0f), m / (float)pieCut * 6.28f);
                Main.dust[dustID].noLight = false;
                Main.dust[dustID].noGravity = true;
            }
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.WorldData);
        }
    }
}