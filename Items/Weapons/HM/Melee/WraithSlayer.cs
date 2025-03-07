using Redemption.BaseExtension;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class WraithSlayer : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Slaying enemies has a chance to summon Cursed Samurai in their place\n" +
                "Cursed Samurai will act as temporary minions to aid you\n" +
                "Deals more damage to ghostly enemies"); */

            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 94;
            Item.height = 94;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 7);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 118;
            Item.knockBack = 6;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ProjectileType<WraithSlayer_Proj>();

            Item.Redemption().TechnicallySlash = true;
            Item.Redemption().CanSwordClash = true;
        }
        public override bool MeleePrefix() => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float adjustedItemScale2 = player.GetAdjustedItemScale(Item);
            Projectile.NewProjectile(source, position, velocity, ProjectileType<WraithSlayer_Proj>(), damage, knockback, player.whoAmI, 0, 0, adjustedItemScale2);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<GraveSteelAlloy>(), 12)
                .AddIngredient(ItemType<GrimShard>(), 8)
                .AddIngredient(ItemType<LostSoul>(), 8)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore", Language.GetTextValue("Mods.Redemption.Items.WraithSlayer.Lore"))
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
    }
}