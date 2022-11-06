using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class ForgottenGreatsword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ophos' Forgotten Greatsword");
            Tooltip.SetDefault("Spins the blade around the player and sets ablaze\n" +
                "Forms a firestorm that engulfs the player, release left-click to disperse it, launching nearby enemies away");

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 70;
            Item.height = 70;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(gold: 18);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.UseSound = SoundID.DD2_BetsyWindAttack;
            Item.autoReuse = true;
            Item.channel = true;

            // Weapon Properties
            Item.damage = 58;
            Item.knockBack = 5;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<ForgottenGreatsword_Proj>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'A silver sword with a steel handle, forged by Ophos, a fabled blacksmith from what is now Hye Anglon.\n" +
                    "Said to be proficient in Firestorm magic, he would mark his works with a gem that imbued his weapons with his magical power.\n" +
                    "This weapon in particular uses a traditional design of the Territory of Northhill.'")
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
            TooltipLine axeLine = new(Mod, "SharpBonus", "Slash Bonus: Small chance to decapitate skeletons, killing them instantly") { OverrideColor = Colors.RarityOrange };
            tooltips.Add(axeLine);
        }
    }
}
