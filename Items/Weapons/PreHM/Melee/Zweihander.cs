using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class Zweihander : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zweihander");
            Tooltip.SetDefault("Parries physical projectiles" +
                "\nDeals more damage at the tip of the blade\n" +
                "'Parry this you filthy casual!'");

            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 74;
            Item.height = 74;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 62;
            Item.knockBack = 7;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<Zweihander_SlashProj>();
        }

        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(30))
                return;

            int sparkle = Dust.NewDust(Item.position, Item.width, Item.height, DustID.SilverCoin, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ZweihanderFragment1>())
                .AddIngredient(ModContent.ItemType<ZweihanderFragment2>())
                .AddCondition(new Recipe.Condition(NetworkText.FromLiteral("Repaired by the Fallen"), _ => false))
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'A famous greatsword of Madmount. A lot of strength is needed to use such a grand weapon,\n" +
                    "but for most Warriors of the Iron Realm this was a trivial issue.'")
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
