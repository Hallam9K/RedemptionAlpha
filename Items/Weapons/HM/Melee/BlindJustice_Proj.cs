using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using Redemption.BaseExtension;
using Redemption.Base;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class BlindJustice_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Melee/BlindJustice";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blind Justice, Demon's Terror");
            ElementID.ProjHoly[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 76;
            Projectile.height = 76;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.ai[0] == 1 ? null : false;
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.fullRotation = 0f;
        }
        float oldRotation = 0f;
        int directionLock = 0;
        private float SwingSpeed;
        public float glow;
        private bool rotRight;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    swordRotation = MathHelper.ToRadians(-45f * player.direction - 90f);

                    glow += 0.025f;
                    glow = MathHelper.Clamp(glow, 0, 0.8f);
                    if (glow >= 0.8 && Projectile.localAI[0] == 0)
                    {
                        RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.2f, 0.85f, 4);
                        RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.2f);
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.NebSound2 with { Pitch = 0.1f }, player.position);
                        Projectile.localAI[0] = 1;
                    }
                    if (!player.channel)
                    {
                        Projectile.ai[0] = 1;
                        oldRotation = swordRotation;
                        directionLock = player.direction;
                        SoundEngine.PlaySound(SoundID.Item71, Projectile.position);
                        if (Projectile.localAI[0] >= 1)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.NebSound1 with { Pitch = 0.1f }, player.position);
                            SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Projectile.position);
                            if (Projectile.owner == Main.myPlayer)
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<BlindJustice_Aura>(), Projectile.damage, 0, Projectile.owner);
                        }
                    }
                }
                else if (Projectile.ai[0] == 1)
                {
                    player.direction = directionLock;
                    Projectile.localAI[1]++;
                    float timer = Projectile.localAI[1] - 1;
                    if (Projectile.localAI[0] >= 1 && timer % (Projectile.localAI[0] == 2 ? 9 : 14) == 0)
                        SoundEngine.PlaySound(SoundID.Item71, Projectile.position);

                    swordRotation = oldRotation.AngleLerp(MathHelper.ToRadians(120f * player.direction - 90f), timer / (Projectile.localAI[0] >= 1 ? (Projectile.localAI[0] == 2 ? 4f : 6f) : 17f) / SwingSpeed);

                    if (Projectile.localAI[1] >= (Projectile.localAI[0] >= 1 ? 48 : 17) * SwingSpeed)
                    {
                        NPC target = null;
                        if (Projectile.localAI[0] == 2 && RedeHelper.ClosestNPC(ref target, 200, player.Center, true))
                        {
                            int hitDirection = target.RightOfDir(Projectile);
                            BaseAI.DamageNPC(target, Projectile.damage * 2, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());

                            SoundEngine.PlaySound(CustomSounds.NebSound3 with { Pitch = .2f, Volume = .6f }, player.Center);
                            player.velocity = target.Center.DirectionTo(player.Center) * 12;
                            player.velocity.Y -= 6;
                            Projectile.alpha = 255;
                            if (target.Center.X < player.Center.X)
                                rotRight = true;
                            Projectile.ai[0] = 2;
                        }
                        else
                            Projectile.Kill();
                    }

                    if (Projectile.localAI[0] >= 1)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile target = Main.projectile[i];
                            if (!target.active || target.whoAmI == Projectile.whoAmI || !target.hostile || target.damage > 200)
                                continue;

                            if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || !target.HasElement(ElementID.Shadow) || target.ProjBlockBlacklist(true))
                                continue;

                            DustHelper.DrawCircle(target.Center, DustID.GoldFlame, 1, 4, 4, nogravity: true);
                            target.Kill();
                        }
                    }
                }
                else
                {
                    if (Projectile.ai[0]++ < 40 && player.velocity.Y != 0)
                    {
                        player.fullRotation += rotRight ? 0.3f : -0.3f;
                        player.fullRotationOrigin = new Vector2(10, 20);
                    }
                    else
                        Projectile.Kill();
                }
            }
            Projectile.velocity = swordRotation.ToRotationVector2();

            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            Projectile.Center = playerCenter + Projectile.velocity * 60f;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (Projectile.ai[0] == 0)
            {
                player.itemRotation = MathHelper.ToRadians(-90f * player.direction);
                player.bodyFrame.Y = 5 * player.bodyFrame.Height;
            }
            else if (Projectile.ai[0] == 1)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.localAI[0] == 2)
                modifiers.FinalDamage *= 2.5f;
            if (Projectile.localAI[0] == 1)
                modifiers.FinalDamage *= 2;
            if (NPCLists.Demon.Contains(target.type))
                modifiers.FinalDamage *= 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            RedeProjectile.Decapitation(target, ref damageDone, ref hit.Crit);
        }
        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition, null, Color.LightBlue * Projectile.Opacity * glow, Projectile.rotation, origin, Projectile.scale, spriteEffects);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
    public class BlindJustice_Proj2 : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Melee/BlindJustice";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blind Justice, Demon's Terror");
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 76;
            Projectile.height = 76;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 40;
        }

        public override bool? CanHitNPC(NPC target) => false;
        private float glow = 1;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            player.Redemption().parryStance = false;
            Projectile.spriteDirection = player.direction;
            if (player.whoAmI == Projectile.owner)
            {
                switch (Projectile.ai[0])
                {
                    case 0:
                        for (int i = 0; i < player.hurtCooldowns.Length; i++)
                        {
                            if (player.hurtCooldowns[i] > 0)
                                Projectile.ai[1] = 100;
                        }
                        if (Projectile.ai[1]++ < 11)
                            player.Redemption().parryStance = true;
                        else if (glow > 0)
                            glow -= 0.3f;

                        if (player.Redemption().parried && Projectile.ai[1] < 100)
                        {
                            player.channel = false;
                            player.immune = true;
                            player.immuneTime = (int)MathHelper.Max(player.immuneTime, 60);
                            glow = 1;
                            Projectile.timeLeft = 40;
                            Projectile.ai[0] = 1;
                            Projectile.ai[1] = 0;
                        }
                        break;
                    case 1:
                        if (Projectile.ai[1]++ == 0)
                        {
                            SoundEngine.PlaySound(CustomSounds.Reflect, player.position);
                            player.RedemptionScreen().ScreenShakeIntensity = 6;
                            RedeDraw.SpawnExplosion(Projectile.Center, Color.White, shakeAmount: 0, scale: 1, noDust: true, tex: ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow2").Value);
                        }
                        if (Projectile.ai[1] == 5)
                        {
                            RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.2f, 0.85f, 4);
                            RedeDraw.SpawnRing(Projectile.Center, new Color(255, 255, 120), 0.2f);
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.NebSound2 with { Pitch = 0.1f }, player.position);
                        }
                        if (Projectile.ai[1] >= 5 && player.controlUseItem)
                        {
                            player.immune = true;
                            player.immuneTime = (int)MathHelper.Max(player.immuneTime, 80);

                            Projectile.Kill();
                            int p = Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), player.Center, player.Center.DirectionTo(Main.MouseWorld) * 20, ModContent.ProjectileType<BlindJustice_Proj>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                            if (Main.projectile[p].ModProjectile is BlindJustice_Proj proj)
                            {
                                Main.projectile[p].localAI[0] = 2;
                                proj.glow = .8f;
                            }
                            player.channel = false;
                        }
                        break;
                }
            }

            Projectile.Center = player.MountedCenter + new Vector2(20 * player.direction, -8);
            Projectile.rotation = 0.4f * player.direction;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
        }

        private float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition, null, Color.LightBlue * Projectile.Opacity * glow, Projectile.rotation, origin, Projectile.scale, spriteEffects);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
