using FluentAssertions;
using KeePassLib.Keys;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Xunit;

namespace KeePass.Extensions.Configuration.Tests
{
    public class KeePassConfigurationExtensionsTests
    {
        #region Fields

        /// <summary>
        /// Gets or sets the builder.
        /// </summary>
        /// <value>
        /// The builder.
        /// </value>
        private IConfigurationBuilder Builder { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeePassConfigurationExtensionsTests"/> class.
        /// </summary>
        public KeePassConfigurationExtensionsTests()
        {
            Builder = new ConfigurationBuilder();

           var builder = new ConfigurationBuilder();

            builder.AddKeePass("Path/To/KeePass.kdbx", useCurrentWindowsAccount: true);
        }

        #endregion

        #region Tests

        [Fact]
        public void AddKeePass_MapsKdbxPathToConnection()
        {
            Builder.AddKeePass("KeePassTestDatabase.kdbx");

            Builder.Sources.Should().NotBeEmpty();
            Builder.Sources.First().Should().BeOfType<KeePassConfigurationSource>();

            var keePassSource = (KeePassConfigurationSource)Builder.Sources.First();
            keePassSource.Path.Should().BeEquivalentTo("KeePassTestDatabase.kdbx");
            keePassSource.Connection.Path.Should().BeEquivalentTo("KeePassTestDatabase.kdbx");
        }

        [Fact]
        public void AddKeePass_WithMasterPassword()
        {
            Builder.AddKeePass("KeePassTestDatabase.kdbx", "1234");
            var keePassSource = (KeePassConfigurationSource) Builder.Sources.First();
            var passwordHash = Convert.ToBase64String(keePassSource.CompositeKey.GetUserKey(typeof(KcpPassword)).KeyData.ReadData());

            passwordHash.Should().BeEquivalentTo("A6xnQhbz4Vx2HuGl4lXwZ5U2I8iziLRFnhP5eNfIRvQ=");
        }

        [Fact]
        public void AddKeePass_WithWindowsAccount()
        {
            Builder.AddKeePass("KeePassTestDatabase.kdbx", useCurrentWindowsAccount: true);
            var keePassSource = (KeePassConfigurationSource)Builder.Sources.First();
            keePassSource.CompositeKey.GetUserKey(typeof(KcpUserAccount)).Should().NotBeNull();
        }

        #endregion
    }
}
