using Microsoft.Xna.Framework;
using Redemption.Buffs;
using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class AbyssalSmoothie : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Massive improvements to all stats\n" +
                "Causes blindness for a short duration" +
                "\n'Tastes abyssmal'");
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
            ItemID.Sets.DrinkParticleColors[Item.type] = new Color[3] {
                new Color(0, 0, 0),
                new Color(0, 0, 0),
                new Color(0, 0, 0)
            };
            ItemID.Sets.IsFood[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 44, ModContent.BuffType<WellFed4>(), 18000, true);
            Item.value = 8000;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override bool? UseItem(Player player)
        {
            player.AddBuff(BuffID.Darkness, 600);
            return base.UseItem(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AbyssBloskus>())
                .AddIngredient(ItemID.BlackCurrant, 2)
                .AddIngredient(ModContent.ItemType<Olives>(), 2)
                .AddIngredient(ItemID.BottledWater)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}