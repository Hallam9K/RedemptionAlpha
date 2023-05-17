using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Projectiles.Ranged;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Redemption.Buffs.Cooldowns;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class XeniumElectrolaser : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Hold left-click to fire blasts of electrified laser beams, which gradually increase in accuracy\n" +
                "Once it is 100% accurate, it will fire a large beam that deals heavy damage\n" +
                "(5[i:" + ModContent.ItemType<EnergyPack>() + "]) Right-click to fire a precision shot that can penetrate surfaces and lingers, but has a 5 second cooldown\n" +
                "Requires an Energy Pack to be in your inventory"); */
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 175;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 90;
            Item.height = 34;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 7;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<XeniumElectrolaser_Beam>();
            Item.shootSpeed = 3;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
                damage *= 3;

            type = ModContent.ProjectileType<XeniumElectrolaser_Proj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                if (player.GetModPlayer<EnergyPlayer>().statEnergy < 5 || player.HasBuff<XeniumElectrolaserCooldown>())
                    return false;

                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, 0, 1);
                player.AddBuff(ModContent.BuffType<XeniumElectrolaserCooldown>(), 5 * 60);
                return false;
            }
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<XeniumAlloy>(), 14)
                .AddIngredient(ModContent.ItemType<Capacitor>(), 2)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 6)
                .AddTile(ModContent.TileType<XeniumRefineryTile>())
                .Register();
        }
    }
}
