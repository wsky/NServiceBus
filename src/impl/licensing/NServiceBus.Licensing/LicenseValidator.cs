﻿using System;
using System.IO;
using Common.Logging;
using Rhino.Licensing;

namespace NServiceBus.Licensing
{
    public class LicenseValidator : AbstractLicenseValidator
    {
        public LicenseValidator(string publicKey, string licensePath) : base(publicKey)
        {
            _licensePath = licensePath;
        }

        protected override string License
        {
            get
            {
                return _inMemoryLicense ?? File.ReadAllText(_licensePath);
            }
            set
            {
                try
                {
                    File.WriteAllText(_licensePath, value);
                }
                catch (Exception ex)
                {
                    _inMemoryLicense = value;
                    Logger.Warn("Could not write new license value to disk, using in-memory license instead", ex);
                }
            }
        }

        public override void AssertValidLicense()
        {
            if (!File.Exists(_licensePath))
            {
                Logger.WarnFormat("Could not find license file: {0}", _licensePath);
                throw new LicenseFileNotFoundException();
            }
            
            base.AssertValidLicense();
        }

        public override void RemoveExistingLicense()
        {
            File.Delete(_licensePath);
        }

        private string _inMemoryLicense;
        private readonly string _licensePath;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LicenseManager).Namespace);
    }
}