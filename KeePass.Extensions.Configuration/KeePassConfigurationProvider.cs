using System;
using System.Collections.Generic;
using KeePassLib;
using Microsoft.Extensions.Configuration;

namespace KeePass.Extensions.Configuration
{
    /// <summary>
    /// Provides configuration key/values from KeePass.
    /// </summary>
    /// <seealso cref="ConfigurationProvider" />
    public class KeePassConfigurationProvider : ConfigurationProvider
    {
        #region Fields

        /// <summary>
        /// The configuration source.
        /// </summary>
        private readonly KeePassConfigurationSource _configurationSource;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeePassConfigurationProvider" /> class.
        /// </summary>
        /// <param name="configurationSource">The configuration source.</param>
        public KeePassConfigurationProvider(KeePassConfigurationSource configurationSource)
        {
            _configurationSource = configurationSource;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Loads configuration values from the source represented by this <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" />.
        /// </summary>
        public override void Load()
        {
            var database = new PwDatabase();
            database.Open(_configurationSource.Connection, _configurationSource.CompositeKey, null);

            var entries = _configurationSource.FilterEntries != null 
                ? _configurationSource.FilterEntries(database) 
                : database.RootGroup.GetEntries(true);

            var resolveKey = _configurationSource.ResolveKey ?? ((entry) =>
            {
                var keyPath = entry.ParentGroup.GetFullPath(":", true);
                var title = entry.Strings.ReadSafe("Title");
                var userName = entry.Strings.ReadSafe("UserName");
                return $"{keyPath}:{title}:{userName}";
            });

            var resolveValue = _configurationSource.ResolveValue ?? ((key, entry) =>
            {
                var value = entry.Strings.ReadSafe("Password");
                return value == "Password" ? null : value;
            });

            foreach(var entry in entries)
            {
                var key = resolveKey(entry);
                var value = resolveValue(key, entry);

                if (!Data.TryAdd(key, value))
                {
                    for (var i = 1; i < int.MaxValue; i++)
                    {
                        var incrementKey = $"{key}`{i}";
                        if (Data.TryAdd(incrementKey, value))
                            break;
                    }
                }
            }
        }

        #endregion
    }
}
