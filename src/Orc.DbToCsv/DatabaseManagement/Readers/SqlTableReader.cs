// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlTableReader.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.DatabaseManagement
{
    using System;
    using System.Data.Common;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.Logging;
    using DataAccess;

    public class SqlTableReader : ReaderBase, IReader
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly string _source;

        private DbConnection _connection;
        private DatabaseSource _databaseSource;
        private bool _isFieldHeadersInitialized;
        private bool _isInitialized;
        private DbDataReader _reader;
        private string[] _fieldHeaders = new string[0];
        #endregion

        #region Constructors
        public SqlTableReader(string source, IValidationContext validationContext, int offset = 0, int fetchCount = 0)
            : base(source, validationContext)
        {
            Argument.IsNotNullOrEmpty(() => source);

            _source = source;
            Offset = offset;
            FetchCount = fetchCount;
        }
        #endregion

        #region Properties
        public string[] FieldHeaders
        {
            get
            {
                if (_isFieldHeadersInitialized)
                {
                    return _fieldHeaders;
                }

                TryInitialize();
                return _fieldHeaders;
            }

            private set => _fieldHeaders = value;
        }

        public object this[int index] => GetValue(index);
        public object this[string name] => GetValue(name);
        public virtual int TotalRecordCount { get; private set; }
        public CultureInfo Culture { get; set; }
        public int Offset { get; set; }
        public int FetchCount { get; set; }
        public int ReadCount { get; private set; } = 0;
        public int ResultIndex { get; private set; } = 0;
        public DbQueryParameters QueryParameters { get; set; }
        #endregion

        #region IReader Members
        public object GetValue(int index) => _reader[index];
        public object GetValue(string name) => _reader[name];

        public bool HasRows => _reader.HasRows;

        public async Task<bool> NextResultAsync()
        {
            var result = await _reader.NextResultAsync();
            if (result)
            {
                ResultIndex++;
                ReadCount = 0;
                _isFieldHeadersInitialized = false;
            }

            return result;
        }

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader.Dispose();
            }

            if (_connection == null)
            {
                return;
            }

            _connection.Close();
            _connection.Dispose();
        }

        public bool Read()
        {
            try
            {
                TryInitialize();

                var readResult = _reader.Read();
                if (readResult)
                {
                    ReadCount++;
                }

                return readResult;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to read source '{Source}'");
                AddValidationError($"Failed to read data: '{ex.Message}'");
                return false;
            }
        }

        public async Task<bool> ReadAsync()
        {
            try
            {
                TryInitialize();

                var readResult = await _reader.ReadAsync();
                if (readResult)
                {
                    ReadCount++;
                }

                return readResult;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to read source '{Source}'");
                AddValidationError($"Failed to read data: '{ex.Message}'");
                return false;
            }
        }

        private void TryInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
            }

            if (ReferenceEquals(_reader, null))
            {
                InitializeReader();
            }

            if (!_isFieldHeadersInitialized)
            {
                InitializeFieldHeaders();
                _isFieldHeadersInitialized = true;
            }
        }
        #endregion

        #region Methods
        private void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            try
            {
                _databaseSource = new DatabaseSource(_source);
                _connection = _databaseSource.CreateConnection();
                //if (_connection == null)
                //{
                //    AddValidationError(errorMessage);
                //}

               //
               // TotalRecordCount = _dbManager.GetRecordCount(DatabaseSource.Parse(Source, false), QueryParameters, out errorMessage);
              
               //if (!string.IsNullOrWhiteSpace(errorMessage))
                //{
                //    AddValidationError(errorMessage);
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to initialize reader for data source '{Source}'");

                AddValidationError($"Filed to initialize reader: '{ex.Message}'");
            }
            finally
            {
                _isInitialized = true;
            }
        }

        private void InitializeReader()
        {
            try
            {
               _reader = _connection.GetRecords(_databaseSource, Offset, FetchCount, QueryParameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to initialize SelectAllReader for data source '{Source}'");
                _reader?.Dispose();
                _reader = null;

                AddValidationError($"Filed to initialize reader: '{ex.Message}'");
            }
        }

        private void InitializeFieldHeaders()
        {
            if (_reader is null)
            {
                return;
            }

            _fieldHeaders = Enumerable.Range(0, _reader.FieldCount).Select(_reader.GetName).ToArray();

#if DEBUG
            Log.Debug($"'{_fieldHeaders.Length}' headers of table were read");
#endif
        }
        #endregion
    }
}
