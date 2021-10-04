using Microsoft.Xna.Framework;
using Redemption.Projectiles.Magic;
using Redemption.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class Shadeburst : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadeburst");
            Tooltip.SetDefault("Conjures a candle");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 950;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 30;
            Item.height = 34;
            Item.useTime = 96;
            Item.useAnimation = 96;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = 2500;
            Item.channel = true;
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ShadeburstCandle>();
            Item.shootSpeed = 0f;
        }    
    }
}