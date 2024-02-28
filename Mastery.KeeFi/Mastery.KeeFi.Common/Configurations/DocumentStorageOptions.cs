using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mastery.KeeFi.Common.Configurations
{
    public sealed class DocumentStorageOptions
    {
        public const string ConfigKey = "DocumentStorage";

        public required string ClientPath { get; set; }

        public required string DocumentPath { get; set; }

        public required string DocumentBlobPath { get; set; }
    }
}
