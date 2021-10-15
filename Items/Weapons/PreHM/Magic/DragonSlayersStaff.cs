using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Magic;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class DragonSlayersStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Slayer's Staff");
            Tooltip.SetDefault("Casts a molten dragon skull to spews out flames" +
                "\nHold down left click long enough to change the flames into a heat ray");
            Item.staff[Item.type] = true;
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 50;
            Item.height = 64;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.reuseDelay = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.channel = true;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.DD2_BetsySummon;
            Item.shootSpeed = 0;
            Item.shoot = ModContent.ProjectileType<DragonSkull_Proj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<DragonLeadAlloy>(), 10)
            .AddIngredient(ItemID.Bone, 5)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}