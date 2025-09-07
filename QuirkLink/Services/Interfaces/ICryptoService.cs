namespace QuirkLink.Services.Interfaces
{
    public interface ICryptoService
    {
        public (string cipherText, string iv) Encrypt(string plainText);

        public string Decrypt(string cipherText, string iv);
    }
}
