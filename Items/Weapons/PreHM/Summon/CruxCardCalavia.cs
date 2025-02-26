using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.NPCs.Friendly.SpiritSummons;
using Terraria;
using Terraria.ID;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class CruxCardCalavia : BaseCruxCard
    {
        public override void SetStaticDefaults()
        {
            ElementID.ItemIce[Type] = true;
        }
        public override void SafeSetDefaults()
        {
            BossCard = true;
            SpiritTypes = new int[1] { NPCType<Calavia_SS>() };
            SoulCost = 30;
            //MoveCost = 3;
            SpiritHealth = new int[1] { 3000 };
            SpiritDefense = new int[1] { 17 };
            SpiritDamage = new int[1] { 78 };
            Item.knockBack = 7;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 9, 33, 0);
            Item.Redemption().CanSwordClash = true;
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