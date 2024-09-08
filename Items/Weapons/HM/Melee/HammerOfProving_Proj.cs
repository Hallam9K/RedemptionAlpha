using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class HammerOfProving_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/HM/Melee/HammerOfProving";

        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hammer of Proving");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetSafeDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Redemption().IsHammer = true;
        }

        int directionLock = 0;
        public Vector2 originPos;
        public Player Player => Main.player[Projectile.owner];
        public override void OnSpawn(IEntitySource source)
        {
            HammerOfProving_Player host = Player.GetModPlayer<HammerOfProving_Player>();
            host.onUse = true;
            if (Player.HeldItem.ModItem is HammerOfProving hammer2)
                hammer2.onUse = true;
        }
        public override void OnKill(int timeLeft)
        {
            HammerOfProving_Player host = Player.GetModPlayer<HammerOfProving_Player>();
            host.onUse = false;
            if (Player.HeldItem.ModItem is HammerOfProving hammer2)
                hammer2.onUse = false;
        }
        public override void AI()
        {
            if (Player.noItems || Player.CCed || Player.dead || !Player.active)
                Projectile.Kill();

            Vector2 playerCenter = Player.RotatedRelativePoint(Player.MountedCenter, false);
            float offset = Player.Bottom.Y - playerCenter.Y;

            if (Player.channel)
                Projectile.ai[0]++;
            else
                Projectile.ai[0]--;

            Projectile.ai[0] = MathHelper.Clamp(Projectile.ai[0], -2, 2);

            if (Main.myPlayer == Projectile.owner && Projectile.ai[2] == 0)
            {
                if (Projectile.ai[0] > 1 && Projectile.ai[1] == 0)
                {
                    Projectile.alpha = 0;
                    Player.direction = directionLock;
                }
                if (!Player.channel && Projectile.ai[1] == 0)
                {
                    Projectile.Kill();
                }

                if (Projectile.ai[1] == 0)
                {
                    if (Player.controlUseItem)
                        Player.channel = true;

                    Player.velocity.Y += MathHelper.Lerp(0, 2f, Projectile.ai[0] / 2);
                    if (Player.velocity.Y >= 3 && Player.channel)
                    {
                        Projectile.friendly = true;
                        Point tileBelow = new Vector2(Player.Bottom.X, Player.Bottom.Y).ToTileCoordinates();
                        Tile tile = Framing.GetTileSafely(tileBelow.X, tileBelow.Y);

                        if (Collision.SolidCollision(Player.position + new Vector2(0, Player.height / 2), Player.width, 8 + (Player.height / 2)) || tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType])
                        {
                            float volume = MathHelper.Lerp(0.5f, 1f, Player.velocity.Y / 40);
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.EarthBoom with { Volume = volume }, playerCenter);
                            for (int i = 0; i < 10; i++)
                                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.Bottom.Y), Projectile.width, 2, DustID.Stone,
                                    -Player.velocity.X * 0.6f, -Player.velocity.Y * 0.6f, Scale: 2);

                            Player.RedemptionScreen().ScreenShakeIntensity += 2 * Player.velocity.Y;
                            Projectile.ai[1] = 1;
                            originPos = Player.Center;
                        }
                    }
                    else
                    {
                        Projectile.friendly = false;
                    }

                }
                else
                {
                    if (Player.HeldItem.ModItem is HammerOfProving hammer)
                    {
                        Projectile.friendly = false;
                        if (Projectile.ai[1]++ >= 30)
                        {
                            hammer.pogo = 0;
                            Projectile.Kill();
                        }
                        if (hammer.pogo > 5)
                            hammer.pogo = 5;
                        if (Projectile.ai[1] == 5 * (hammer.pogo) || Projectile.ai[1] == 5 * (hammer.pogo - 1) || Projectile.ai[1] == 5 * (hammer.pogo - 2) || Projectile.ai[1] == 5 * (hammer.pogo - 3) || Projectile.ai[1] == 5 * (hammer.pogo - 4))
                        {
                            for (int i = -1; i <= 1; i += 2)
                            {
                                Vector2 origin = originPos;
                                origin.X += Projectile.ai[1] * 16 * i;
                                int numtries = 0;
                                int x = (int)(origin.X / 16);
                                int y = (int)(origin.Y / 16);
                                while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
                                {
                                    y++;
                                    origin.Y = y * 16;
                                }
                                while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 20)
                                {
                                    numtries++;
                                    y--;
                                    origin.Y = y * 16;
                                }
                                if (numtries >= 20)
                                    break;

                                if (Main.netMode != NetmodeID.Server && Projectile.owner == Main.myPlayer)
                                {
                                    if (Projectile.owner == Main.myPlayer)
                                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), origin + new Vector2(0, -20), new Vector2(0, -30), ModContent.ProjectileType<HolyHammer>(), (int)(Projectile.damage * 0.75f), 2, Player.whoAmI);
                                }
                            }
                        }
                    }
                }
            }

            Projectile.velocity = new Vector2(0, 1);

            Projectile.spriteDirection = Player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            Projectile.Center = playerCenter + Projectile.velocity * 50f;

            Player.heldProj = Projectile.whoAmI;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            if (Projectile.ai[0] == 0)
            {
                Player.itemRotation = MathHelper.ToRadians(-90f * Player.direction);
            }
            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2 - 0.4f);
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Player.velocity.Y > 0)
                modifiers.FinalDamage *= ((Player.velocity.Y / 8) + 1);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Player.HeldItem.ModItem is HammerOfProving hammer)
                hammer.pogo++;

            Player.channel = false;

            float volume = MathHelper.Lerp(0.4f, 0.8f, MathF.Max(Player.velocity.Y / 40, 0));
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.GuardBreak with { Volume = volume, Pitch = -.5f }, Player.position);

            RedeDraw.SpawnExplosion(target.Center * 0.5f + Projectile.Center * 0.5f, Color.LightYellow, scale: 2f, noDust: true, rot: Main.rand.NextFloatDirection(), shakeAmount: 2, tex: ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow2").Value);

            Vector2 directionTo = target.DirectionTo(Player.Center);
            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(target.Center + directionTo * 15 + new Vector2(0, 20) + Player.velocity, ModContent.DustType<DustSpark2>(), directionTo.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f) + 3.14f) * Main.rand.NextFloat(4f, 5f) + (Player.velocity / 10), 0, Color.White * .8f, 1f);

            if (Player.velocity.Y >= 20 && target.knockBackResist > 0)
                target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 180);

            if (Main.myPlayer == Projectile.owner)
            {
                Player.velocity.Y = -15;
            }

            Projectile.localNPCImmunity[target.whoAmI] = 20;
            target.immune[Projectile.owner] = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Projectile.velocity * 30f - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
    public class HammerOfProving_Player : ModPlayer
    {
        public bool onUse;
        public override void PostUpdateRunSpeeds()
        {
            if (onUse)
            {
                Player.accRunSpeed *= 1.5f;
                Player.maxFallSpeed += 10f;
            }
        }
    }
}