using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Materials.PreHM;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using Redemption.BaseExtension;
using Redemption.Globals.Player;

namespace Redemption.Items.Armor.PreHM.SoulSoldier
{
    [AutoloadEquip(EquipType.Head)]
    public class SoulSoldierHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = false;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 30;
            Item.sellPrice(silver: 30);
            Item.rare = ItemRarityID.Green;
            Item.defense = 3;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SoulSoldierChestplate>() && legs.type == ModContent.ItemType<SoulSoldierGreaves>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+4 increased ritual damage upon your Spirit Level increasing";
            player.GetModPlayer<RitualistPlayer>().soulSoldierBonus = true;
            player.RedemptionPlayerBuff().MetalSet = true;

            if (Main.rand.NextBool(10) && Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) > 1f && !player.rocketFrame)
            {
                int index = Dust.NewDust(new Vector2(player.position.X - player.velocity.X * 2f, player.position.Y - 2f - player.velocity.Y * 2f), player.width, player.height,
                    DustID.Web);
                Main.dust[index].noGravity = true;
                Dust dust = Main.dust[index];
                dust.velocity -= player.velocity * 0.5f;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GraveSteelAlloy>(12)
                .AddIngredient<LostSoul>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}