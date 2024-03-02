using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class HydrasMaw : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.PoisonS);
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 82;
            Item.height = 48;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 10;
            Item.shoot = ModContent.ProjectileType<HydrasMaw_Proj>();
            Item.UseSound = SoundID.Item13;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult *= 2f;
        }
        private int orbCooldown;
        public override void UpdateInventory(Player player)
        {
            if (orbCooldown > 0)
                orbCooldown--;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = SoundID.Item106;
                return orbCooldown <= 0;
            }
            else
                Item.UseSound = SoundID.Item13;
            return true;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 Offset = Vector2.Normalize(velocity) * 60f;
            Vector2 Offset2 = RedeHelper.PolarVector(6 * player.direction, velocity.ToRotation() - MathHelper.PiOver2);

            position.Y += Offset2.Y;
            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.itemAnimationMax = (int)(Item.useTime * 1.5f);
                player.itemTime = (int)(Item.useTime * 1.5f);
                player.itemAnimation = (int)(Item.useTime * 1.5f);
                orbCooldown = 40;
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HydrasMaw_Ball>(), damage, knockback, player.whoAmI);
                return false;
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(4));
                float scale = 1f - (Main.rand.NextFloat() * 0.1f);
                perturbedSpeed *= scale;
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, -8);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Plating>(), 4)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 8)
                .AddIngredient(ModContent.ItemType<ToxicBile>(), 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}