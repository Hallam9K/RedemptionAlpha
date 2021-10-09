using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class KeepersClaw : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Keeper's Claw");
            Tooltip.SetDefault("Hitting enemies inflict Necrotic Gouge\n" +
                "Deals double damage to undead" +
                "\n'The hand of my beloved, cold and dead...'");

            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 54;
            Item.height = 48;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 36;
            Item.knockBack = 4;
            Item.crit = 4;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<KeepersClaw_Slash>();
        }
    }
}
