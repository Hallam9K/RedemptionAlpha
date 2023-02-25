using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class MythrilsBane : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mythril's Bane");
            Tooltip.SetDefault("Hitting armed enemies with the blade inflicts Disarmed and Broken Armor\n" +
                "Disarmed heavily decreases contact damage and their weapon damage\n" +
                "Blocks weak physical projectiles");

            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 84;
            Item.height = 84;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(platinum: 5);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.channel = true;

            // Weapon Properties
            Item.damage = 170;
            Item.knockBack = 5;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<MythrilsBaneSlash_Proj>();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<MythrilsBane_Proj>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'The mighty blade of Zephos, recieved during his time in Arrgath - Capital of Erellon.\n" +
                    "Similarly to his previous weapon, this sword has a slit for catching opponent's blades and snapping them.'")
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Add(line);
            }
            else
            {
                TooltipLine line = new(Mod, "HoldShift", "Hold [Shift] to view lore")
                {
                    OverrideColor = Color.Gray,
                };
                tooltips.Add(line);
            }

            TooltipLine slashLine = new(Mod, "SharpBonus", RedeItem.slashBonus) { OverrideColor = Colors.RarityOrange };
            tooltips.Add(slashLine);
        }
    }
}
