using System.Text;
using Xunit;

namespace SmartHome.Tests
{
    public class UtilsTest
    {
        [Fact]
#pragma warning disable CA1822 // Mark members as static
        public void EncryptAndDecrypt()
#pragma warning restore CA1822 // Mark members as static
        {
            const string Unencoded = "Foo";

            byte[] encoded = EncryptionHelpers.Encrypt(
                Encoding.UTF8.GetBytes(Unencoded));

            string decoded = Encoding.UTF8.GetString(
                EncryptionHelpers.Decrypt(encoded));

            Assert.Equal(Unencoded, decoded);
        }
    }
}
