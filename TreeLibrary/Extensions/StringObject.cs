using Microsoft.VisualBasic;

namespace TreeLibrary.Extensions
{
    /// <summary>
    /// 字符串处理辅助类
    /// </summary>
    public static class StringObject
    {

        /// <summary>
        /// 获取中文字符的首英文字符
        /// </summary>
        /// <param name="c">要获取的字符</param>
        /// <returns>如果是中文字符，返回首写英文字符，否则返回本身</returns>
        public static string GetChineseFirstChar(char c)
        {
            int num = Strings.Asc(c);
            int num2 = 65536 + num;
            if (num2 > 45217 && num2 <= 45252)
            {
                return "A";
            }

            if (num2 > 45253 && num2 <= 45760)
            {
                return "B";
            }

            if (num2 > 45761 && num2 <= 46317)
            {
                return "C";
            }

            if (num2 > 46318 && num2 <= 46825)
            {
                return "D";
            }

            if (num2 > 46826 && num2 <= 47009)
            {
                return "E";
            }

            if (num2 > 47010 && num2 <= 47296)
            {
                return "F";
            }

            if (num2 > 47297 && num2 <= 47613)
            {
                return "G";
            }

            if (num2 > 47614 && num2 <= 48118)
            {
                return "H";
            }

            if (num2 > 48119 && num2 <= 49061)
            {
                return "J";
            }

            if (num2 > 49062 && num2 <= 49323)
            {
                return "K";
            }

            if (num2 > 49324 && num2 <= 49895)
            {
                return "L";
            }

            if (num2 > 49896 && num2 <= 50370)
            {
                return "M";
            }

            if (num2 > 50371 && num2 <= 50613)
            {
                return "N";
            }

            if (num2 > 50614 && num2 <= 50621)
            {
                return "O";
            }

            if (num2 > 50622 && num2 <= 50905)
            {
                return "P";
            }

            if (num2 > 50906 && num2 <= 51386)
            {
                return "Q";
            }

            if (num2 > 51387 && num2 <= 51445)
            {
                return "R";
            }

            if (num2 > 51446 && num2 <= 52217)
            {
                return "S";
            }

            if (num2 > 52218 && num2 <= 52697)
            {
                return "T";
            }

            if (num2 > 52698 && num2 <= 52979)
            {
                return "W";
            }

            if (num2 > 52980 && num2 <= 53688)
            {
                return "X";
            }

            if (num2 > 53689 && num2 <= 54480)
            {
                return "Y";
            }

            if (num2 > 54481 && num2 <= 62289)
            {
                return "Z";
            }

            return c.ToString();
        }
    }
}