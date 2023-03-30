using Terraria.Audio;
using Microsoft.Xna.Framework;
using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace Redemption.Items.Accessories.HM
{
    [AutoloadEquip(EquipType.Wings)]
    public class XenomiteJetpack : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Allows flight and slow fall");
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(90, 7f, 2.5f);
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = 300000;
            Item.rare = ItemRarityID.Pink;
            Item.hasVanityEffects = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.flapSound = false;
        }
        public override bool WingUpdate(Player player, bool inUse)
        {
            if (inUse)
            {
                if (++player.wingFrameCounter >= 12)
                {
                    player.wingFrameCounter = 0;
                    SoundEngine.PlaySound(SoundID.Item13 with { Volume = .6f }, player.position);
                }
                player.wingFrame = 1 + (player.wingFrameCounter / 4);
                Color color = player.wingFrame switch
                {
                    2 => new Color(212, 246, 187),
                    3 => new Color(253, 242, 170),
                    _ => new Color(199, 253, 230),
                };
                Lighting.AddLight(player.Center, 255f / color.R * .3f, 255f / color.G * .3f, 255f / color.B * .3f);
            }
            else
                player.wingFrame = 0;
            return true;
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 1.7f;
            constantAscend = 0.135f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Xenomite>(), 20)
            .AddIngredient(ItemID.TitaniumBar, 5)
            .AddIngredient(ItemID.SoulofFlight, 20)
            .AddTile(TileID.MythrilAnvil)
            .SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1))
            .Register();
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Xenomite>(), 20)
            .AddIngredient(ItemID.AdamantiteBar, 5)
            .AddIngredient(ItemID.SoulofFlight, 20)
            .AddTile(TileID.MythrilAnvil)
            .SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1))
            .Register();
        }
    }
}
