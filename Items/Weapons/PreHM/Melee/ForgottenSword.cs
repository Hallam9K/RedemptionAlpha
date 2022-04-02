using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class ForgottenSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sword of the Forgotten");
            Tooltip.SetDefault("Slashes upwards instead of downwards");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 68;
            Item.height = 68;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 65);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 33;
            Item.knockBack = 5;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<ForgottenSword_Proj>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine axeLine = new(Mod, "SharpBonus", "Slash Bonus: Small chance to decapitate skeletons, killing them instantly") { OverrideColor = Colors.RarityOrange };
            tooltips.Add(axeLine);
        }
    }
}
