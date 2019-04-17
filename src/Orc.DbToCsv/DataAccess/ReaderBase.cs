// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReaderBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DataAccess
{
    using Catel;
    using Catel.Data;

    public abstract class ReaderBase
    {
        #region Fields
        protected readonly string Source;
        #endregion

        #region Constructors
        protected ReaderBase(string source)
        {
            Argument.IsNotNullOrWhitespace(() => source);

            Source = source;
            ValidationContext = new ValidationContext();
        }
        #endregion

        #region Properties
        public IValidationContext ValidationContext { get; }
        #endregion

        #region Methods
        protected void AddValidationError(string message)
        {
            ValidationContext.AddValidationError(message, $"DataSource: '{Source}'");
        }
        #endregion
    }
}
