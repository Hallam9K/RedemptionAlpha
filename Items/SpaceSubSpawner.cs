using Redemption.WorldGeneration.Space;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.Items
{
    public class SpaceSubSpawner : ModItem
    {
        public override string Texture => "Redemption/Items/SlayerMedal";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Placeholder Upper Atmosphere/Space Sender of DOOM!!!");
            Tooltip.SetDefault("It reads - [c/b8eff5:'SPAAAAAAAAAAAAAAAAAAAAAAAAAACE']");
        }

        public override void SetDefaults()
        {
            Item.useAnimation = 5;
            Item.useTime = 5;
            Item.UseSound = CustomSounds.Choir;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.width = 16;
            Item.height = 26;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Blue;
        }
        public override bool? UseItem(Player player)
        {
            if (SubworldSystem.IsActive<SpaceSub>())
            {
                SubworldSystem.Exit();
                return true;
            }
            else if (!SubworldSystem.AnyActive<Redemption>())
            {
                Main.rand = new UnifiedRandom();
                SubworldSystem.Enter<SpaceSub>();
            }
            return true;
        }
    }
}