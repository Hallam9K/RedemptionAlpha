using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using System.Collections.Generic;
using Redemption.Projectiles.Magic;
using Terraria.GameContent.Creative;
using Redemption.Globals;
using Terraria.Audio;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class SunshardGreatstaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Casts a burst of redemptive sparks" +
                "\nCasts a holy ray of light every 3 consecutive shots");
            Item.staff[Item.type] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.height = 62;
            Item.width = 62;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 7;
            Item.channel = true;
            Item.rare = ItemRarityID.Yellow;
            Item.noMelee = true;
            Item.autoReuse = true;
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

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
                DustHelper.DrawCircle(position, DustID.GoldFlame, 2, 4, 4, 1, 2, nogravity: true);
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
                .AddIngredient(ItemID.CrystalVileShard)
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
                TooltipLine line = new(Mod, "Lore",
                    "'A golden wand, used by ancient clerics in Thamor.\n" +
                    "Imbued with holy magic, it was used to heal townsfolk, as well as defending small settlements.'")
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
    }
}