using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Sharprompt;
using System.Security.Cryptography.X509Certificates;

var url = Prompt.Input<string>("Insert the Key Vault Uri");
var clientUrl = new Uri(url);

Console.WriteLine("Listing certificates...");
var client = new CertificateClient(clientUrl, new DefaultAzureCredential());

var certificates = client.GetPropertiesOfCertificates();
if (!certificates.Any())
{
    Console.WriteLine("This Vault don't have certificates");
    return;
}

var certificateReference = Prompt.Select("Select a certificate", certificates, textSelector: (certificate) => certificate.Name);

var storeName = Prompt.Select("Select a store name",
    Enum.GetValues(typeof(StoreName)).Cast<StoreName>(), textSelector: (name) => name.ToString());

var storeLocation = Prompt.Select("Select a store location", 
    Enum.GetValues(typeof(StoreLocation)).Cast<StoreLocation>(), textSelector: (location) => location.ToString());

Console.WriteLine("Downloading certificate...");
var certificate = (await client.DownloadCertificateAsync(certificateReference.Name)).Value;

var certificateStore = new X509Store(storeName, storeLocation);
Console.WriteLine("Opening the store..");
certificateStore.Open(OpenFlags.MaxAllowed);

var cs = certificateStore.Certificates;
certificateStore.Add(certificate);
certificateStore.Close();
Console.WriteLine("Certificate added successfully");