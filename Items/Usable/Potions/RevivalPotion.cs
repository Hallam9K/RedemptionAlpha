using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class RevivalPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Use on an unconsious town npc to wake them up" +
                "\nConsume to cure most debuffs"); */
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(255, 136, 96),
                new Color(252, 85, 101),
                new Color(255, 49, 87)
            };
            Item.ResearchUnlockCount = 4;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 38;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.healLife = 100;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.potion = true;
        }
        public override bool? UseItem(Player player)
        {
            player.ClearBuff(BuffID.Bleeding);
            player.ClearBuff(BuffID.Poisoned);
            player.ClearBuff(BuffID.OnFire);
            player.ClearBuff(BuffID.Venom);
            player.ClearBuff(BuffID.Darkness);
            player.ClearBuff(BuffID.Blackout);
            player.ClearBuff(BuffID.Silenced);
            player.ClearBuff(BuffID.Cursed);
            player.ClearBuff(BuffID.Slow);
            player.ClearBuff(BuffID.Weak);
            player.ClearBuff(BuffID.BrokenArmor);
            player.ClearBuff(BuffID.CursedInferno);
            player.ClearBuff(BuffID.Ichor);
            player.ClearBuff(BuffID.Frostburn);
            player.ClearBuff(BuffID.Chilled);
            player.ClearBuff(BuffID.VortexDebuff);
            player.ClearBuff(BuffID.Obstructed);
            player.ClearBuff(BuffID.Electrified);
            player.ClearBuff(BuffID.Rabies);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ModContent.ItemType<AnglonicMysticBlossom>())
                .AddIngredient(ItemID.Mushroom, 4)
                .AddIngredient(ItemID.Gel, 8)
                .AddIngredient(ItemID.BottledWater, 4)
                .AddTile(TileID.Bottles)
                .DisableDecraft()
                .Register();
        }
    }
}
