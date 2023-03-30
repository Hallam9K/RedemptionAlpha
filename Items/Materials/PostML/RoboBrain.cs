using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class RoboBrain : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Obliterator Brain");
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 40, 0);
            Item.rare = ItemRarityID.Red;
        }
        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Red.ToVector3() * 0.6f * Main.essScale);
        }
    }
}
