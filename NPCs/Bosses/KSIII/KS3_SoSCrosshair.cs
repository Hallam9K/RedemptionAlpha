using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_SoSCrosshair : ModProjectile
    {
        public override string Texture => "Redemption/Projectiles/Ranged/Hardlight_SoSCrosshair";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crosshair");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.scale = 3;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public Vector2[] crossPos = new Vector2[4];
        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            Projectile.rotation += 0.05f;
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (!host.active || (host.type != ModContent.NPCType<KS3>() && host.type != ModContent.NPCType<KS3_Clone>()))
                Projectile.Kill();

            Player player = Main.player[host.target];
            if (!player.active || player.dead)
                Projectile.Kill();

            Projectile.Center = player.MountedCenter;

            switch (Projectile.ai[1])
            {
                case 0:
                    Projectile.localAI[1]++;
                    Projectile.localAI[0] += 6;
                    for (int i = 0; i < 4; i++)
                        crossPos[i] = Projectile.Center + Vector2.One.RotatedBy(MathHelper.ToRadians((360 / 4 * i) + Projectile.localAI[0])) * (120 - Projectile.localAI[1]);

                    Projectile.scale -= 0.03f;
                    Projectile.scale = MathHelper.Clamp(Projectile.scale, 1, 3);
                    if (Projectile.localAI[1] >= 120)
                    {
                        Projectile.scale = 1;
                        Projectile.localAI[1] = 0;
                        Projectile.ai[1] = 1;
                    }
                    break;
                case 1:
                    for (int i = 0; i < 4; i++)
                        crossPos[i] = Projectile.Center;

                    Projectile.localAI[1]++;
                    if (Projectile.localAI[1] >= 30 && Projectile.localAI[1] % 5 == 0 && Projectile.localAI[1] < 60 && Projectile.owner == Main.myPlayer)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.MissileFire1, player.position);

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(player.Center.X + Main.rand.Next(-200, 201), player.Center.Y - 800), RedeHelper.PolarVector(14, (player.Center - Projectile.Center).ToRotation()), ModContent.ProjectileType<KS3_SoSMissile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, 1);
                    }
                    if (Projectile.localAI[1] >= 60 && !RedeHelper.AnyProjectiles(ModContent.ProjectileType<KS3_SoSMissile>()))
                        Projectile.Kill();
                    break;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 4;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);

            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(texture, crossPos[i] - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White) * 0.25f, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}