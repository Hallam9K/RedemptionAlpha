using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    [AutoloadEquip(EquipType.Neck)]
    public class NecklaceOfPerception : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Necklace of Perception");
            /* Tooltip.SetDefault("Increases movement speed after taking damage\n" +
                "Increases movement speed while an enemy is near\n" +
                "8% increased critical strike chance\n"
                + "Improves vision"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Expert;
            Item.hasVanityEffects = true;
            Item.accessory = true;
            Item.expert = true;
        }
        NPC target;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.nightVision = true;
            player.panic = true;
            player.GetCritChance<GenericDamageClass>() += 8;
            if (RedeHelper.ClosestNPC(ref target, 300, player.Center, true))
                player.AddBuff(BuffID.Panic, 60);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PanicNecklace)
                .AddIngredient<NecklaceOfSight>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
