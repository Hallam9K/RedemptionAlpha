using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Localization;
using Terraria.DataStructures;
using System.Collections.Generic;
using Redemption.Projectiles.Magic;
using Redemption.Globals;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class SunshardGreatstaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Casts a burst of redemptive sparks" +
                "\nCasts a holy ray of light every 3 consecutive shots"); */
            Item.staff[Item.type] = true;

            Item.ResearchUnlockCount = 1;
        }

        private float glowRot = 0;
        public override void SetDefaults()
        {
            Item.damage = 65;
            Item.height = 62;
            Item.width = 62;
            Item.useTime = 56;
            Item.useAnimation = 56;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7;
            Item.channel = true;
            Item.rare = ItemRarityID.Yellow;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.UseSound = SoundID.Item125;
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<Sunshard>();

        }
        private int CastCount;
        public override void HoldItem(Player player)
        {
            if (!player.channel)
                CastCount = 0;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 5;
            float rotation = MathHelper.ToRadians(20);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1)));
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }

            CastCount++;
            if (CastCount >= 3)
            {
                SoundEngine.PlaySound(SoundID.Item122, player.position);
                RedeDraw.SpawnRing(position, new Color(255, 255, 120), 0.12f, 0.86f, 4);
                RedeDraw.SpawnRing(position, new Color(255, 255, 120), 0.14f, 0.83f, 3);
                RedeDraw.SpawnRing(position, new Color(255, 255, 120), 0.16f);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SunshardRay>(), damage, knockback, player.whoAmI);
                CastCount = 0;
            }
            return false;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 70f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SkyFracture)
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddIngredient(ItemID.SoulofSight, 10)
                .AddIngredient(ItemID.SoulofFright, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.Items.SunshardGreatstaff.Lore"))
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.Viewer"))
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }

        public override void PostUpdate()
        {
            glowRot += 0.03f;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(211, 232, 169), new Color(247, 247, 169), new Color(211, 232, 169));
            Vector2 origin = new(glow.Width / 2, glow.Height / 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(glow, Item.Center - Main.screenPosition - new Vector2(-14f, 16f), new Rectangle(0, 0, glow.Width, glow.Height), color, glowRot, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition - new Vector2(-14f, 16f), new Rectangle(0, 0, glow.Width, glow.Height), color, -glowRot, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
    }
}
