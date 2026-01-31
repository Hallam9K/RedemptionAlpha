using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Players;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Redemption.Items.Armor.PostML.Xenium
{
    [AutoloadEquip(EquipType.Head)]
    public class XeniumVisor : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Purple;
            Item.defense = 22;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<XeniumBreastplate>() && legs.type == ItemType<XeniumLeggings>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<XeniumAlloy>(12)
            .AddIngredient<CarbonMyofibre>(4)
            .AddTile<XeniumRefineryTile>()
            .Register();
        }


        //modified code from calamity & slr
        public int cannonTimer = 480;
        public bool CannonOn = false;
        internal static Item dummyCannon = new();
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += .10f;
            player.GetCritChance<GenericDamageClass>() += 15;
            if (!IsArmorSet(player))
            {
                CannonOn = false;
            }
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.Redemption.GenericTooltips.ArmorSetBonus.Xenium.Bonus");
            player.RedemptionPlayerBuff().xeniumBonus = true;
            player.GetModPlayer<EnergyPlayer>().energyRegen += 15;

            if (CannonOn)
            {
                if (cannonTimer-- <= 0)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.ShootChange with { Pitch = -.2f }, player.position);

                    CannonOn = false;
                }

                if (dummyCannon.IsAir)
                    dummyCannon.SetDefaults(ItemType<XeniumGrenadeCannon>());
                if (Main.myPlayer == player.whoAmI)
                {
                    if (Main.mouseItem.type != dummyCannon.type && !Main.mouseItem.IsAir && !(player.inventory[58] == dummyCannon))
                    {
                        Main.LocalPlayer.QuickSpawnItem(null, Main.mouseItem, Main.mouseItem.stack);
                    }
                    Main.mouseItem = dummyCannon;
                }
                player.inventory[58] = dummyCannon;
                player.selectedItem = 58;
            }
            else if (Main.myPlayer == player.whoAmI)
            {
                if (Main.mouseItem.type == ItemType<XeniumGrenadeCannon>())
                    Main.mouseItem = new Item();

                cannonTimer = 480;
                dummyCannon.TurnToAir();
            }
        }

        public static bool IsArmorSet(Player Player)
        {
            return Player.armor[0].type == ItemType<XeniumVisor>() && Player.armor[1].type == ItemType<XeniumBreastplate>() && Player.armor[2].type == ItemType<XeniumLeggings>();
        }
        private void ActivateShoulderCannon(On_Player.orig_KeyDoubleTap orig, Player player, int keyDir)
        {
            if (keyDir == 0 && player.RedemptionPlayerBuff().xeniumBonus)
            {
                var helm = player.armor[0].ModItem as XeniumVisor;

                if (helm.CannonOn)
                {
                    if (!Main.dedServ)
                    {
                        SoundEngine.PlaySound(CustomSounds.ShootChange with { Pitch = -.1f }, player.position);
                        DustHelper.DrawDustImage(player.Center, DustID.GreenFairy, 0.1f, "Redemption/Effects/DustImages/WarpShape", 2, true, 0);
                    }
                    helm.CannonOn = false;
                    dummyCannon.TurnToAir();
                }
                else
                {
                    if (player.itemTime == 0)
                    {
                        player.releaseUseItem = false;
                        player.controlUseItem = false;

                        if (!Main.dedServ)
                        {
                            SoundEngine.PlaySound(CustomSounds.ShootChange with { Pitch = -.2f }, player.position);
                            DustHelper.DrawDustImage(player.Center, DustID.GreenFairy, 0.1f, "Redemption/Effects/DustImages/WarpShape", 2, true, 0);
                        }
                        dummyCannon.SetDefaults(ItemType<XeniumGrenadeCannon>());
                        helm.CannonOn = true;
                    }
                }
            }

            orig(player, keyDir);
        }

        private void SpoofMouseItem(On_Main.orig_DrawPendingMouseText orig)
        {
            if (dummyCannon.IsAir && !Main.gameMenu)
                dummyCannon.SetDefaults(ItemType<XeniumGrenadeCannon>());

            orig();
        }
        public static bool Activate(Player player)
        {
            if (IsArmorSet(player))
            {
                var helm = player.armor[0].ModItem as XeniumVisor;
                return helm.CannonOn;
            }
            return false;
        }

        public override void Load()
        {
            On_Player.KeyDoubleTap += ActivateShoulderCannon;
            On_Main.DrawPendingMouseText += SpoofMouseItem;
        }

        public override void Unload()
        {
            On_Player.KeyDoubleTap -= ActivateShoulderCannon;
            On_Main.DrawPendingMouseText -= SpoofMouseItem;
            dummyCannon.TurnToAir();
            dummyCannon = null;
        }
    }
}
