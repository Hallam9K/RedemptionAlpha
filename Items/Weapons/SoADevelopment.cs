using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons
{
    public class SoADevelopment : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 100000;
            Item.DamageType = DamageClass.Melee;
            Item.width = 34;
            Item.height = 24;
            Item.useTime = 600;
            Item.useAnimation = 600;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 50f;
            Item.crit = 94;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
            Item.useTurn = false;
        }
    }
}
