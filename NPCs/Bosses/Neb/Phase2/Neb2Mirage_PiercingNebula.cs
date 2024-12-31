using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.NPCs.Bosses.Neb.Clone;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb.Phase2
{
    public class Neb2Mirage_PiercingNebula : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/Phase2/Nebuleus2";
        public float[] oldrot = new float[5];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Type] = 9;
        }
        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        private int armFrame;
        private Vector2 shootPos;
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || (npc.type != NPCType<Nebuleus2>() && npc.type != NPCType<Nebuleus2_Clone>()))
            {
                Projectile.alpha += 10;
                Projectile.velocity *= .9f;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
                return;
            }
            Player player = Main.player[npc.target];
            Projectile.LookAtEntity(player);

            switch (Projectile.ai[1])
            {
                default:
                    if (++Projectile.localAI[1] >= 5)
                    {
                        Projectile.localAI[1] = 0;
                        if (++armFrame > 2)
                            armFrame = 2;
                    }
                    Projectile.rotation = Projectile.velocity.X * 0.01f;
                    if (Projectile.ai[2]++ == 0)
                        Projectile.velocity = RedeHelper.Spread(2);
                    if (Projectile.ai[2] % 30 == 0)
                    {
                        Projectile.localAI[0] += Main.rand.NextFloat(-1f, 1f);
                        Projectile.netUpdate = true;
                    }
                    Projectile.alpha -= 5;
                    Projectile.velocity.RotatedBy(Projectile.localAI[0], npc.Center);

                    if (Projectile.alpha <= 0)
                    {
                        shootPos = player.Center + RedeHelper.PolarVector(Main.rand.Next(200, 500), RedeHelper.RandomRotation());
                        Projectile.localAI[0] = 0;
                        Projectile.ai[1] = 1;
                        Projectile.ai[2] = 0;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 1:
                    if (++Projectile.localAI[1] >= 5)
                    {
                        Projectile.localAI[1] = 0;
                        if (++armFrame > 2)
                            armFrame = 2;
                    }
                    Projectile.rotation = Projectile.velocity.X * 0.01f;
                    Projectile.Move(shootPos, 90, 40);
                    if (Projectile.ai[2]++ >= 120 || Projectile.DistanceSQ(shootPos) < 80 * 80)
                    {
                        shootPos = player.Center;
                        Projectile.ai[1] = 2;
                        Projectile.ai[2] = 0;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 2:
                    if (++Projectile.localAI[1] >= 5 && Projectile.ai[2] > 15)
                    {
                        Projectile.localAI[1] = 0;
                        if (++armFrame > 5)
                            armFrame = 5;
                    }
                    Projectile.rotation = Projectile.velocity.X * 0.01f;
                    Projectile.velocity *= .7f;
                    if (Projectile.ai[2] == 10)
                        shootPos = player.Center;
                    if (Projectile.ai[2]++ == 20 && Projectile.owner == Main.myPlayer)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(18, (shootPos - Projectile.Center).ToRotation()), ProjectileType<PNebula1_Tele>(), Projectile.damage, 3, Main.myPlayer);
                    }
                    if (Projectile.ai[2] >= 50)
                    {
                        Projectile.alpha += 10;
                        if (Projectile.alpha >= 255)
                            Projectile.Kill();
                    }
                    break;
            }

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 7)
                    Projectile.frame = 0;
            }
            for (int k = oldrot.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return RedeColor.NebColour * Projectile.Opacity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.LivingRainbowDye);
            int height = texture.Height / 9;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawCenter = new(Projectile.Center.X, Projectile.Center.Y);
            Vector2 drawOrigin = new(texture.Width / 2, height / 2);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            int height2 = Nebuleus2.armsPiercingAni.Value.Height / 6;
            int y2 = height2 * armFrame;

            for (int k = Projectile.oldPos.Length - 1; k >= 0; k -= 1)
            {
                float alpha = 1f - (k + 1) / (float)(Projectile.oldPos.Length + 2);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[k] + new Vector2(Projectile.width / 2, Projectile.height / 2) - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor * (0.5f * alpha) * Projectile.Opacity, oldrot[k], drawOrigin, Projectile.scale, effects, 0);
                Main.EntitySpriteDraw(Nebuleus2.armsPiercingAni.Value, Projectile.oldPos[k] + new Vector2(Projectile.width / 2, Projectile.height / 2) + new Vector2(0, 0) - Main.screenPosition, new Rectangle?(new Rectangle(0, y2, Nebuleus2.armsPiercingAni.Value.Width, height2)), Main.DiscoColor * (0.5f * alpha) * Projectile.Opacity, oldrot[k], drawOrigin, Projectile.scale, effects, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive(true);
            GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

            Main.EntitySpriteDraw(Nebuleus2.wings.Value, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(Nebuleus2.armsPiercingAni.Value, drawCenter + new Vector2(0, 0) - Main.screenPosition, new Rectangle?(new Rectangle(0, y2, Nebuleus2.armsPiercingAni.Value.Width, height2)), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(Nebuleus2.armsPiercingGlow.Value, drawCenter + new Vector2(0, 0) - Main.screenPosition, new Rectangle?(new Rectangle(0, y2, Nebuleus2.armsPiercingAni.Value.Width, height2)), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}