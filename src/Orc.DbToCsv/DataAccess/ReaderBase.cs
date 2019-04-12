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
        #region Constructors
        protected ReaderBase(string source, IValidationContext validationContext)
        {
            Argument.IsNotNullOrWhitespace(() => source);
            Argument.IsNotNull(() => validationContext);

            Source = source;
            _validationContext = validationContext;
        }
        #endregion

        #region Methods
        protected void AddValidationError(string message)
        {
            _validationContext.AddValidationError(message, $"DataSource: '{Source}'");
        }
        #endregion

        #region Fields
        private readonly IValidationContext _validationContext;
        protected readonly string Source;
        #endregion
    }
}
