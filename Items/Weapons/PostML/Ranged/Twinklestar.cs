using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary.Core;
using ParticleLibrary.Utilities;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Items.Materials.PostML;
using Redemption.Particles;
using Redemption.Projectiles.Ranged;
using Redemption.Rarities;
using ReLogic.Content;
using System;
using Terraria;
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
            Item.shoot = ProjectileType<Twinklestar_TinyStar>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 6;
            for (int i = 0; i < numberProjectiles; i++)
            {
                velocity *= Main.rand.NextFloat(0.8f, 1f);
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(40));
                Projectile.NewProjectile(source, position, perturbedSpeed, ProjectileType<Twinklestar_TinyStar>(), damage, knockback, player.whoAmI, 0, 0, i);
            }
            Projectile.NewProjectile(source, position, velocity, ProjectileType<Twinklestar_Holdout>(), damage, knockback, player.whoAmI, 0, 0, player.itemAnimationMax);
            return false;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ProjectileType<Twinklestar_Holdout>()] <= 0;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Phantasm)
                .AddIngredient(ItemType<LifeFragment>(), 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
    public class Twinklestar_Holdout : TrueMeleeProjectile
    {
        public ref float Timer => ref Projectile.ai[0];
        public ref float Charge => ref Projectile.ai[1];
        public Player Player => Main.player[Projectile.owner];

        public override string Texture => "Redemption/Items/Weapons/PostML/Ranged/Twinklestar";
        public override bool ShouldUpdatePosition() => false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            Projectile.timeLeft = 2;

            Player.itemTime = 2;
            Player.itemAnimation = 2;

            Vector2 playerCenter = Player.RotatedRelativePoint(Player.MountedCenter, true);
            RedeProjectile.HoldOutProj_SlowTurn(Projectile, Player, playerCenter, 0.4f);
            Projectile.Center = playerCenter + Projectile.velocity * 10;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;

            Player.ChangeDir(Projectile.direction);
            Player.heldProj = Projectile.whoAmI;
            Player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            if (!Player.channel || Player.noItems || Player.CCed || Player.dead || !Player.active)
            {
                if (Timer++ == 0 && Charge >= 20)
                {
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 10f, ProjectileType<Twinklestar_ShootingStar>(), Projectile.damage * 3, Projectile.knockBack, Projectile.owner, Charge);
                    DrawParticle();
                }

                if (Timer == 0 && Charge < 5)
                    Projectile.Kill();

                if (Timer >= 20)
                    Projectile.Kill();
            }
            else
            {
                Charge += 2 * Player.GetAttackSpeed(DamageClass.Ranged);
                Charge = MathHelper.Clamp(Charge, 0, 60);
            }
            Projectile.netUpdate = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = texture.Frame();
            Vector2 drawOrigin = rect.Size() * 0.5f;
            Vector2 drawPos = Projectile.Center + Projectile.velocity * 16 - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPos, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            DrawChargeIndicator();
            return false;
        }
        public void DrawParticle()
        {
            Vector2 drawPos = Projectile.Center + Projectile.velocity * 32;
            Vector2 velocity = Projectile.velocity * 20;

            for (int i = 0; i < 4; i++)
            {
                RedeParticleManager.CreateSpeedParticle(drawPos + new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)) * 6, velocity * Main.rand.NextFloat(0.75f, 1f) * (i + 1) * 0.33f, 0.5f, Color.White.WithAlpha(0), 0.99f, 16);
                RedeParticleManager.CreateSpeedParticle(drawPos + new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)) * 6, -velocity * Main.rand.NextFloat(0.75f, 1f) * (i + 1) * 0.33f, 0.5f, Color.White.WithAlpha(0), 0.99f, 16);
                RedeParticleManager.CreateSpeedParticle(drawPos + new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)) * 6, velocity.RotatedBy(1.57f) * Main.rand.NextFloat(0.75f, 1f) * (i + 1), 0.5f, Color.White.WithAlpha(0), 0.91f, 16);
                RedeParticleManager.CreateSpeedParticle(drawPos + new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)) * 6, velocity.RotatedBy(-1.57f) * Main.rand.NextFloat(0.75f, 1f) * (i + 1), 0.5f, Color.White.WithAlpha(0), 0.91f, 16);
            }
        }
        public void DrawChargeIndicator()
        {
            float chargeProgress = EaseFunction.EaseCubicIn.Ease(Charge / 60f);
            float opacityProgress = EaseFunction.Linear.Ease(Timer / 20f);

            Texture2D TrailTex = Request<Texture2D>("Redemption/Textures/Trails/CrystalTrail", AssetRequestMode.ImmediateLoad).Value;
            Effect effect = Request<Effect>("Redemption/Effects/Beam", AssetRequestMode.ImmediateLoad).Value;

            Color col1 = Color.Lerp(RedeColor.NebColour, Color.White, chargeProgress * 0.5f);
            Color col2 = Color.Lerp(Main.DiscoColor, Color.White, chargeProgress * 0.5f);

            effect.Parameters["uTexture"].SetValue(TrailTex);
            effect.Parameters["progress"].SetValue(Main.GlobalTimeWrappedHourly / 3f);
            effect.Parameters["uColor"].SetValue(col1.ToVector4());
            effect.Parameters["uSecondaryColor"].SetValue(col2.ToVector4());

            Vector2 vel = Projectile.velocity;
            float opacity = MathHelper.Lerp(1, 0, opacityProgress);

            TrianglePrimitive tri = new()
            {
                TipPosition = Projectile.Center - vel * chargeProgress * 30 - Main.screenPosition,
                Rotation = vel.ToRotation(),
                Height = 150,
                Color = Color.White * opacity,
                Width = MathHelper.Lerp(100, 20, chargeProgress)
            };
            PrimitiveRenderer.DrawPrimitiveShape(tri, effect);
        }
    }
}
