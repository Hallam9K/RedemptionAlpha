using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Magic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class DeathMetal : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.noMelee = true;
            Item.knockBack = 7;
            Item.value = 4500;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item47 with { PitchVariance = .2f, Pitch = .5f };
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<DeathCoil_Proj>();
            Item.shootSpeed = 20f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GraveSteelAlloy>(16)
                .AddIngredient(ItemID.Bone, 12)
                .AddIngredient<GrimShard>()
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[Item.shoot] <= 0)
            {
                SoundEngine.PlaySound(SoundID.Zombie83, player.position);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DeathCoil_Proj>(), damage, knockback, player.whoAmI);
            }
            Vector2 Offset = Vector2.Normalize(velocity) * 30f;
            Projectile.NewProjectile(source, position + Offset, velocity * .1f, ModContent.ProjectileType<DeathMetal_Pulse>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
    public class DeathMetal_Pulse : TrueMeleeProjectile
    {
        public override string Texture => "Redemption/Textures/Shockwave";
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.DamageType = DamageClass.Magic;
        }
        public Vector2 direction;
        public ref float Timer => ref Projectile.ai[0];
        public float progress;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Projectile.rotation = direction.ToRotation() - MathHelper.PiOver2;
            Projectile.Center = player.Center + (direction * 30);

            progress = Timer / 30;
            if (Timer++ == 0)
            {
                for (int i = 0; i < 15; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center, 1, 1, ModContent.DustType<GlowDust>(), direction.X, direction.Y, Scale: .5f);
                    Main.dust[dust].velocity *= 3;
                    Main.dust[dust].noGravity = true;
                    Color dustColor = new(61, 255, 178) { A = 0 };
                    Main.dust[dust].color = dustColor;
                }
            }
            if (Timer < 10 && Projectile.ai[1] is 0)
            {
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    Projectile proj = Main.projectile[p];
                    if (!proj.active || proj.owner != Projectile.owner || proj.type != ModContent.ProjectileType<DeathCoil_Proj>())
                        continue;

                    if (proj.ai[0] != 1 || !proj.Hitbox.Intersects(Projectile.Hitbox))
                        continue;

                    SoundEngine.PlaySound(SoundID.AbigailUpgrade, Projectile.position);
                    int manaHeal = player.GetManaCost(player.HeldItem);
                    if (manaHeal > 0)
                    {
                        player.statMana += manaHeal;
                        player.ManaEffect(manaHeal);
                    }
                    proj.velocity = Projectile.velocity * 10;
                    proj.velocity *= 1 + (.1f * proj.localAI[0]);
                    proj.ai[0] = 0;
                    proj.ai[1] = 0;
                    if (proj.localAI[0] < 10)
                        proj.localAI[0]++;
                    proj.timeLeft = 300;
                    proj.penetrate = 6 + (int)proj.localAI[0];
                    proj.netUpdate = true;
                    Projectile.ai[1] = 1;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Vector2 scale = new(1, 0.3f);
            scale *= MathF.Pow(progress, 0.3f);
            float opacity = 1 - progress;
            Color color = new(61, 255, 178);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(color) * opacity, Projectile.rotation, drawOrigin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(color) * opacity, Projectile.rotation, drawOrigin, scale * 0.3f, spriteEffects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}