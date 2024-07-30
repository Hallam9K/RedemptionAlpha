using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Tools.PreHM
{
    public class PureIronHammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pure-Iron Hammer");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.attackSpeedOnlyAffectsWeaponAnimation = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 38;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 6;
            Item.useAnimation = 22;
            Item.hammer = 70;
            Item.shootSpeed = 10;
            Item.knockBack = 3;
            Item.value = 1050;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<PureIronHammer_Proj>();
        }
        public override bool MeleePrefix() => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale2 = player.GetAdjustedItemScale(Item);
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PureIronHammer_Proj>(), damage, knockback, player.whoAmI, 0, 0, adjustedItemScale2);
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PureIronAlloy>(), 16)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class PureIronHammer_Proj : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Items/Tools/PreHM/PureIronHammer";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjIce[Type] = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 24;
            Projectile.Redemption().IsHammer = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public Vector2 vector;
        public Vector2 startVector;
        private float SwingSpeed;
        private float Rot;
        private float Length;
        private float progress;
        private bool landed;
        public ref float Timer => ref Projectile.localAI[0];
        private Player Player => Main.player[Projectile.owner];
        public override void AI()
        {
            Player.heldProj = Projectile.whoAmI;
            Player.itemTime = 2;
            Player.itemAnimation = 2;

            SwingSpeed = 1 / Player.GetAttackSpeed(DamageClass.Melee);
            int update = Projectile.extraUpdates + 1;

            if (Player.noItems || Player.CCed || Player.dead || !Player.active)
                Projectile.Kill();

            Vector2 armCenter = Player.RotatedRelativePoint(Player.MountedCenter, true) + new Vector2(-Player.direction * 2, -2);
            Projectile.Center = armCenter + vector;

            Projectile.spriteDirection = Player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = (Projectile.Center - armCenter).ToRotation() + 3 * MathHelper.PiOver4;

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (armCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            progress = Timer / (60 * update * SwingSpeed);
            if (!landed)
            {
                if (Timer++ == 0)
                {
                    Projectile.scale *= Projectile.ai[2];
                    Length = 55 * Projectile.ai[2];
                    startVector = RedeHelper.PolarVector(1, Projectile.spriteDirection * MathHelper.PiOver2 + MathHelper.PiOver2) * Length;
                }
                if (Timer == (int)(30 * update * SwingSpeed))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.Swoosh1 with { Pitch = -.6f }, Player.position);
                }
                if (progress < 0.3f)
                {
                    Rot = -MathHelper.ToRadians(120 + 100f * MathF.Atan(5 * MathHelper.Pi * (progress - 0.5f))) * Projectile.spriteDirection;
                    vector = startVector.RotatedBy(Rot);
                }
                else if (progress < 1f)
                {
                    Projectile.friendly = true;
                    Rot = MathHelper.ToRadians(120 + 100f * MathF.Atan(5 * MathHelper.Pi * (progress - 0.5f))) * Projectile.spriteDirection;
                    vector = startVector.RotatedBy(Rot);
                }
                else
                    Projectile.Kill();

                Point tileBelow = new Vector2(Projectile.Bottom.X, Projectile.Bottom.Y).ToTileCoordinates();
                Tile tile = Framing.GetTileSafely(tileBelow.X, tileBelow.Y);

                if (progress > 0.53f && progress < 0.6f)
                {
                    if (Collision.SolidCollision(Projectile.position + new Vector2(0, Projectile.height / 2), Projectile.width,  (Projectile.height / 2)) || tile is { HasUnactuatedTile: true } && Main.tileSolid[tile.TileType])
                    {
                        Explosion();
                        Player.RedemptionScreen().ScreenShakeIntensity += 4;
                        originPos = Projectile.Center;
                        landed = true;
                    }
                }
            }
            else
            {
                explosionTimer++;
                GenerateIceSpike();
                Projectile.friendly = false;
                if (Projectile.ai[0]++ > 30 * update * SwingSpeed)
                    Projectile.Kill();
            }
            if (Timer == 2)
            {
                Projectile.alpha = 0;
                for (int i = 0; i < oldDirVector.Length; i++)
                    oldDirVector.SetValue(vector, i);
            }

            for (int k = oldDirVector.Length - 1; k > 0; k--)
            {
                oldDirVector[k] = oldDirVector[k - 1];
            }
            oldDirVector[0] = vector;

            if (Main.netMode != NetmodeID.Server && progress > 0.35f)
            {
                TrailHelper.ManageSwordTrailPosition(oldDirVector.Length, armCenter, oldDirVector, ref directionVectorCache, ref positionCache);
                ManageTrail();
            }
        }
        public override bool? CanCutTiles() => true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Player.RedemptionPlayerBuff().pureIronBonus)
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
        }
        Vector2 originPos;
        Vector2 directionVec;
        float explosionTimer;
        public void Explosion()
        {
            RedeHelper.NPCRadiusDamage(Projectile.getRect(), Player, Projectile.damage, Projectile.knockBack);
            RedeDraw.SpawnExplosion(Projectile.Bottom, Color.LightCyan, 1, 0, 0, 0, .2f, true);

            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Volume = 3f }, Player.Center);

            for (int i = 0; i < 10; i++)
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.Bottom.Y), Projectile.width, 2, DustID.Stone, 0, -4, Scale: 1.5f);

            directionVec = Projectile.Center - Player.Center;
        }
        public void DrawExplosion()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = ModContent.Request<Texture2D>("Redemption/Textures/Shockwave").Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Vector2 drawPos = originPos + Vector2.UnitY * 20;
            Vector2 scale = new(1, 0.3f);

            int update = Projectile.extraUpdates + 1;
            float progress = explosionTimer / (15 * update);
            scale *= MathF.Pow(progress, 0.3f);
            float opacity = 1 - progress;
            Color color = Color.LightSteelBlue;
            float rotation = directionVec.ToRotation();

            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(color) * opacity, rotation, drawOrigin, scale * 0.4f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, new Rectangle?(rect), Projectile.GetAlpha(color) * opacity, rotation, drawOrigin, scale * 0.6f, spriteEffects, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault(true);
        }
        public void GenerateIceSpike()
        {
            Projectile.ai[1]++;

            if (Projectile.ai[1] == 4)
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
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), origin + new Vector2(0, -80), new Vector2(0, -30), ModContent.ProjectileType<PureIronHammer_IceSpike>(), (int)(Projectile.damage * 0.75f), 0, Player.whoAmI);
                    }
                }
            }
        }
        #region slash trail property
        private Vector2[] oldDirVector = new Vector2[120];
        private List<Vector2> directionVectorCache = new();
        private List<Vector2> positionCache = new();
        private DanTrail trail;
        private Color baseColor = Color.LightSteelBlue;
        private Color endColor = Color.LightSkyBlue;
        #endregion
        public void ManageTrail()
        {
            trail ??= new DanTrail(Main.instance.GraphicsDevice, oldDirVector.Length, new NoTip(),
            factor =>
            {
                float mult = factor;
                return 16f * MathF.Pow(mult, 0.2f) * Projectile.scale;
            },
            factor =>
            {
                float progress = EaseFunction.EaseCubicOut.Ease(1 - factor.X);
                return Color.Lerp(baseColor, endColor, EaseFunction.EaseCubicIn.Ease(progress)) * (1 - progress) * Projectile.Opacity;
            });
            trail.Positions = positionCache.ToArray();
            trail.NextPosition = Projectile.Center;
        }
        public void DrawTrail()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Effect effect = ModContent.Request<Effect>("Redemption/Effects/GlowTrailShader", AssetRequestMode.ImmediateLoad).Value;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.X, -Main.screenPosition.Y, 0);
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/CrystalTrail").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            Texture2D texture = Player.direction == 1 ? ModContent.Request<Texture2D>("Redemption/Textures/Trails/SlashTrail_5").Value : ModContent.Request<Texture2D>("Redemption/Textures/Trails/SlashTrail_5_flipped").Value;

            Effect effect2 = ModContent.Request<Effect>("Redemption/Effects/GlowTrailShader", AssetRequestMode.ImmediateLoad).Value;
            effect2.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect2.Parameters["sampleTexture"].SetValue(texture);
            effect2.Parameters["time"].SetValue(1);
            effect2.Parameters["repeats"].SetValue(-Player.direction);

            trail?.Render(effect2);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Vector2 dirOffeset = vector.SafeNormalize(Vector2.One) * Projectile.scale;
            Vector2 drawPos = Projectile.Center + dirOffeset * -12 + dirOffeset.RotatedBy(MathHelper.PiOver2) * 2 * Projectile.spriteDirection;

            DrawTrail();
            if(explosionTimer > 0)
                DrawExplosion();
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
    public class PureIronHammer_IceSpike : TrueMeleeProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_961";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ElementID.ProjIce[Type] = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 200;
            Projectile.penetrate = -1;
            Projectile.alpha = 254;
            Projectile.scale = 1;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        private Player Player => Main.player[Projectile.owner];
        int variation;
        float delay;
        public override void OnSpawn(IEntitySource source)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundID.DeerclopsIceAttack with { Volume = .5f }, Projectile.Center);

            variation = Main.rand.Next(1, 4);

            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), 16, Projectile.velocity * 0.75f * MathHelper.Lerp(0.2f, 0.7f, Main.rand.NextFloat()));
                dust.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                dust.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
            }
            for (int j = 0; j < 5; j++)
            {
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), 16, Main.rand.NextVector2Circular(2f, 2f) + Projectile.velocity * 0.75f * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                dust2.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                dust2.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                dust2.fadeIn = 1f;
            }
        }
        public override void AI()
        {
            Projectile.scale = MathHelper.Clamp(delay * 0.2f, 0, 1);
            if (Projectile.scale >= 0.5f)
                Projectile.friendly = true;

            if (delay == 2)
                Projectile.alpha = 0;

            if (delay++ > 40)
                Projectile.alpha += 100;

            if (delay == 38)
            {
                for (int i = -1; i < 2; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Vector2.UnitY * 10 + Vector2.UnitY * 60 * i, Vector2.Zero, ModContent.ProjectileType<IceSpike_Mist>(), Projectile.damage, Projectile.knockBack, Projectile.owner, i);

                for (int i = 0; i < 5; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.RandAreaInEntity(), Vector2.Zero, ModContent.ProjectileType<IceSpikeShard>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }
        public override bool? CanCutTiles() => true;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Player.RedemptionPlayerBuff().pureIronBonus)
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);

            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), 16, Main.rand.NextVector2Circular(8f, 8f) * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                dust.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                dust.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
            }
            for (int j = 0; j < 5; j++)
            {
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), 16, Main.rand.NextVector2Circular(8f, 8f) * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                dust2.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                dust2.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                dust2.fadeIn = 1f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = texture.Frame(1, 5);
            Vector2 drawOrigin = new(0, rect.Size().Y / 2);
            Vector2 v = Projectile.Center + Vector2.UnitY * 100;
            Main.EntitySpriteDraw(texture, v + Vector2.UnitX * 20 - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, texture.Frame(1, 5, 0, variation + 1), Projectile.GetAlpha(lightColor), Projectile.rotation - MathHelper.PiOver2, drawOrigin, Projectile.scale * 0.5f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, v - Vector2.UnitX * 20 - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, texture.Frame(1, 5, 0, variation - 1), Projectile.GetAlpha(lightColor), Projectile.rotation - MathHelper.PiOver2, drawOrigin, Projectile.scale * 0.5f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, v - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY, texture.Frame(1, 5, 0, variation), Projectile.GetAlpha(lightColor), Projectile.rotation - MathHelper.PiOver2, drawOrigin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
    public class IceSpike_Mist : ModProjectile
    {
        public override string Texture => "Redemption/Textures/IceMist";
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
            Projectile.timeLeft = Main.rand.Next(180, 190);
            Projectile.scale = Main.rand.NextFloat(0.5f, 0.6f);
            Projectile.rotation = RedeHelper.RandomRotation();
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity = Main.rand.NextVector2Circular(1f, 1f);

            float Scale = Projectile.ai[0] == -1 ? 0.9f : 1.1f;
            Projectile.scale *= Scale;
        }
        public override void AI()
        {
            if(Projectile.timeLeft < 170)
                Projectile.alpha += 10;

            Projectile.velocity *= 0.9f;
            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] = Main.rand.Next(1, 3);

            if (Projectile.localAI[0] == 1)
                Projectile.rotation -= 0.003f;
            else if (Projectile.localAI[0] == 2)
                Projectile.rotation += 0.003f;

            if (Main.rand.NextBool(10))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }

            if (Projectile.alpha > 255)
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < 6; i++)
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White with { A = 0 }), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
    public class IceSpikeShard : ModProjectile
    {
        public override string Texture => "Redemption/Projectiles/Magic/Icefall_Proj";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ElementID.ProjIce[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.scale = 1f;
            Projectile.frame = Main.rand.Next(3);
            Projectile.localAI[0] = Main.rand.Next(1, 3);
            Projectile.rotation = RedeHelper.RandomRotation();
        }
        public override void AI()
        {
            if (Projectile.ai[0] is 1)
            {
                Projectile.rotation += Projectile.velocity.X / 20 * Projectile.direction;
                Projectile.velocity.Y += 0.2f;
                return;
            }
            if (Projectile.localAI[0] == 1)
                Projectile.rotation -= 0.02f;
            else if (Projectile.localAI[0] == 2)
                Projectile.rotation += 0.02f;

            if (Projectile.scale >= 1)
                Projectile.velocity.Y += 0.2f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int i = 0; i < 14; i++)
            {
                int dustIndex4 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice);
                Main.dust[dustIndex4].velocity *= 2f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.Frostburn, 180);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] is 1)
                modifiers.FinalDamage *= 4;
            modifiers.FinalDamage *= Projectile.scale;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => modifiers.FinalDamage *= Projectile.scale;
    }
}
