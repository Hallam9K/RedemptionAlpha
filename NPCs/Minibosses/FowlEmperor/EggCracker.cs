using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.FowlEmperor
{
    public class EggCracker : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Stuns the panicking Fowl Emperor\n" +
                "Disappears if the Fowl Emperor isn't alive"); */
            Item.ResearchUnlockCount = 99;
        }
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 24;
            Item.damage = 40;
            Item.knockBack = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 0;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.UseSound = SoundID.Item1;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Default;
            Item.shootSpeed = 13f;
            Item.shoot = ModContent.ProjectileType<EggCracker_Proj>();
        }
        public override void UpdateInventory(Player player)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<FowlEmperor>()))
            {
                int egg = player.FindItem(Type);
                if (egg >= 0)
                {
                    player.inventory[egg].stack = 0;
                    if (player.inventory[egg].stack <= 0)
                        player.inventory[egg] = new Item();
                }
            }
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<FowlEmperor>()))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Item.active = false;
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Item.whoAmI);
                }
            }
        }
    }
}
