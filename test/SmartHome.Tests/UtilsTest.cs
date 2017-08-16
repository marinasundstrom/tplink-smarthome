using System;
using System.Text;
using Xunit;

namespace SmartHome.Tests
{
    public class UtilsTest
    {
        [Fact]
        public void EncryptAndDecrypt()
        {
            string unencoded = "Foo";

            var encoded = Utils.Encrypt(
                Encoding.UTF8.GetBytes(unencoded));

            var decoded = Encoding.UTF8.GetString(
                Utils.Decrypt(encoded));

            Assert.Equal(unencoded, decoded);
        }
    }
}
