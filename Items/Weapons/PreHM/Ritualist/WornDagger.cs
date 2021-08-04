using Redemption.DamageClasses;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{ 
    public class WornDagger : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = ModContent.GetInstance<RitualistClass>();
            Item.width = 44;
            Item.height = 48;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.autoReuse = true;
            Item.damage = 8;
            Item.knockBack = 4;
            Item.UseSound = SoundID.Item1;
        }
    }
}