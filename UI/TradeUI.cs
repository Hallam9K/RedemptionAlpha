using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Plants;
using Redemption.Items.Weapons.PreHM.Magic;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.Minibosses.Calavia;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.UI
{
    public class TradeUI : UIState
    {
        private readonly UIImage BgSprite = new(ModContent.Request<Texture2D>(Redemption.EMPTY_TEXTURE));
        public static bool Visible = false;
        public static bool AppendedNymph = false;
        public static bool AppendedCalavia = false;
        public Vector2 lastScreenSize;

        public override void OnInitialize()
        {
            lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);

            BgSprite.Width.Set(200, 0f);
            BgSprite.Height.Set(300, 0f);
            BgSprite.Top.Set((Main.screenHeight / 2f) - 164f, 0f);
            BgSprite.Left.Set((Main.screenWidth / 2f) - 103f, 0f);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface)
                Main.LocalPlayer.mouseInterface = true;

            if (Main.LocalPlayer.talkNPC == -1)
            {
                Visible = false;
                return;
            }
            bool forestNymph = Main.npc[Main.LocalPlayer.talkNPC].type == ModContent.NPCType<ForestNymph_Friendly>();
            bool calavia = Main.npc[Main.LocalPlayer.talkNPC].type == ModContent.NPCType<Calavia_NPC>();
            if (!Main.LocalPlayer.releaseInventory || (!forestNymph && !calavia))
                Visible = false;

            if (forestNymph && !AppendedNymph)
            {
                AppendedCalavia = false;
                BgSprite.RemoveAllChildren();
                int pad = 36;
                BgSprite.Append(new TradePanelUI(new Item(ItemID.HerbBag), new Item(ModContent.ItemType<ForestCore>())));
                BgSprite.Append(new TradePanelUI(new Item(ModContent.ItemType<AnglonicMysticBlossom>()), new Item(ModContent.ItemType<ForestNymphsSickle>())) { Top = new StyleDimension(pad, 0) });
                pad += 36;
                BgSprite.Append(new TradePanelUI(new Item(ItemID.JungleRose), new Item(ItemID.NaturesGift)) { Top = new StyleDimension(pad, 0) });
                pad += 36;
                BgSprite.Append(new TradePanelUI(new Item(ItemID.NaturesGift), new Item(ItemID.JungleRose)) { Top = new StyleDimension(pad, 0) });
                pad += 36;
                BgSprite.Append(new TradePanelUI(new Item(ItemID.Fireblossom, 10), new Item(ItemID.ObsidianRose), 10) { Top = new StyleDimension(pad, 0) });
                pad += 36;
                BgSprite.Append(new TradePanelUI(new Item(ItemID.Blinkroot, 10), new Item(ItemID.MetalDetector), 10) { Top = new StyleDimension(pad, 0) });
                pad += 36;
                BgSprite.Append(new TradePanelUI(new Item(ModContent.ItemType<LostSoul>(), 8), new Item(ItemID.HerbBag), 8) { Top = new StyleDimension(pad, 0) });
                Append(BgSprite);
                AppendedNymph = true;
            }
            else if (calavia && !AppendedCalavia)
            {
                AppendedNymph = false;
                BgSprite.RemoveAllChildren();
                int pad = 36;
                BgSprite.Append(new TradePanelUI(new Item(ModContent.ItemType<Zweihander>()), new Item(ModContent.ItemType<BladeOfTheMountain>())) { Top = new StyleDimension(pad, 0) });
                pad += 36;
                BgSprite.Append(new TradePanelUI(new Item(ModContent.ItemType<Mistfall>()), new Item(ModContent.ItemType<Icefall>())) { Top = new StyleDimension(pad, 0) });
                Append(BgSprite);
                AppendedCalavia = true;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            if (lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                BgSprite.Top.Pixels = (Main.screenHeight / 2f) - 164f;
                BgSprite.Left.Pixels = (Main.screenWidth / 2f) - 103f;
                BgSprite.Recalculate();
            }

            base.Draw(spriteBatch);
        }
    }
}
