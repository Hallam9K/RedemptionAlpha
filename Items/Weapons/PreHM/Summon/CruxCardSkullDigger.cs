using Redemption.Globals;
using Redemption.NPCs.Friendly.SpiritSummons;
using Terraria;
using Terraria.ID;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class CruxCardSkullDigger : BaseCruxCard
    {
        public override void SetStaticDefaults()
        {
            ElementID.ItemArcane[Type] = true;
        }
        public override void SafeSetDefaults()
        {
            BossCard = true;
            SpiritTypes = new int[1] { NPCType<SkullDigger_SS>() };
            SoulCost = 30;
            //MoveCost = 2;
            SpiritHealth = new int[1] { 2400 };
            SpiritDefense = new int[1] { 0 };
            SpiritDamage = new int[1] { 30 };
            SpawnDustSize = 4;
            Item.knockBack = 3;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 9, 33, 0);
        }
        public override void SpawnSpirits(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NewSpirit(player, (int)player.Center.X + 10, (int)player.Center.Y, 0);
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: SpiritTypes[0]);
        }
    }
}