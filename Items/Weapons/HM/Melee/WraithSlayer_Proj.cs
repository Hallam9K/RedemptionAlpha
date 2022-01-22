using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Buffs.NPCBuffs;
using Terraria.Graphics.Shaders;
using Redemption.Projectiles.Melee;
using Redemption.Base;
using Redemption.NPCs.PreHM;
using Redemption.NPCs.Friendly;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class WraithSlayer_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Melee/WraithSlayer";

        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wraith Slayer");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Length = 46;
            Rot = MathHelper.ToRadians(3);
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
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;

            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() - MathHelper.Pi - MathHelper.PiOver4;

            if (Main.myPlayer == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        player.itemRotation = (player.Center - Projectile.Center).ToRotation() * -player.direction;
                        if (Timer++ == 0)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Swing1").WithPitchVariance(0.1f).WithVolume(0.4f), player.position);
                            startVector = RedeHelper.PolarVector(1, Projectile.velocity.ToRotation() - (MathHelper.PiOver2 * Projectile.spriteDirection));
                            speed = MathHelper.ToRadians(3);
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
                        if (Timer >= 16 * SwingSpeed)
                        {
                            SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                            Projectile.ai[0]++;
                            Timer = 0;
                            Projectile.netUpdate = true;
                        }
                        break;
                    case 1:
                        player.itemRotation = (player.Center - Projectile.Center).ToRotation() * -player.direction;
                        if (Timer++ < 5 * SwingSpeed)
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed += 0.14f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        else
                        {
                            Rot -= speed / SwingSpeed * Projectile.spriteDirection;
                            speed *= 0.6f;
                            vector = startVector.RotatedBy(Rot) * Length;
                        }
                        if (Timer >= 10 * SwingSpeed)
                            Projectile.Kill();
                        break;
                }
            }
            if (Timer > 1)
                Projectile.alpha = 0;

            Projectile.Center = player.MountedCenter + vector;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (NPCLists.Spirit.Contains(target.type))
                damage *= 2;

            RedeProjectile.Decapitation(target, ref damage, ref crit);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0 && target.lifeMax >= 50 && (Main.rand.NextBool(6) || NPCLists.Spirit.Contains(target.type)) && NPC.CountNPCS(ModContent.NPCType<WraithSlayer_Samurai>()) < 4)
            {
                for (int i = 0; i < 20; i++)
                    Dust.NewDust(target.position, target.width, target.height, DustID.Wraith);

                Player player = Main.player[Projectile.owner];
                RedeHelper.SpawnNPC((int)target.Center.X, (int)target.Center.Y, ModContent.NPCType<WraithSlayer_Samurai>(), ai3: player.whoAmI);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.MediumPurple * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color * (Timer / 10), oldrot[k], origin, Projectile.scale, spriteEffects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}