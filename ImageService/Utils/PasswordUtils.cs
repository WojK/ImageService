namespace ImagesService.Utils
{
    public class PasswordUtils
    {

        public static double CountEntropy(string password)
        {
            bool hasLowerLetter = false;
            bool hasUpperLetter = false;
            bool hasDigit = false;
            bool hasSpecialChar = false;
            int poolSize = 0;

            foreach (char c in password)
            {
                if (char.IsLower(c))
                {
                    hasLowerLetter = true;
                }
                else if (char.IsUpper(c))
                {
                    hasUpperLetter = true;
                }
                else if (char.IsDigit(c))
                {
                    hasDigit = true;
                }
                else
                {
                    hasSpecialChar = true;
                }
            }

            if (hasLowerLetter)
            {
                poolSize += 26;
            }
            if (hasUpperLetter)
            {
                poolSize += 26;
            }
            if (hasDigit)
            {
                poolSize += 10;
            }
            if (hasSpecialChar)
            {
                poolSize += 32;
            }

            double entropy = password.Length * Math.Log2(poolSize);
            return entropy;
        }

        // For non-vital accounts, 25-30 bits of entropy are enough.For more important accounts, aim for 60-80 bits of entropy, up to 100 for crucial ones.
        // A password is secure if it has at least 50 bits of entropy and does not appear in any list of leaked passwords.
    }
}

