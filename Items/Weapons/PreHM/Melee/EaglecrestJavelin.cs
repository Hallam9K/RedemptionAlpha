using Microsoft.Xna.Framework;
using Redemption.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class EaglecrestJavelin : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Hold left-click to charge the javelin, release to throw\n" +
                "Strikes the ground and foe alike with lightning, dealing " + ElementID.ThunderS + " damage");

            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 42;
            Item.height = 46;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 3);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 24;
            Item.knockBack = 6;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 0f;
            Item.shoot = ModContent.ProjectileType<EaglecrestJavelin_Proj>();
            Item.ExtraItemShoot(ModContent.ProjectileType<EaglecrestJavelin_Thunder>());
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'An ancient spear used in rituals to the great god Ukko in ancient Gathuram.\n" +
                    "Charged with electric magic to signify Ukko, while the blade has dulled it still works as a fine weapon.'")
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
        }
    }
}
