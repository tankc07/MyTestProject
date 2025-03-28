using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api测试
{
    public class MoneyConverter
    {
        private static readonly string[] ChineseNumbers = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
        private static readonly string[] ChineseUnits = { "", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "兆", "拾", "佰", "仟" };
        private static readonly string[] ChineseDecimalUnits = { "角", "分" };

        public static string ToCHN(double money)
        {
            string sign = "";
            if (money < 0)
            {
                sign = "负";
                money = -money;
            }

            // 将 double 转换为 decimal 进行高精度运算
            decimal decimalMoney = (decimal)money;
            long integerPart = (long)decimalMoney;
            //0.5四舍五入为1, 需要指定MidpointRounding.AwayFromZero, 否则为0
            int decimalPart = (int)Math.Round((decimalMoney - (decimal)integerPart) * 100, MidpointRounding.AwayFromZero);

            string result = sign;

            // 处理整数部分
            if (integerPart == 0)
            {
                result += "零元";
            }
            else
            {
                string integerStr = integerPart.ToString();
                int length = integerStr.Length;
                bool zeroFlag = false;

                for (int i = 0; i < length; i++)
                {
                    int digit = int.Parse(integerStr[i].ToString());
                    int unitIndex = length - i - 1;

                    if (digit == 0)
                    {
                        zeroFlag = true;
                        if (unitIndex % 4 == 0) // 万、亿等单位
                        {
                            result += ChineseUnits[unitIndex];
                            zeroFlag = false;
                        }
                    }
                    else
                    {
                        if (zeroFlag)
                        {
                            result += "零";
                            zeroFlag = false;
                        }
                        result += ChineseNumbers[digit];
                        result += ChineseUnits[unitIndex];
                    }
                }
                result += "元";
            }

            // 处理小数部分
            if (decimalPart > 0)
            {
                int jiao = decimalPart / 10;
                int fen = decimalPart % 10;

                if (jiao > 0)
                {
                    result += ChineseNumbers[jiao];
                    result += ChineseDecimalUnits[0];
                }
                else if (result[result.Length - 1] != '零')
                {
                    result += "零";
                }

                if (fen > 0)
                {
                    result += ChineseNumbers[fen];
                    result += ChineseDecimalUnits[1];
                }
            }
            else
            {
                result += "整";
            }

            return result;
        }
    }
}
