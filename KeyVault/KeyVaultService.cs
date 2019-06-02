using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;      // NuGet
using Microsoft.Azure.KeyVault;                             // NuGet
using NLog;                                                 // NuGet

namespace AzureServices.KeyVault
{
    public class KeyVaultService: IDisposable
    {
        protected KeyVaultClient client;
        protected Logger logger = LogManager.GetCurrentClassLogger();

        public KeyVaultService(string user, string password, Logger customLogger = null)
        {
            try
            {
                this.client = GetKeyVaultClient(user, password);

                // Use the specified logger if provided
                if (customLogger != null)
                    this.logger = customLogger;

                logger.Debug($"KeyVaultService: Successfully created new instance of KeyVaultService");
            }
            catch (Exception ex)
            {
                logger.Error($"KeyVaultService: Failed to create new instance of KeyVaultService\n{ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param> User ID / Application (client) ID
        /// <param name="password"></param> User PW / Client secrets
        /// <returns></returns>
        /// <example>
        ///     KeyVaultClient client = GetKeyVaultClient("[Application (client) ID]", "[Client secrets]");
        /// </example>
        private KeyVaultClient GetKeyVaultClient(string user, string password)
        {
            try
            {
                // Define the KeyVaultClient.AuthenticationCallback Delegate
                var authCallBackDel = new KeyVaultClient.AuthenticationCallback(async (string authority, string resource, string scope) =>
                {
                    var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
                    ClientCredential clientCred = new ClientCredential(user, password);
                    var authResult = await context.AcquireTokenAsync(resource, clientCred);
                    return authResult.AccessToken;
                });
                // Create the Key Vault Client
                KeyVaultClient client = new KeyVaultClient(authCallBackDel);

                logger.Debug($"GetKeyVaultClient: Successfully created new Key Vault Client");

                return client;
            }
            catch (Exception ex)
            {
                logger.Error($"GetKeyVaultClient: Failed to create Key Vault Client\n{ex.Message}\n{ex.StackTrace}");
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param> KeyVaultClient with access to key vault
        /// <param name="secretId"></param> Secret Identifier 
        /// <returns></returns>
        /// <example>
        ///     GetSecret(KeyVaultClient, "https://[kv name].vault.azure.net/secrets/[secret name]/[guid]")
        /// </example>
        public string GetSecret(string secretId)
        {
            try
            {
                var secret = client.GetSecretAsync(secretId).GetAwaiter().GetResult();
                logger.Debug($"GetSecret: Successfully obtained secret");
                return Convert.ToString(secret.Value);
            }
            catch (Exception ex)
            {
                logger.Error($"GetSecret: Failed to get Secret using secretId\n{ex.Message}\n{ex.StackTrace}");
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param> KeyVaultClient with access to key vault
        /// <param name="keyVaultDns"></param> Key Vault DNS Name
        /// <param name="secretname"></param> Secret Name in this Key Vault
        /// <returns></returns>
        /// <example>
        ///     GetSecret(client, "https://[kv name].vault.azure.net/", "[secret name]")
        /// </example>
        public string GetSecret(string keyVaultDns, string secretname)
        {
            try
            {
                var secret = client.GetSecretAsync(keyVaultDns, secretname).GetAwaiter().GetResult();
                logger.Debug($"GetSecret: Successfully obtained secret");
                return Convert.ToString(secret.Value);
            }
            catch (Exception ex)
            {
                logger.Error($"GetSecret: Failed to get Secret using kvDns and secretName\n{ex.Message}\n{ex.StackTrace}");
                throw ex;
            }
        }

        public void Dispose()
        {
            try
            {
                this.client.Dispose();
                this.client = null;
                GC.SuppressFinalize(this);
                logger.Debug($"Dispose: Successfully disposed of KeyVaultService");
            }
            catch (Exception ex)
            {
                this.client = null;
                logger.Error($"Dispose: Failed to properly dispose of KeyVaultService\n{ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
    }
}
