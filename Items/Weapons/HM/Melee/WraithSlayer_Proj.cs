using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.NPCs.Friendly;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class WraithSlayer_Proj : TrueMeleeProjectile
    {
        public float[] oldrot = new float[8];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wraith Slayer");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjArcane[Type] = true;
            ElementID.ProjShadow[Type] = true;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Length = 78;
            Rot = MathHelper.ToRadians(3);
            Projectile.usesLocalNPCImmunity = true;
        }
        private Vector2 startVector;
        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public float Timer;
        private float speed;
        private float SwingSpeed;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.Center = player.MountedCenter + vector;

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver4 - (Projectile.ai[0] == 0 ? 0 : MathHelper.PiOver2);
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4 + (Projectile.ai[0] == 0 ? 0 : MathHelper.PiOver2);

            if (Main.myPlayer == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ == 0)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.Swing1, player.position);
                            startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                            speed = MathHelper.ToRadians(10);
                        }
                        if (Timer < 5 * SwingSpeed)
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.14f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot += speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.8f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 10 * SwingSpeed)
                        {
                            Projectile.alpha = 255;
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            Projectile.ai[0]++;
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 1:
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                        if (Timer++ < 4 * SwingSpeed)
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.23f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.6f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 18 * SwingSpeed)
                            Projectile.Kill();
                        break;
                }
            }
            if (Timer > 1)
                Projectile.alpha = 0;
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Spirit.Contains(target.type))
                modifiers.FinalDamage *= 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);

            if (target.life <= 0 && target.lifeMax >= 50 && (Main.rand.NextBool(6) || NPCLists.Spirit.Contains(target.type)) && NPC.CountNPCS(ModContent.NPCType<WraithSlayer_Samurai>()) < 4)
            {
                for (int i = 0; i < 20; i++)
                    Dust.NewDust(target.position, target.width, target.height, DustID.Wraith);

                Player player = Main.player[Projectile.owner];
                RedeHelper.SpawnNPC(Projectile.GetSource_FromThis(), (int)target.Center.X, (int)target.Center.Y, ModContent.NPCType<WraithSlayer_Samurai>(), ai3: player.whoAmI);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteEffects spriteEffects2 = Projectile.ai[0] == 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Vector2 v = RedeHelper.PolarVector(20, (Projectile.Center - player.Center).ToRotation());

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.MediumPurple * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos - v, null, color * (Timer / 10), oldrot[k], origin, Projectile.scale, spriteEffects | spriteEffects2, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects | spriteEffects2, 0);
            return false;
        }
    }
}
