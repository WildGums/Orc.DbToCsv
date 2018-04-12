// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Int32ProjectProperty.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv
{
    using System.Windows.Markup;

    [ContentProperty("Value")]
    public class Int32ProjectProperty : ProjectProperty
    {
        #region Properties
        public int Value { get; set; }
        #endregion
    }
}