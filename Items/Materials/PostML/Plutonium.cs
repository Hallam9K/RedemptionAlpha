using Redemption.Globals.Player;
using Redemption.Tiles.Ores;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class Plutonium : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Plutonium");
            // Tooltip.SetDefault("Right-click to recharge +30 Energy if an Energy Pack is in your inventory");
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PlutoniumTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 10000;
            Item.rare = ItemRarityID.Cyan;
        }
        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<EnergyPlayer>().energyMax > 0 && Main.LocalPlayer.GetModPlayer<EnergyPlayer>().statEnergy < Main.LocalPlayer.GetModPlayer<EnergyPlayer>().energyMax;
        public override void RightClick(Player player)
        {
            SoundEngine.PlaySound(CustomSounds.Spark1 with { Pitch = 0.2f }, player.position);
            player.GetModPlayer<EnergyPlayer>().statEnergy += 30;
        }
    }
}