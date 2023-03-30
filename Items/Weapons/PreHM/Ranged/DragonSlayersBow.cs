using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class DragonSlayersBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dragon Slayer's Greatbow");
            // Tooltip.SetDefault("Replaces Wooden Arrows with Hellfire Arrows");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 26;
            Item.height = 58;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = false;

            // Weapon Properties
            Item.damage = 42;
            Item.knockBack = 4;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 20f;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<DragonLeadAlloy>(), 10)
                .AddIngredient(ItemID.Bone, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ProjectileID.HellfireArrow;
        }
        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Dragonlike.Contains(target.type))
                modifiers.FinalDamage *= 10;
        }
    }
}