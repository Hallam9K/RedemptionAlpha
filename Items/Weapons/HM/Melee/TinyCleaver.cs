using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Weapons.HM.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class TinyCleaver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tiny Cleaver");
            Tooltip.SetDefault("Swings causes the blade segments to detach, increasing range");

            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 52;
            Item.height = 52;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 1);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;

            // Weapon Properties
            Item.damage = 160;
            Item.knockBack = 6;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<TinyCleaver_Proj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CorruptedXenomite>(), 4)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 8)
                .AddIngredient(ModContent.ItemType<Plating>(), 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine axeLine = new(Mod, "SharpBonus", "Slash Bonus: Small chance to decapitate skeletons, killing them instantly") { OverrideColor = Colors.RarityOrange };
            tooltips.Add(axeLine);
        }
    }
}
