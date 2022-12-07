using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Buffs.Pets;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Accessories.HM;
using Redemption.Projectiles.Minions;
using Redemption.Projectiles.Pets;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Redemption.Items.Accessories.PostML
{
    public class Xenoemia : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Makes you become one with the infection");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<DummyPet_Proj>(), ModContent.BuffType<XenoemiaBuff>());
            Item.width = 18;
            Item.height = 36;
            Item.rare = ItemRarityID.Master;
            Item.master = true;
            Item.value = Item.sellPrice(0, 5);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return false;
        }
    }
}