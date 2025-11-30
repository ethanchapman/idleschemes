namespace IdleSchemes.Core.Helpers {
    public class MoneyHelper {

        private const int GROUP_LENGTH = 3;

        public static string ToString(int amount, string symbol = "$", string dec = ".", string separator = ",") {
            var dollars = AddSeparators(amount / 100, separator);
            var cents = (amount % 100).ToString().PadLeft(2, '0');
            return symbol + dollars + dec + cents;
        }

        private static string AddSeparators(int amount, string separator = ",") {
            string str = amount.ToString();
            int fullGroupsCount = str.Length / GROUP_LENGTH;
            if(fullGroupsCount == 0) {
                return str;
            }

            int lastGroupDigits = str.Length % GROUP_LENGTH;
            List<string> groups = new List<string>();
            groups.Add(str.Substring(0, lastGroupDigits));
            for(int i = 0; i < fullGroupsCount; i++) {
                groups.Add(str.Substring(lastGroupDigits + (i * GROUP_LENGTH)));
            }
            return string.Join(separator, groups);
        }
    }
}
