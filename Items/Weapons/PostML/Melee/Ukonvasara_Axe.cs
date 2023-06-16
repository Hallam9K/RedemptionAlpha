using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Buffs.Debuffs;
using Redemption.BaseExtension;
using Redemption.Projectiles.Ranged;
using Redemption.Buffs;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class Ukonvasara_Axe : TrueMeleeProjectile
    {
        public float[] oldrot = new float[4];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ukonvasara");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjEarth[Type] = true;
            ElementID.ProjThunder[Type] = true;
        }

        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Redemption().IsAxe = true;
        }

        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.ai[0] >= 1 ? null : false;

        float oldRotation = 0f;
        int directionLock = 0;
        private float SwingSpeed;
        private float swordRotation;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            SwingSpeed = SetSwingSpeed(1);

            player.AddBuff(ModContent.BuffType<HammerBuff>(), 10);
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 0)
                {
                    swordRotation = MathHelper.ToRadians(-45f * player.direction - 90f);

                    Projectile.ai[0] = 1;
                    oldRotation = swordRotation;
                    directionLock = player.direction;
                }
                if (Projectile.ai[0] >= 1)
                {
                    if (Projectile.ai[0] > 4)
                        Projectile.alpha = 0;
                    player.direction = directionLock;

                    Projectile.ai[0]++;
                    Projectile.ai[0] *= 1.15f;

                    float timer = Projectile.ai[0] - 1;
                    if (Projectile.ai[0] > 5)
                        Projectile.alpha = 0;

                    if (Projectile.ai[1] == 0)
                    {
                        if (Projectile.ai[0] < 129 * SwingSpeed)
                            swordRotation = oldRotation.AngleLerp(MathHelper.ToRadians(120f * player.direction - 90f), timer / 140f / SwingSpeed);

                        if (Projectile.ai[0] >= 126 * SwingSpeed)
                        {
                            player.velocity.Y += 6;
                            player.Redemption().contactImmune = true;
                            Point tileBelow = new Vector2(Projectile.Center.X, Projectile.Bottom.Y).ToTileCoordinates();
                            Point tileBelow2 = new Vector2(player.Center.X, player.Bottom.Y).ToTileCoordinates();
                            Tile tile = Framing.GetTileSafely(tileBelow.X, tileBelow.Y);
                            Tile tile2 = Framing.GetTileSafely(tileBelow2.X, tileBelow2.Y);
                            if (((tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType]) || (tile2 is { HasUnactuatedTile: true } && Main.tileSolid[tile2.TileType])) && player.velocity.Y >= 0)
                            {
                                player.immune = true;
                                player.immuneTime = 60;
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.EarthBoom, Projectile.position);
                                for (int i = 0; i < 20; i++)
                                    Dust.NewDust(new Vector2(Projectile.position.X, Projectile.Bottom.Y), Projectile.width, 2, DustID.Stone,
                                        -player.velocity.X * 0.6f, -player.velocity.Y * 0.6f);

                                Main.NewLightning();
                                if (Main.myPlayer == Projectile.owner)
                                {
                                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<UkonArrowStrike>(), Projectile.damage * 3, Projectile.knockBack, Main.myPlayer, 0, 1);
                                    Main.projectile[p].DamageType = DamageClass.Melee;
                                    Main.projectile[p].localAI[0] = 35;
                                    Main.projectile[p].alpha = 0;
                                    Main.projectile[p].position.Y -= 540;
                                    Main.projectile[p].frame = 12;
                                    Main.projectile[p].netUpdate = true;
                                }
                                player.RedemptionScreen().ScreenShakeIntensity += 8;
                                Projectile.ai[1] = 1;
                            }
                            if ((!player.channel || player.velocity.Y <= 0) && Projectile.ai[0] >= 148 && Projectile.ai[1] == 0)
                                Projectile.Kill();
                        }
                    }
                    else
                    {
                        Projectile.friendly = false;
                        if (Projectile.ai[1]++ >= 30)
                            Projectile.Kill();
                    }
                }
            }

            Projectile.velocity = swordRotation.ToRotationVector2();

            Projectile.spriteDirection = player.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.Center = playerCenter + Projectile.velocity * 30f;

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            if (Projectile.ai[0] == 0)
            {
                player.itemRotation = MathHelper.ToRadians(-90f * player.direction);
                player.bodyFrame.Y = 5 * player.bodyFrame.Height;
            }
            else
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = Projectile.rotation;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (player.velocity.Y >= 20 && target.knockBackResist > 0)
                target.AddBuff(ModContent.BuffType<StunnedDebuff>(), 120);

            Projectile.localNPCImmunity[target.whoAmI] = 20;
            target.immune[Projectile.owner] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}