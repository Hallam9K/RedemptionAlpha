using Redemption.Items.Accessories.HM;
using Redemption.Tiles.Ores;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Materials.PostML
{
    public class Corium : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Holding this will cause severe radiation poisoning without proper equipment");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Red;
            Item.value = 40000;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<SolidCoriumTile>();
        }
        public override void HoldItem(Player player)
        {
            if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Muller5").WithVolume(.9f).WithPitchVariance(.1f), player.position);

            if (Main.rand.NextBool(50) && player.RedemptionRad().irradiatedLevel < 5)
                player.RedemptionRad().irradiatedLevel += 2;
        }
    }
}
