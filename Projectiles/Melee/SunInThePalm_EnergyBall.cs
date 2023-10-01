
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Weapons.PostML.Melee;
using Redemption.Particles;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class SunInThePalm_EnergyBall : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Lab/MACE/MACE_FireBlast";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Blast");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjFire[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 140;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.scale = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
        private SlotId rumble;
        private ActiveSound sound;
        public override void AI()
        {
            Projectile proj = Main.projectile[(int)Projectile.ai[0]];
            Player player = Main.player[Projectile.owner];
            Projectile.width = Projectile.height = (int)(140 * Projectile.scale);
            switch (Projectile.localAI[0])
            {
                case 0:
                    if (sound == null)
                        rumble = SoundEngine.PlaySound(CustomSounds.EnergyCharge2 with { Pitch = -.4f }, Projectile.position);
                    Projectile.scale = 0.1f;
                    Projectile.localAI[0] = 1;
                    break;
                case 1:
                    Vector2 Pos = proj.Center + proj.DirectionFrom(player.Center) * 40 * Projectile.scale;
                    Projectile.Center = Pos;
                    Projectile.timeLeft = 10;
                    if (!proj.active || proj.type != ModContent.ProjectileType<SunInThePalm_Proj>())
                        Projectile.Kill();

                    if (Projectile.scale >= 1)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile target = Main.projectile[i];
                            if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile)
                                continue;

                            if (target.damage > 160 / 4 || target.width + target.height > Projectile.width + Projectile.height)
                                continue;

                            if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || target.ProjBlockBlacklist(true))
                                continue;

                            target.Redemption().DissolveTimer++;
                            int d = Dust.NewDust(target.position, target.width, target.height, DustID.RedTorch, Scale: 3);
                            Main.dust[d].noGravity = true;
                            if (target.Redemption().DissolveTimer >= target.damage / 2)
                            {
                                DustHelper.DrawCircle(target.Center, DustID.RedTorch, 1, 4, 4, dustSize: 3, nogravity: true);
                                target.Kill();
                            }
                        }
                    }
                    if (Projectile.scale > 2f)
                    {
                        if (Projectile.localAI[1]++ >= 70)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.MACEProjectLaunch, Projectile.position);
                            player.AddBuff(BuffID.OnFire, 180);
                            Projectile.Kill();
                        }
                    }
                    if (player.channel)
                    {
                        if (Projectile.scale <= 2f)
                        {
                            Projectile.localAI[1] = 0;

                            if (sound == null)
                                rumble = SoundEngine.PlaySound(CustomSounds.EnergyCharge2 with { Pitch = -.4f }, Projectile.position);

                            Projectile.scale += 0.01f;
                            if (Projectile.alpha > 0)
                                Projectile.alpha -= 10;
                        }
                    }
                    else
                    {
                        Projectile.localAI[1] = 0;
                        if (player.controlUseItem)
                            player.channel = true;
                        if (sound != null)
                        {
                            sound.Stop();
                            rumble = SlotId.Invalid;
                        }
                        Projectile.scale -= 0.04f;
                        if (Projectile.scale <= .1f)
                            Projectile.Kill();
                    }
                    break;
            }
            SoundEngine.TryGetActiveSound(rumble, out sound);
            if (sound != null)
                sound.Position = Projectile.position;
            Projectile.rotation += 0.1f;
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.9f, Projectile.Opacity * 0.2f, Projectile.Opacity * 0.2f);
        }
        public override bool? CanHitNPC(NPC target) => Projectile.localAI[0] == 1 ? null : false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 900);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= ((Projectile.scale / 8) + 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.BloodbathDye);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);
            Vector2 drawOrigin = new(texture.Width * 0.5f, texture.Height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(70 * Projectile.scale, 70 * Projectile.scale);
                Color color = Projectile.GetAlpha(Color.White) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/RedEyeFlare").Value;
            Rectangle rect2 = new(0, 0, flare.Width, flare.Height);
            Vector2 origin2 = new(flare.Width / 2, flare.Height / 2);
            if (Projectile.localAI[0] < 2)
            {
                Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, new Rectangle?(rect2), Projectile.GetAlpha(Color.OrangeRed) * 0.6f, Projectile.rotation, origin2, Projectile.scale * 2f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(flare, Projectile.Center - Main.screenPosition, new Rectangle?(rect2), Projectile.GetAlpha(Color.OrangeRed) * 0.6f, -Projectile.rotation, origin2, Projectile.scale * 2f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            if (sound != null)
            {
                sound.Stop();
                rumble = SlotId.Invalid;
            }
            Player player = Main.player[Projectile.owner];
            player.RedemptionScreen().ScreenShakeIntensity += 20 * (Projectile.scale * 2);
            for (int i = 0; i < 40; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), new EmberParticle(), Color.Red, 3 * Projectile.scale, 0, 2);
            for (int i = 0; i < 20; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), new EmberParticle(), Color.Red, 1, 0);
            SoundEngine.PlaySound(CustomSounds.MissileExplosion with { Volume = 1 * Projectile.scale, Pitch = -.5f }, Projectile.position);

            int boomOrigin = (int)(140 * Projectile.scale);
            Rectangle boom = new((int)Projectile.Center.X - boomOrigin, (int)Projectile.Center.Y - boomOrigin, boomOrigin * 2, boomOrigin * 2);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || !target.CanBeChasedBy())
                    continue;

                if (target.immune[Projectile.whoAmI] > 0 || !target.Hitbox.Intersects(boom))
                    continue;

                target.immune[Projectile.whoAmI] = 20;
                int hitDirection = target.RightOfDir(Projectile);
                BaseAI.DamageNPC(target, (int)(Projectile.damage * (Projectile.scale * 1.5f)), 7, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
                BaseAI.DamageNPC(target, (int)(Projectile.damage * (Projectile.scale * 1.25f)), 4, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
            }
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch, Scale: 2);
                Main.dust[dust].velocity *= 6;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
