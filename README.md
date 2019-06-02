# Azure Services: Key Vault

- This library assumes you have an Azure Key Vault setup along with user credentials with rights to access the vault.

## Setting up Azure
1. Create new App Credentials
    - Azure Active Directory > App registrations > New registration
        - Settings: Any
        - Copy the Application (client) ID

    - Create Secret for the New App Credentials
        - Certificates & secrets > New client secret > Add
            - Copy the Value

2. Create a Security Group to Access the Key Vault
    - Azure Active Directory > Groups > New Group
        - Settings:
            - Group Type: Security
            - Membership Type: Assigned
            - Members: Add the App Credentials created previously
        
3. Create the Key Vault
    - Key vaults > Add > Create
        - Settings:
            - Add Access Policies: 
                - Permissions: Set all to Get only
                - Select principal: Add the Security Group created earlier
        - Copy the DNS Name

    - Create a Secret to store in the Key Vault
        - Secrets > Generate/Import > Create
            - Copy the Name and the Secret Identifier

## Using this Library
1. Create an instance of KeyVaultService using the App's credentials
    ```cs 
    KeyVaultService kvs = new KeyVaultService("[Application (client) ID]", "[Application Client Secret Value]");
2. Get the secret by using either:
    1. The secret's Secret Identifier, or
        ```cs
        kvs.GetSecret("[Secret Identifier]");
    2. The Key Vault's DNS Name and the Secret's Name
        ```cs 
        kvs.GetSecret("[Key Vault DNS Name]", "[Secret Name]")
3. Call KeyVaultService.Dispose() to end the session and remove the stored credentials