using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Usable;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Redemption.Globals
{
    public class AntiqueDorulCurrency : CustomCurrencySingleCoin
    {
        public AntiqueDorulCurrency(int coinItemId, long currencyCap, string currencyTextKey, Color currencyTextColor) : base(coinItemId, currencyCap)
        {
            CurrencyTextKey = currencyTextKey;
            CurrencyTextColor = currencyTextColor;
        }
        public override void DrawSavingsMoney(SpriteBatch sb, string text, float shopx, float shopy, long totalCoins, bool horizontal = false)
        {
            int num = _valuePerUnit.Keys.ElementAt(0);
            Main.instance.LoadItem(num);
            Texture2D value = TextureAssets.Item[num].Value;
            Rectangle rect = Main.itemAnimations[num].GetFrame(value);
            Vector2 origin = rect.Size() / 2f;

            if (horizontal)
            {
                _ = 99;
                Vector2 position = new Vector2(shopx + ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One).X + 45f, shopy + 50f);
                sb.Draw(value, position, rect, Color.White, 0f, origin, CurrencyDrawScale, SpriteEffects.None, 0f);
                Utils.DrawBorderStringFourWay(sb, FontAssets.ItemStack.Value, totalCoins.ToString(), position.X - 11f, position.Y, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
            }
            else
            {
                int num2 = ((totalCoins > 99) ? (-6) : 0);
                sb.Draw(value, new Vector2(shopx + 11f, shopy + 75f), rect, Color.White, 0f, origin, CurrencyDrawScale, SpriteEffects.None, 0f);
                Utils.DrawBorderStringFourWay(sb, FontAssets.ItemStack.Value, totalCoins.ToString(), shopx + (float)num2, shopy + 75f, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
            }
        }
    }
    public sealed class RedeCurrency : ModSystem
    {
        public static int AntiqueDorulCurrency { get; set; }

        public override void PostSetupContent()
        {
            AntiqueDorulCurrency = CustomCurrencyManager.RegisterCurrency(new AntiqueDorulCurrency(ItemType<AncientGoldCoin>(), 999, "Mods.Redemption.Currencies.AntiqueDorulCurrency", new Color(208, 200, 48)));
        }
    }
}