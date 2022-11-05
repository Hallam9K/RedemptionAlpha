using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class BladeOfTheMountain : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of the Mountain");
            Tooltip.SetDefault("Parries physical or ice projectiles" +
                "\nHitting on the very tip of the blade can freeze enemies" +
                "\nEnemies with knockback immunity cannot be frozen\n" +
                "'Send them to their snowy grave'");

            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 76;
            Item.height = 80;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 3);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 78;
            Item.knockBack = 7;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<BladeOfTheMountain_Slash>();
        }

        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(20))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height,
                DustID.IceTorch, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Zweihander>())
                .AddIngredient(ModContent.ItemType<GathicCryoCrystal>(), 12)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                TooltipLine line = new(Mod, "Lore",
                    "'Once a normal greatsword wielded by a well-known warrior of the Iron Realm, her final battle being against\n" +
                    "a great bear in the mountains. The bear was slain, but the warrior's injuries sealed her death shortly after.\n" +
                    "The icy blood of the bear fused with the blade, chilling it with an enchanting glow.\n" +
                    "The blade laid to rest besides it's owner, until another warrior discovered it many years later.'")
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
