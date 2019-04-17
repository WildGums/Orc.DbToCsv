// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReaderBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DataAccess
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;

    public abstract class ReaderBase : IReader
    {
        #region Fields
        protected readonly string Source;
        #endregion

        #region Constructors
        protected ReaderBase(string source, int offset = 0, int fetchCount = 0)
        {
            Argument.IsNotNullOrWhitespace(() => source);

            Source = source;
            ValidationContext = new ValidationContext();
            Offset = offset;
            FetchCount = fetchCount;
        }
        #endregion

        #region Properties
        public IValidationContext ValidationContext { get; }
        public abstract string[] FieldHeaders { get; }
        public abstract object this[int index] { get; }
        public abstract object this[string name] { get; }
        public abstract int TotalRecordCount { get; }
        public CultureInfo Culture { get; set; }
        public int Offset { get; set; }
        public int FetchCount { get; set; }
        #endregion

        #region IReader Members
        public abstract bool Read();

        public virtual async Task<bool> NextResultAsync()
        {
            return false;
        }

        public abstract void Dispose();
        #endregion

        #region Methods
        protected void AddValidationError(string message)
        {
            ValidationContext.AddValidationError(message, $"DataSource: '{Source}'");
        }
        #endregion
    }
}
