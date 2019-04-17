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
    using Catel.Logging;
    using DataAccess;

    public class SqlTableReader : ReaderBase, IReader
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly DatabaseSource _databaseSource;

        private string[] _fieldHeaders = new string[0];
        private DbSourceGatewayBase _gateway;
        private bool _isFieldHeadersInitialized;
        private bool _isInitialized;
        private DbDataReader _reader;
        #endregion

        #region Constructors
        public SqlTableReader(string source, int offset = 0, int fetchCount = 0)
            : this(new DatabaseSource(source), offset, fetchCount)
        {
        }

        public SqlTableReader(DatabaseSource source, int offset = 0, int fetchCount = 0)
            : base(source.ToString())
        {
            Argument.IsNotNull(() => source);

            _databaseSource = source;
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
        public bool HasRows => _reader.HasRows;
        #endregion

        #region IReader Members
        public async Task<bool> ReadAsync()
        {
            try
            {
                TryInitialize();
                if (_reader == null)
                {
                    return false;
                }

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
        public bool Read()
        {
            try
            {
                TryInitialize();

                if (_reader == null)
                {
                    return false;
                }

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

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader.Dispose();
            }

            if (_gateway == null)
            {
                return;
            }

            _gateway.Close();
            _gateway.Dispose();
        }
        #endregion

        #region Methods
        public object GetValue(int index) => _reader[index];
        public object GetValue(string name) => _reader[name];

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

        private void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            try
            {
                _gateway = _databaseSource.CreateGateway();
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
                _reader = _gateway.GetRecords(QueryParameters, Offset, FetchCount);
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
