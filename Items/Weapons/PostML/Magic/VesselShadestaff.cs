using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Items.Materials.PostML;
using Redemption.Projectiles.Magic;
using Redemption.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class VesselShadestaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Hold left-click to summon Shadesouls that float around the player" +
                "\nRelease left-click to make them fly towards cursor point at high speeds");
            Item.staff[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 400;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(0, 55, 0, 0);
            Item.UseSound = SoundID.Item20;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VesselStaff_Proj>();
            Item.shootSpeed = 2f;
            Item.channel = true;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < (player.altFunctionUse == 2 ? 1 : 20);
        }
        public override bool AltFunctionUse(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<VesselStaff_Proj2>()] < 1;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2 && player.RedemptionPlayerBuff().shadowBinder && player.RedemptionPlayerBuff().shadowBinderCharge >= 1)
                type = ModContent.ProjectileType<VesselStaff_Proj2>();
            else
                type = ModContent.ProjectileType<VesselStaff_Proj>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.player[Main.myPlayer];
            int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("Tooltip0"));
            string text;
            if (player.RedemptionPlayerBuff().shadowBinder)
                text = "Right-clicking will summon a ring of shade around you that grows bigger the longer it is active, damaging enemies (Consumes all Shadowbound Souls over time)";
            else
                text = "Has a special ability if Sielukaivo Shadowbinder is equipped";
            TooltipLine line = new(Mod, "text", text) { overrideColor = Color.DarkGray };
            tooltips.Insert(tooltipLocation, line);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<VesselFragment>(), 30)
                .AddIngredient(ModContent.ItemType<Shadesoul>(), 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
