using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace KeePass.Extensions.Configuration
{
    /// <summary>
    /// Registers configuration key/values from KeePass.
    /// </summary>
    /// <seealso cref="IConfigurationProvider" />
    public class KeePassConfigurationSource : FileConfigurationSource
    {
        #region Fields

        /// <summary>
        /// The KeePass database connection.
        /// </summary>
        public IOConnectionInfo Connection { get; set; }

        /// <summary>
        /// The composite key.
        /// </summary>
        public CompositeKey CompositeKey { get; set; }

        /// <summary>
        /// Gets or sets the filter entries.
        /// </summary>
        /// <value>
        /// The filtered entries.
        /// </value>
        public Func<PwDatabase, IEnumerable<PwEntry>> FilterEntries { get; set; }

        /// <summary>
        /// The mapping of an entry to a key in the configuration provider.
        /// </summary>
        /// <value>
        /// The resolve key.
        /// </value>
        public Func<PwEntry, string> ResolveKey { get; set; }

        /// <summary>
        /// Retrieves a value from an entry.
        /// </summary>
        public Func<string, PwEntry, string> ResolveValue { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Builds the <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" /> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</param>
        /// <returns>
        /// An <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" />
        /// </returns>
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            FileProvider = FileProvider ?? builder.GetFileProvider();
            return new KeePassConfigurationProvider(this);
        }

        #endregion
    }
}
