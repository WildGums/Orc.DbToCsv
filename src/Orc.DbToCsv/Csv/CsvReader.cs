// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvReader.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.DbToCsv.Csv
{
    using System;
    using System.IO;
    using Catel;
    using Catel.Logging;
    using DataAccess;
    using FileSystem;
    using Orc.Csv;

    public class CsvReader : ReaderBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ICsvReaderService _csvReaderService;
        private readonly IFileService _fileService;

        private int _currentOffset;
        private int _fetchedCount;
        private CsvHelper.CsvReader _reader;
        #endregion

        #region Constructors
        public CsvReader(string source, ICsvReaderService csvReaderService, IFileService fileService)
            : base(source)
        {
            Argument.IsNotNullOrWhitespace(() => source);
            Argument.IsNotNull(() => csvReaderService);
            Argument.IsNotNull(() => fileService);

            _csvReaderService = csvReaderService;
            _fileService = fileService;

            Initialize(source);
        }
        #endregion

        #region Properties
        public override string[] FieldHeaders
        {
            get
            {
                if (_isFieldHeaderInitialized)
                {
                    return _reader.Context.HeaderRecord;
                }

                if (_reader.Read())
                {
                    _reader.ReadHeader();

                    _isFieldHeaderInitialized = true;
                }

                return _reader.Context.HeaderRecord;
            }
        }

        public override object this[int index] => _reader[index];
        public override object this[string name] => _reader[name];
        public override int TotalRecordCount => GetRecordCount();
        #endregion

        private bool _isFieldHeaderInitialized = false;

        #region Methods
        public override bool Read()
        {
            if (ReferenceEquals(_reader, null))
            {
                return false;
            }

            try
            {
                if (FetchCount <= 0)
                {
                    return _reader.Read();
                }

                while (_currentOffset < Offset && Offset >= 0 && _reader.Read())
                {
                    _currentOffset++;
                }

                if (_fetchedCount >= FetchCount && FetchCount > 0)
                {
                    return false;
                }

                _fetchedCount++;

                return _reader.Read();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to read file '{Source}'");
                AddValidationError($"Failed to read data: '{ex.Message}'");
                return false;
            }
        }

        public override void Dispose()
        {
            _reader?.Dispose();
        }

        private int GetRecordCount()
        {
            var lineCount = 0;
            using (var reader = File.OpenText(Source))
            {
                while (reader.ReadLine() != null)
                {
                    lineCount++;
                }
            }

            return lineCount;
        }

        private void Initialize(string source)
        {
            if (!_fileService.Exists(source))
            {
                AddValidationError($"File '{source}' not found");
                return;
            }

            try
            {
                var csvContext = new CsvContext<object>
                {
                    Culture = Culture
                };

                _reader = _csvReaderService.CreateReader(source, csvContext);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to initialize reader for data source '{Source}'");
                _reader?.Dispose();
                _reader = null;

                AddValidationError($"Filed to initialize reader: '{ex.Message}'");
            }
        }
        #endregion
    }
}
