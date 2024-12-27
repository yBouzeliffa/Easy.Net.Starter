using Easy.Net.Starter.Models;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Text;

namespace Easy.Net.Starter.Services;

public class RsaDto
{
    public string PrivateKey { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
}
public interface IRsaService
{
    RsaDto GenerateKeyPair();
    byte[] CreateApiKey();
    bool ValidateApiKey(byte[] signature, string publicKeyPem);
}
public class RsaService : IRsaService
{
    private readonly AppSettings _settings;
    public RsaService(AppSettings settings)
    {
        _settings = settings;
    }

    public RsaDto GenerateKeyPair()
    {
        var generator = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
        generator.Init(new KeyGenerationParameters(new SecureRandom(), 256));
        var keyPair = generator.GenerateKeyPair();

        string privateKey = ConvertKeyToPem(keyPair.Private);
        string publicKey = ConvertKeyToPem(keyPair.Public);

        return new RsaDto
        {
            PrivateKey = privateKey,
            PublicKey = publicKey
        };
    }

    public byte[] CreateApiKey()
    {
        byte[] data = Encoding.UTF8.GetBytes(_settings.ApiKey.ApiDataKey);
        AsymmetricCipherKeyPair keyPair = ConvertPemToKeyPair(_settings.ApiKey.ApiPrivateKey);
        var privateKey = keyPair.Private;
        var signer = SignerUtilities.GetSigner("SHA-256withECDSA");
        signer.Init(true, privateKey);
        signer.BlockUpdate(data, 0, data.Length);
        return signer.GenerateSignature();
    }

    public bool ValidateApiKey(byte[] signature, string publicKeyPem)
    {
        byte[] data = Encoding.UTF8.GetBytes(_settings.ApiKey.ApiDataKey);
        AsymmetricKeyParameter publicKey = ConvertPemToPublicKey(publicKeyPem);
        var verifier = SignerUtilities.GetSigner("SHA-256withECDSA");
        verifier.Init(false, publicKey);
        verifier.BlockUpdate(data, 0, data.Length);
        return verifier.VerifySignature(signature);
    }

    private static string ConvertKeyToPem(AsymmetricKeyParameter key)
    {
        using (TextWriter textWriter = new StringWriter())
        {
            var pemWriter = new PemWriter(textWriter);
            pemWriter.WriteObject(key);
            return textWriter.ToString() ?? string.Empty;
        }
    }

    private static AsymmetricCipherKeyPair ConvertPemToKeyPair(string pem)
    {
        using (TextReader textReader = new StringReader(pem))
        {
            var pemReader = new PemReader(textReader);
            return (AsymmetricCipherKeyPair)pemReader.ReadObject();
        }
    }

    private static AsymmetricKeyParameter ConvertPemToPublicKey(string pem)
    {
        using (TextReader textReader = new StringReader(pem))
        {
            var pemReader = new PemReader(textReader);
            var publicKey = (AsymmetricKeyParameter)pemReader.ReadObject();
            return publicKey;
        }
    }
}
