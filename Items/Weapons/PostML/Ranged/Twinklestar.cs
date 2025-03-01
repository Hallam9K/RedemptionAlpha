using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Helpers;
using Redemption.Items.Materials.PostML;
using Redemption.Particles;
using Redemption.Rarities;
using Redemption.Textures;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class Twinklestar : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.reuseDelay = 5;
            Item.shootSpeed = 20f;
            Item.knockBack = 0.25f;
            Item.width = 50;
            Item.height = 102;
            Item.damage = 350;
            Item.UseSound = CustomSounds.NebSound1 with { Volume = .7f };
            Item.rare = RarityType<CosmicRarity>();
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.channel = true;
            Item.DamageType = DamageClass.Ranged;
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ProjectileType<TwinklestarTinyStar>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 6;
            for (int i = 0; i < numberProjectiles; i++)
            {
                velocity *= Main.rand.NextFloat(0.8f, 1f);
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(40));
                Projectile.NewProjectile(source, position, perturbedSpeed, ProjectileType<TwinklestarTinyStar>(), damage, knockback, player.whoAmI, 0, 0, i);
            }
            Projectile.NewProjectile(source, position, velocity, ProjectileType<TwinklestarHoldout>(), damage, knockback, player.whoAmI, 0, 0, player.itemAnimationMax);
            return false;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ProjectileType<TwinklestarHoldout>()] <= 0;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Phantasm)
                .AddIngredient(ItemType<LifeFragment>(), 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
    public class TwinklestarHoldout : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PostML/Ranged/Twinklestar";
        private const float AimResponsiveness = 1f;
        private float charge;
        private float Timer;
        private float Timer2;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            // Use CloneDefaults to clone all basic Projectile statistics from the vanilla Last Prism.
            Projectile.CloneDefaults(ProjectileID.LastPrism);
            Projectile.friendly = false;
            Projectile.Redemption().TechnicallyMelee = true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            UpdatePlayerVisuals(player, rrp);

            if (Projectile.owner == Main.myPlayer)
            {
                // Slightly re-aim the Prism every frame so that it gradually sweeps to point towards the mouse.
                UpdateAim(rrp, player.HeldItem.shootSpeed);
                // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
                // player.channel indicates whether the player is still holding down the mouse button to use the item.
                bool stillInUse = player.channel && !player.noItems && !player.CCed;

                Vector2 beamVelocity = Projectile.velocity;
                int uuid = Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI);
                int damage = Projectile.damage;
                float knockback = Projectile.knockBack;

                if (stillInUse)
                {
                    charge += 2 * player.GetAttackSpeed(DamageClass.Ranged);
                    charge = MathHelper.Clamp(charge, 0, 50);
                    if (player.ownedProjectileCounts[ProjectileType<TwinklestarRadius>()] <= 0)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<TwinklestarRadius>(), damage, knockback, Projectile.owner, Projectile.whoAmI, uuid);
                }
                else
                {
                    if (Timer++ == 0 && charge >= 20)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, 0.5f * beamVelocity, ProjectileType<ShootingStar>(), damage * 3, knockback, Projectile.owner, charge, uuid);

                    if (Timer == 0 && charge < 5)
                        Projectile.Kill();

                    else if (Timer >= 30)
                        Projectile.Kill();
                }
                Projectile.netUpdate = true;
            }

            if (Timer2 >= 29)
                Timer2 = 0;
            Projectile.timeLeft = 2;
        }
        private void UpdatePlayerVisuals(Player player, Vector2 playerHandPos)
        {
            float num = MathHelper.ToRadians(0f);
            if (Projectile.spriteDirection == -1)
                num = MathHelper.ToRadians(180f);

            // Place the Prism directly into the player's hand at all times.
            Projectile.Center = playerHandPos;
            // The beams emit from the tip of the Prism, not the side. As such, rotate the sprite by pi/2 (90 degrees).
            Projectile.rotation = Projectile.velocity.ToRotation() + num;
            Projectile.spriteDirection = Projectile.direction;

            // The Prism is a holdout Projectile, so change the player's variables to reflect that.
            // Constantly resetting player.itemTime and player.itemAnimation prevents the player from switching items or doing anything else.
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            // If you do not multiply by Projectile.direction, the player's hand will point the wrong direction while facing left.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }
        private void UpdateAim(Vector2 source, float speed)
        {
            // Get the player's current aiming direction as a normalized vector.
            Vector2 aim = Vector2.Normalize(Main.MouseWorld - source);
            if (aim.HasNaNs())
            {
                aim = Vector2.UnitX * Main.player[Projectile.owner].direction;
            }

            // Change a portion of the Prism's current velocity so that it points to the mouse. This gives smooth movement over time.
            aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, AimResponsiveness));
            aim *= speed;

            if (aim != Projectile.velocity)
            {
                Projectile.netUpdate = true;
            }
            Projectile.velocity = aim;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.MouseWorld.X < player.Center.X)
                player.direction = -1;
            else
                player.direction = 1;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Color drawColor = Color.White;
            Main.EntitySpriteDraw(texture, position, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), drawColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, effects, 0);
            return false;
        }
    }
    public class TwinklestarRadius : ModProjectile, IDrawAdditive
    {
        public override string Texture => "Redemption/Textures/TelegraphLine";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Telegraph");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.LastPrism);
            Projectile.friendly = false;
            Projectile.Redemption().TechnicallyMelee = true;
            Projectile.alpha = 255;
        }
        public readonly float LaserLength = 100;
        public float charge;
        public float Timer;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.ownedProjectileCounts[ProjectileType<TwinklestarHoldout>()] <= 0)
                Projectile.Kill();

            Projectile host = Main.projectile[(int)Projectile.ai[0]];
            float num = MathHelper.ToRadians(0f);
            if (host.spriteDirection == -1)
                num = MathHelper.ToRadians(180f);

            Projectile.Center = host.Center;
            Projectile.rotation = host.rotation + num;
            Projectile.alpha -= 4;
            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 100, 255);
            if (!player.channel)
                Projectile.alpha += 20;
            if (charge >= 50)
                Projectile.alpha += 5;
            charge++;
            charge = MathHelper.Clamp(charge, 0, 50);
        }
        public void AdditiveCall(SpriteBatch sB, Vector2 screenPos)
        {
            DrawTether(Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * LaserLength, screenPos, RedeColor.NebColour, Main.DiscoColor, 10 * charge, Projectile.Opacity);
        }
        public void DrawTether(Vector2 End, Vector2 screenPos, Color color1, Color color2, float Size, float Strength)
        {
            Texture2D TrailTex = Request<Texture2D>("Redemption/Textures/Trails/Trail_5", AssetRequestMode.ImmediateLoad).Value;
            Effect effect = Request<Effect>("Redemption/Effects/Beam", AssetRequestMode.ImmediateLoad).Value;
            effect.Parameters["uTexture"].SetValue(TrailTex);
            effect.Parameters["progress"].SetValue(Main.GlobalTimeWrappedHourly / 3);
            effect.Parameters["uColor"].SetValue(color1.ToVector4());
            effect.Parameters["uSecondaryColor"].SetValue(color2.ToVector4());

            Vector2 dist = End - Projectile.Center;

            TrianglePrimitive tri = new()
            {
                TipPosition = Projectile.Center - screenPos,
                Rotation = dist.ToRotation(),
                Height = Size + 20 + dist.Length(),
                Color = Color.White * Strength,
                Width = 2 * Size + 10
            };

            PrimitiveRenderer.DrawPrimitiveShape(tri, effect);
        }
    }
    public class TwinklestarTinyStar : ModProjectile
    {
        public override string Texture => "Redemption/Textures/WhiteOrb";
        public override void SetStaticDefaults()
        {
            ElementID.ProjCelestial[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = 3;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.alpha = 5;
            Projectile.scale = .4f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Projectile.rotation += 0.1f;
            Projectile.velocity *= 0.96f;
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.ai[0] = Main.rand.NextFloat(0f, 360f);
                Projectile.scale = Main.rand.NextFloat(.3f, .5f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Projectile.Opacity = Projectile.timeLeft <= 20 ? 1f - 1f / 20f * (20 - Projectile.timeLeft) : 1f;
            float amount = MathHelper.Lerp(0f, 1f, Main.GlobalTimeWrappedHourly * 64f % 360 / 360);
            Color hsl = Main.hslToRgb(amount, 1f, 0.75f);
            Color color2 = Color.Multiply(new(hsl.R, hsl.G, hsl.B, 0), Projectile.Opacity);
            Main.spriteBatch.Draw(CommonTextures.RainbowParticle2.Value, Projectile.Center - Main.screenPosition, null, color2, Projectile.ai[0].InRadians().AngleLerp((Projectile.ai[0] + 90f).InRadians(), (120f - Projectile.timeLeft) / 120f), new Vector2(71, 21), 0.75f * Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(CommonTextures.RainbowParticle2.Value, Projectile.Center - Main.screenPosition, null, color2, Projectile.ai[0].InRadians().AngleLerp((Projectile.ai[0] + 90f).InRadians(), (120f - Projectile.timeLeft) / 120f) + MathHelper.PiOver2, new Vector2(71, 21), 0.75f * Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color2 * .5f, Projectile.ai[0].InRadians().AngleLerp((Projectile.ai[0] + 90f).InRadians(), (120f - Projectile.timeLeft) / 120f), new Vector2(114 / 2, 114 / 2), Projectile.scale + 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(CommonTextures.GlowParticle.Value, Projectile.Center - Main.screenPosition, null, color2, Projectile.rotation, new Vector2(64, 64), Projectile.scale * .3f, SpriteEffects.None, 0f);
            return false;
        }
    }
    public class ShootingStar : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Neb/PNebula1";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjCelestial[Type] = true;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public float vectorOffset = 0f;
        public bool offsetLeft = false;
        public Vector2 originalVelocity = Vector2.Zero;
        public ref float Size => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[2];
        public float Timer2;
        public float flag;

        private readonly int NUMPOINTS = 50;
        public Color baseColor = Color.Pink;
        public Color endColor = RedeColor.NebColour;
        public Color edgeColor = Color.Purple;
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 4f;
        public override void AI()
        {
            Projectile.alpha += 2;
            Projectile.localAI[0]++;

            if (Main.rand.NextBool(3))
                ParticleManager.NewParticle(Projectile.Center + Projectile.velocity, RedeHelper.Spread(1), new RainbowParticle(), Color.White, Main.rand.NextFloat(.4f, .6f) * Projectile.Opacity, 0, 0, 0, 0, Main.rand.Next(20, 40), Projectile.Opacity);
            for (int i = 0; i < 3; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center, 2, 2, DustType<GlowDust>(), Scale: 2 * Projectile.Opacity)];
                dust.noGravity = true;
                dust.noLight = true;
                Color dustColor = new(RedeColor.NebColour.R, RedeColor.NebColour.G, RedeColor.NebColour.B) { A = 0 };
                dust.color = dustColor * .2f * Projectile.Opacity;
            }
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                ManageTrail();
            }

            Timer++;
            if (Timer == 10 && !Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.NebSound3 with { Pitch = .3f, Volume = .6f }, Projectile.position);

            Projectile.rotation += 0.1f;
            switch (Projectile.localAI[1])
            {
                case 0:
                    Projectile.scale = 0.01f;
                    Projectile.localAI[1] = 1;
                    break;
                case 1:
                    if (Projectile.alpha > 0)
                        Projectile.alpha -= 50;

                    Projectile.scale += 0.06f;
                    Projectile.scale = MathHelper.Clamp(Projectile.scale, 0.01f, 0.5f);
                    break;
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || !(proj.type == ProjectileType<TwinklestarTinyStar>()))
                    continue;

                if (proj.velocity.Length() > 10)
                    proj.friendly = true;
                else
                    proj.friendly = false;

                if (Helper.CheckCircularCollision(Projectile.Center, (int)Size * 6, proj.Hitbox) && Timer >= 10)
                {
                    proj.ai[0] = 1;
                    if (proj.ai[0] == 1)
                        proj.Move(Projectile.Center - 30f * Vector2.Normalize(Projectile.velocity), 200, 20);

                    if (proj.ai[0] == 1)
                    {
                        proj.ai[1] = 1;
                        proj.friendly = true;
                    }
                }
            }
            MakeDust(45, 0.25f);
            MakeDust(70, 0.5f);

            if (fakeTimer > 0)
                FakeKill();
        }

        public void MakeDust(float pos, float spread)
        {
            Vector2 position = Projectile.Center + (Vector2.Normalize(Projectile.velocity) * pos);
            Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GiantCursedSkullBolt)];
            dust.position = position;
            dust.velocity = (Projectile.velocity.RotatedBy(1.57) * spread) + (Projectile.velocity / 4f);
            dust.position += Projectile.velocity.RotatedBy(1.57) * spread;
            dust.fadeIn = 0.5f;
            dust.noGravity = true;
            dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GiantCursedSkullBolt)];
            dust.position = position;
            dust.velocity = (Projectile.velocity.RotatedBy(-1.57) * spread) + (Projectile.velocity / 4f);
            dust.position += Projectile.velocity.RotatedBy(-1.57) * spread;
            dust.fadeIn = 0.5f;
            dust.noGravity = true;
        }

        private int fakeTimer;
        private void FakeKill()
        {
            if (fakeTimer++ == 0)
            {
                for (int i = 0; i < 20; i++)
                    ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), new GlowParticle2(), new Color(117, 10, 47), 3 * Projectile.scale, .45f, Main.rand.Next(50, 60));
                for (int i = 0; i < 20; i++)
                    ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), new GlowParticle2(), new Color(94, 53, 104), 3 * Projectile.scale, .45f, Main.rand.Next(50, 60));
                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<VoidFlame>(), Scale: 2);
                    Main.dust[dust].velocity *= 2;
                    Main.dust[dust].noGravity = true;
                }
            }
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.velocity *= 0;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            if (fakeTimer >= 60)
                Projectile.Kill();
        }
        public override void OnKill(int timeLeft)
        {
            if (fakeTimer > 0)
                return;
            for (int i = 0; i < 20; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), new GlowParticle2(), new Color(117, 10, 47), 3 * Projectile.scale, .45f, Main.rand.Next(50, 60));
            for (int i = 0; i < 20; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10 * Projectile.scale), new GlowParticle2(), new Color(94, 53, 104), 3 * Projectile.scale, .45f, Main.rand.Next(50, 60));
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<VoidFlame>(), Scale: 2);
                Main.dust[dust].velocity *= 2;
                Main.dust[dust].noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(new Color(255, 255, 255, 0)) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public void ManageTrail()
        {
            trail ??= new DanTrail(Main.instance.GraphicsDevice, NUMPOINTS, new TriangularTip(4),
            factor =>
            {
                float mult = factor;
                if (mult < 0.01f)
                {
                    mult = 0.01f;
                }

                return thickness * 6 * mult * Projectile.Opacity;
            },
            factor =>
            {
                if (factor.X > 0.99f)
                    return Color.Transparent;

                return edgeColor * 0.1f * factor.X * Projectile.Opacity;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center;
            trail2 ??= new DanTrail(Main.instance.GraphicsDevice, NUMPOINTS, new TriangularTip(4),
            factor =>
            {
                float mult = factor;
                if (mult < 0.01f)
                {
                    mult = 0.01f;
                }

                return thickness * 3 * mult * Projectile.Opacity;
            },
            factor =>
            {
                float progress = EaseFunction.EaseCubicOut.Ease(1 - factor.X);
                return Color.Lerp(baseColor, endColor, EaseFunction.EaseCubicIn.Ease(progress)) * (1 - progress) * Projectile.Opacity;
            });

            trail2.Positions = cache2.ToArray();
            trail2.NextPosition = Projectile.Center;
        }
    }
}
