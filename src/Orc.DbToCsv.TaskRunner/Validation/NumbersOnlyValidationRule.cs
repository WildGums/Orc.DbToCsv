// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumbersOnlyValidationRule.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.TaskRunner.Validation
{
    using System.Globalization;
    using System.Windows.Controls;

    public class NumbersOnlyValidationRule : ValidationRule
    {
        #region Methods
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var valueString = value.ToString().Replace("%", string.Empty);

            if (string.IsNullOrEmpty(valueString.Trim()))
            {
                valueString = "0";
            }

            var dummy = 0.0;

            if (double.TryParse(valueString, out dummy))
            {
                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "Please enter value in numeric format");
        }
        #endregion
    }
}