using Redemption.Buffs;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class CrystalSerum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anti-Crystallizer Serum");
            Tooltip.SetDefault("Makes you immune to Xenomite for a while"
                + "\n'Label says 'Do not swallow.' Why would you do that?'");
        }

        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;           
            Item.useStyle = ItemUseStyleID.EatFood;               
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.maxStack = 30;                
            Item.consumable = true;          
            Item.width = 12;
            Item.height = 38;
            Item.value = 100;
            Item.rare = ItemRarityID.Lime;
            Item.buffType = ModContent.BuffType<AntiXenomiteBuff>();    
            Item.buffTime = 500;    
            return;
        }
    }
}