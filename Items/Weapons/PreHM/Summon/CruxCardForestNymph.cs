using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.NPCs.Friendly.SpiritSummons;
using Terraria;
using Terraria.ID;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class CruxCardForestNymph : BaseCruxCard
    {
        public override void SetStaticDefaults()
        {
            ElementID.ItemNature[Type] = true;
        }
        public override void SafeSetDefaults()
        {
            SpiritTypes = new int[1] { NPCType<ForestNymph_SS>() };
            SoulCost = 12;
            SpiritHealth = new int[1] { 500 };
            SpiritDefense = new int[1] { 5 };
            SpiritDamage = new int[1] { 28 };
            Item.knockBack = 5;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 2, 66, 0);
            Item.Redemption().TechnicallySlash = true;
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