using System.Collections.Generic;
using FluentAssertions;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using NSubstitute;
using Xunit;

namespace KeePass.Extensions.Configuration.Tests
{
    public class KeePassConfigurationProviderTests
    {
        #region Fields

        /// <summary>
        /// Gets or sets the builder.
        /// </summary>
        /// <value>
        /// The builder.
        /// </value>
        private KeePassConfigurationSource ConfigurationSource { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeePassConfigurationProviderTests"/> class.
        /// </summary>
        public KeePassConfigurationProviderTests()
        {
            ConfigurationSource = Substitute.For<KeePassConfigurationSource>();
            ConfigurationSource.Connection = new IOConnectionInfo(){Path = "KeePassTestDatabase.kdbx"};

            var compositeKey = new CompositeKey();
            compositeKey.AddUserKey(new KcpPassword("1234"));
            ConfigurationSource.CompositeKey = compositeKey;
        }

        #endregion

        #region Tests

        [Theory]
        [InlineData("KeePassTestDatabase", new[] { "Internet", "Duplicate Entries" })]
        [InlineData("KeePassTestDatabase:Internet", new[] { "Facebook", "npm", "Twitter" })]
        [InlineData("KeePassTestDatabase:Internet:Facebook", new[] { "Takent33" })]
        [InlineData("KeePassTestDatabase:Internet:npm", new[] { "FrancesRBenjamin@teleworm.us" })]
        [InlineData("KeePassTestDatabase:Internet:Twitter", new[] { "Nottlespiche" })]
        [InlineData("KeePassTestDatabase:Duplicate Entries:Duplicate Entry", new[] { "BaconRules", "BaconRules`1", "BaconRules`2" })]
        public void Load(string parentPath, string[] expectedChildKeys)
        {
            var provider = new KeePassConfigurationProvider(ConfigurationSource);
            provider.Load();
            var keys = provider.GetChildKeys(new List<string>(), parentPath);

            keys.Should().Contain(expectedChildKeys);
        }

        #endregion
    }
}
