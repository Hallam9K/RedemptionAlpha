using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;

namespace Redemption.Globals
{
    public class AntiqueDorulCurrency : CustomCurrencySingleCoin
	{
		public AntiqueDorulCurrency(int coinItemID, long currencyCap, string CurrencyTextKey) : base(coinItemID, currencyCap)
		{
			this.CurrencyTextKey = CurrencyTextKey;
			CurrencyTextColor = Color.LightGray;
		}
	}
}