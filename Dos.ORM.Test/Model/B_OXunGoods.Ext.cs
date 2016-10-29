using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WD.Model
{
	partial class B_OXunGoods
	{
		/// <summary>
		/// 所属行业
		/// </summary>
		public string IndustryName { set; get; }
		/// <summary>
		/// 所属行业2
		/// </summary>
		public string IndustryName2 { set; get; }
		/// <summary>
		/// <summary>
		/// 所属城市
		/// </summary>
		public string BussAreaName { set; get; }
		/// <summary>
		/// 所属商圈
		/// </summary>
		public string TradingName { set; get; }
		public string TradingAreaCode
		{
			set; get;
		}
		public string GoodsTypeName
		{
			get
			{

				return "";
			}
		}
		public string BuyStartTimeStr
		{
			get
			{
				if (BuyStartTime == null)
					return "";
				else return BuyStartTime.ToString();
			}
		}
		public string BuyEndTimeStr
		{
			get
			{
				if (BuyEndTime == null)
					return "";
				else return BuyEndTime.ToString();
			}
		}
		public string CreateTimeStr
		{
			get
			{
				return CreateTime.ToString();
			}
		}
		public string UpdateTimeStr
		{
			get
			{
				return UpdateTime.ToString();
			}
		}
		public string ValidEndTimeStr
		{
			get
			{
				return ValidEndTime.ToString();
			}
		}
		public string ValidStatTimeStr
		{
			get { return ValidStatTime.ToString(); }
		}
	}
}
