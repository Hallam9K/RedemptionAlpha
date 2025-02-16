using Redemption.NPCs.Friendly.SpiritSummons;
using Terraria;
using Terraria.ID;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class CruxCardTied : BaseCruxCard
    {
        public override void SafeSetDefaults()
        {
            SpiritTypes = new int[1] { NPCType<Asher_SS>() };
            SoulCost = 14;
            SpiritHealth = new int[1] { 418 };
            SpiritDefense = new int[1] { 10 };
            SpiritDamage = new int[1] { 23 };
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 2, 66, 0);
        }
        public override void SpawnSpirits(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NewSpirit(player, (int)player.Center.X + 10, (int)player.Center.Y + 10, 0);
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: SpiritTypes[0]);
        }
    }
}