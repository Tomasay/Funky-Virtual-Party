namespace Glitch9.Encryption
{
    public readonly struct EncryptedString
    {
        public string Value { get; }

        public EncryptedString(string value)
        {
            Value = Encrypter.EncryptString(value);
        }

        public string Decrypt()
        {
            return Encrypter.DecryptString(Value);
        }

        public static explicit operator string(EncryptedString encryptedString)
        {
            return encryptedString.Value;
        }

        public static explicit operator EncryptedString(string value)
        {
            return new EncryptedString(value);
        }

        public readonly override string ToString()
        {
            return Value;
        }
    }
}