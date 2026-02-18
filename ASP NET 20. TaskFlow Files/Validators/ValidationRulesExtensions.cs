using FluentValidation;
using System.Text.RegularExpressions;

namespace ASP_NET_20._TaskFlow_Files.Validators;
public static class ValidationRulesExtensions
{ 
    public static IRuleBuilderOptions<T, string> Password<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        bool mustContainsLowerCase = true,
        bool mustContainsUpperCase = true,
        bool mustContainsDigit = true
        )
    {
        return ruleBuilder.Must(passwword =>
        {
            if (mustContainsLowerCase && !Regex.IsMatch(passwword, @"[a-z]"))
                return false;

            if (mustContainsUpperCase && !Regex.IsMatch(passwword, @"[A-Z]"))
                return false;

            if (mustContainsDigit && !Regex.IsMatch(passwword, @"\d"))
                return false;

            return true;
        });
    }
}
