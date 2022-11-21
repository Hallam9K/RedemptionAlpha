using IL.Terraria.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.NPCs.Bosses.Neb;
using Redemption.Projectiles.Melee;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class PiercingNebulaWeapon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Piercing Nebula");
            Tooltip.SetDefault("Deals less damage the further away the target\n" +
                "'Penetrates through even the fabric of space'");
            SacrificeTotal = 1;
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 333;
            Item.DamageType = DamageClass.Melee;
            Item.width = 82;
            Item.height = 82;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.UseSound = SoundID.Item125;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PNebula1_Friendly>();
            Item.shootSpeed = 9f;
            Item.rare = ModContent.RarityType<CosmicRarity>();
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture).Value;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<PiercingNebulaWeapon_Proj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DayBreak)
                .AddIngredient(ModContent.ItemType<LifeFragment>(), 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}