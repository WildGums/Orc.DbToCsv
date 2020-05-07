// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringProjectProperty.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Windows.Markup;

    [ContentProperty(nameof(Value))]
    public class StringProjectProperty : ProjectProperty
    {
        #region Properties
        public string Value { get; set; }
        #endregion
    }
}
