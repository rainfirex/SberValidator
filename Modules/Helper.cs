using System.Text.RegularExpressions;

namespace SberValidator.Modules
{
    class Helper
    {
        /// <summary>
        /// Возвращает слова в падеже, зависимом от заданного числа
        /// </summary>
        /// <param name="number">Число</param>
        /// <param name="nominative">Именительный падеж (столбец)</param>
        /// <param name="genitive">Родительный падеж (столбца)</param>
        /// <param name="plural">Множество (столбцов)</param>
        /// <returns></returns>
        public static string Declension(int number, string nominative, string genitive, string plural)
        {
            number = number % 100;


            if (number >= 5 && number <= 20) return plural;

            var i =  number % 10;

            switch(i)
            {
                case 1:
                    return nominative;
                case 2:
                case 3:
                case 4:
                    return genitive;
                default:
                    return plural;

            }
        }

        /// <summary>
        /// Валидация строки
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool Validate(string value, string pattern)
        {
            Match match = new Regex(pattern).Match(value);
            return match.Success;
        }
    }
}
