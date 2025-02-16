using Redemption.Globals;
using Redemption.NPCs.Friendly.SpiritSummons;
using Terraria;
using Terraria.ID;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class CruxCardGladestone : BaseCruxCard
    {
        public override void SetStaticDefaults()
        {
            ElementID.ItemEarth[Type] = true;
        }
        public override void SafeSetDefaults()
        {
            SpiritTypes = new int[1] { NPCType<AncientGladestoneGolem_SS>() };
            SoulCost = 18;
            //MoveCost = 3;
            SpiritHealth = new int[1] { 250 };
            SpiritDefense = new int[1] { 20 };
            SpiritDamage = new int[1] { 35 };
            Item.knockBack = 7;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 3, 44, 0);
        }
        public override void SpawnSpirits(Player player)
        {
            for (int i = 0; i < 2; i++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NewSpirit(player, (int)player.Center.X + 10, (int)player.Center.Y + 10, 0);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: SpiritTypes[0]);
            }
        }
    }
}