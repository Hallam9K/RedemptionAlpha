using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using Redemption.Rarities;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class DarkStar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Star");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 225;
            Item.width = 30;
            Item.height = 32;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 0, 85, 0);
            Item.maxStack = 6;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<DarkStar_Proj>();
            Item.shootSpeed = 10f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override Color? GetAlpha(Color lightColor) => RedeColor.COLOR_GLOWPULSE;
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < Item.stack;
        }
    }
    public class DarkStar_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Star");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Indigo) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
        public int cooldown;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            BaseAI.AIBoomerang(Projectile, ref Projectile.ai, player.position, player.width, player.height, true, 70f, 30, 34f, 0.6f, true);
            if (cooldown > 0)
                cooldown--;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (cooldown <= 0 && Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 spawnPos = target.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(45 * i)) * (Math.Max(target.width, target.height) + 40);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), spawnPos, RedeHelper.PolarVector(0.1f, (target.Center - spawnPos).ToRotation()), ModContent.ProjectileType<WhiteNeedle_Proj>(), Projectile.damage / 3, 0, Projectile.owner);
                }
                cooldown = 30;
            }
            target.AddBuff(ModContent.BuffType<BlackenedHeartDebuff>(), 120);
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 8;
            height = 8;
            return true;
        }
    }
}
