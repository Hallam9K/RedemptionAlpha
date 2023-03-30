using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.WorldGeneration.Misc
{
    public class EnterPlayground : ModItem
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("THINGY TO ENTER THE THING");
            // Tooltip.SetDefault("Sends all players to the Playground");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 30;
            Item.noUseGraphic = true;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.UseSound = SoundID.NPCDeath62;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }
        public override bool? UseItem(Player player)
        {
            if (!SubworldSystem.AnyActive<Redemption>())
                SubworldSystem.Enter<PlaygroundSub>();
            if (SubworldSystem.IsActive<PlaygroundSub>())
                SubworldSystem.Exit();
            return true;
        }
    }
}