using System;
using System.Collections.Generic;
using System.IO;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace KeePass.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for KeePass configuration.
    /// </summary>
    public static class KeePassConfigurationExtensions
    {
        #region Methods

        /// <summary>
        /// Adds KeePass configuration source to the <code>builder</code>.
        /// </summary>
        /// <param name="builder">The configuration builder.</param>
        /// <param name="path">The path to the KeePass database file (KDBX).</param>
        /// <param name="masterPassword">The master password.</param>
        /// <param name="useCurrentWindowsAccount">if set to <c>true</c> [use current windows account].</param>
        /// <param name="filterEntries">Filter used to narrow the selection of entries loaded by the KeePass configuration provider.</param>
        /// <param name="resolveKey">The entry mapping to a string value used as the key in configuration lookups.</param>
        /// <param name="resolveValue">The entry mapping to a string value used as the value in configuration lookups.</param>
        /// <returns>The same <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder AddKeePass(this IConfigurationBuilder builder, 
            string path, 
            string masterPassword = null, 
            bool useCurrentWindowsAccount = false, 
            Func<PwDatabase, IEnumerable<PwEntry>> filterEntries = null,
            Func<PwEntry, string> resolveKey = null,
            Func<string, PwEntry, string> resolveValue = null)
        {
            var compositeKey = new CompositeKey();

            if (masterPassword != null)
                compositeKey.AddUserKey(new KcpPassword(masterPassword));

            if (useCurrentWindowsAccount)
                compositeKey.AddUserKey(new KcpUserAccount());

            return builder.AddKeePass(path, compositeKey, filterEntries: filterEntries, resolveKey: resolveKey, resolveValue: resolveValue);
        }

        /// <summary>
        /// Adds KeePass configuration source to the <code>builder</code>.
        /// </summary>
        /// <param name="builder">The configuration builder.</param>
        /// <param name="path">The path to the KeePass database file (KDBX).</param>
        /// <param name="compositeKey">The composite key used to unlock the KeePass database.</param>
        /// <param name="connection">The KeePass database connection.</param>
        /// <param name="filterEntries">Filter used to narrow the selection of entries loaded by the KeePass configuration provider.</param>
        /// <param name="resolveKey">The entry mapping to a string value used as the key in configuration lookups.</param>
        /// <param name="resolveValue">The entry mapping to a string value used as the value in configuration lookups.</param>
        /// <param name="provider">The file provider.</param>
        /// <param name="optional">if set to <c>true</c> [optional].</param>
        /// <param name="reloadOnChange">if set to <c>true</c> [reload on change].</param>
        /// <returns>The same <see cref="T:Microsoft.Extensions.Configuration.IConfigurationBuilder" />.</returns>
        public static IConfigurationBuilder AddKeePass(
                    this IConfigurationBuilder builder,
                    string path,
                    CompositeKey compositeKey,
                    IOConnectionInfo connection = null,
                    Func<PwDatabase, IEnumerable<PwEntry>> filterEntries = null,
                    Func<PwEntry, string> resolveKey = null,
                    Func<string, PwEntry, string> resolveValue = null,
                    IFileProvider provider = null,
                    bool optional = false,
                    bool reloadOnChange = false)
        {
            if (provider == null && Path.IsPathRooted(path))
            {
                provider = new PhysicalFileProvider(Path.GetDirectoryName(path));
                path = Path.GetFileName(path);
            }

            connection = connection ?? new IOConnectionInfo { Path = path };

            var source = new KeePassConfigurationSource()
            {
                Connection = connection,
                CompositeKey = compositeKey,
                FilterEntries = filterEntries,
                ResolveKey = resolveKey,
                ResolveValue = resolveValue,
                FileProvider = provider,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            };

            return builder.Add(source);
        }

        #endregion
    }
}
