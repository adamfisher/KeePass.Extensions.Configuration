using FluentAssertions;
using KeePassLib.Keys;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Runtime.InteropServices;
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
        }

        #endregion

        #region Tests

        [Theory]
        [InlineData("KeePassTestDatabase.kdbx")]
        [InlineData("../../../KeePassTestDatabase.kdbx")]
        public void AddKeePass_MapsKdbxPathToConnection(string path)
        {
            Builder.AddKeePass(path);

            Builder.Sources.Should().NotBeEmpty();
            Builder.Sources.First().Should().BeOfType<KeePassConfigurationSource>();

            var keePassSource = (KeePassConfigurationSource)Builder.Sources.First();
            keePassSource.Path.Should().BeEquivalentTo(path);
            keePassSource.Connection.Path.Should().BeEquivalentTo(path);

            var provider = new KeePassConfigurationProvider(keePassSource);
            provider.Invoking(x => x.Load()).Should().Throw<InvalidCompositeKeyException>();
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Builder.AddKeePass("KeePassTestDatabase.kdbx", useCurrentWindowsAccount: true);
                var keePassSource = (KeePassConfigurationSource)Builder.Sources.First();
                keePassSource.CompositeKey.GetUserKey(typeof(KcpUserAccount)).Should().NotBeNull();
            }

            Assert.True(true);
        }

        #endregion
    }
}
