using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using System.Collections.Generic;
using Redemption.Projectiles.Magic;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class SunshardStave : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunshard Staff");
            Tooltip.SetDefault("Casts redemptive sparks.");
            Item.staff[Item.type] = true;

        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.height = 62;
            Item.width = 62;
            Item.useTime = 36;
            Item.DamageType = DamageClass.Magic;
            Item.crit = 20;
            Item.mana = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8;
            Item.useAnimation = 36;
            Item.rare = ItemRarityID.Yellow;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item125;
            Item.shootSpeed = 26f;
            Item.shoot = ModContent.ProjectileType<Sunshard>();

        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int NumShards = 4 + Main.rand.Next(2);

            for (int i = 0; i < NumShards; i++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));

                newVelocity *= 1f - Main.rand.NextFloat(0.5f);

                Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'A golden wand, used by ancient clerics in Thamor.\n" +
                    "Imbued with holy magic, it was used to heal townsfolk, as well as defence for small settlements.'")
                {
                    overrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view lore")
                {
                    overrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 55f;

            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalVileShard)
                .AddIngredient(ItemID.SkyFracture)
                .AddIngredient(ItemID.SoulBottleMight, 10)
                .AddIngredient(ItemID.SoulBottleMight, 10)
                .AddIngredient(ItemID.SoulBottleMight, 10)
                .Register();
        }
    }
}