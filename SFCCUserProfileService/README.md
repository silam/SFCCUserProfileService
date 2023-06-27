# Example of User Profile json

{
    "person_key": 12332112,
    "rwsc_employee": false,
    "first_name": "Chas",
    "last_name": "Someone",
    "profile": {
        "addresses": [
            {
                "type": "home",
                "delivery": "1500 E Main Ste 201",
                "city": "Springfield",
                "state": "VA",
                "postal_code": "22162-1010"
            },
            {
                "type": "shipping",
                "delivery": "1313 Mockingbird Lane",
                "city": "Springfield",
                "state": "VA",
                "postal_code": "22162-1030"
            },
            {
                "type": "billing",
                "delivery": "1500 E Main Ste 201",
                "city": "Springfield",
                "state": "VA",
                "postal_code": "22162-1010"
            }
        ],
        "emails": [
            {
                "personal": "somebody@something.com",
                "validated": true
            },
            {
                "work": "IworkHere@myjob.com",
                "validated": false
            },
            {
                "other": "hahaha@gmail.com",
                "validated": false
            }
        ],
        "phones": [
            {
                "type": "mobile",
                "number": "8609221234",
                "sms_capable": true,
                "validated": true
            },
            {
                "type": "home",
                "number": "8609224321",
                "sms_capable": false,
                "validated": false
            }
        ],
        "occupation": "an occupation code or free text",
        "preferencess": {
            "communication": "text",
            "receipt": "paper"
        },
        "industrial_account": {
            "account_name": "Test account for Ecommerce",
            "program_name": "$100 subsidy - ST",
            "program_id": "P-282189",
            "last_transaction_date": "2023-06-13T14:00:00Z",
            "voucher_type": "digital"
        },
        "home_store": "RWSS309",
        "ufx_information": {
            "last_scan_date": "2023-05-04T12:34:00Z",
            "last_scan_url": "http://somesite.com/13125323"
        },
        "related_accounts": [
            12321123,
            12343421,
            53322213
        ],
        "discounts": [],
        "profile_notes": ""
    },
    "id": "25eafa50-dd46-4d8e-88ac-f48a2887b283",
  
}
# Install

choco install azure-functions-core-tools

# Steps to deplou azure function using Terraform

steps by steps to run

1- CD to terraform directory
2 - Enter: >Terraform Init
3 - Enter: >Terraform validate
4 - Enter: >Terrafrom plan -out=state
5 - Enter: >Terraform apply "state"

You see output like this


To perform exactly these actions, run the following command to apply:
    terraform apply "state"
PS C:\dev\SFCCUserProfileService_1\SFCCUserProfileService\SFCCUserProfileService\terraform> terraform apply "state"
azurerm_function_app.function_app: Modifying... [id=/subscriptions/264a2087-c116-4fbf-a051-7209921e0896/resourceGroups/rgSLTerraformRTQwpi/providers/Microsoft.Web/sites/sfccuserprofile-dev-func-app]
azurerm_function_app.function_app: Still modifying... [id=/subscriptions/264a2087-c116-4fbf-a051-...Web/sites/sfccuserprofile-dev-func-app, 10s elapsed]
azurerm_function_app.function_app: Still modifying... [id=/subscriptions/264a2087-c116-4fbf-a051-...Web/sites/sfccuserprofile-dev-func-app, 20s elapsed]
azurerm_function_app.function_app: Modifications complete after 29s [id=/subscriptions/264a2087-c116-4fbf-a051-7209921e0896/resourceGroups/rgSLTerraformRTQwpi/providers/Microsoft.Web/sites/sfccuserprofile-dev-func-app]

Apply complete! Resources: 0 added, 1 changed, 0 destroyed.

Outputs:

function_app_default_hostname = "sfccuserprofile-dev-func-app.azurewebsites.net"
function_app_name = "sfccuserprofile-dev-func-app"
key_vault_endpoint = "https://slkvterraformrtqwpi.vault.azure.net/"
key_vault_id = "/subscriptions/264a2087-c116-4fbf-a051-7209921e0896/resourceGroups/rgSLTerraformRTQwpi/providers/Microsoft.KeyVault/vaults/SLkvTerraformRTQwpi"
PS C:\dev\SFCCUserProfileService_1\SFCCUserProfileService\SFCCUserProfileService\terraform>


6 - Go to application directory and enter 
    >func azure functionapp publish sfccuserprofile-dev-func-app

7 - To clean up, enter the following

    > terraform destroy

    ==============================

    To deploy Terraform using Azure Devops

    1 - az login 
    or 
        az login --user-device-code

        [
  {
    "cloudName": "AzureCloud",
    "homeTenantId": "cdd071d0-805d-4b10-bac9-ee8225b4cbdc",
    "id": "264a2087-c116-4fbf-a051-7209921e0896",
    "isDefault": true,
    "managedByTenants": [],
    "name": "Azure subscription 1",
    "state": "Enabled",
    "tenantId": "cdd071d0-805d-4b10-bac9-ee8225b4cbdc",
    "user": {
      "name": "silam@hotmail.com",
      "type": "user"
    }
  }
]


2- Get the list of accouint

C:\Users\silam>az account list --output table
Name                                   CloudName    SubscriptionId                        TenantId
        State    IsDefault
-------------------------------------  -----------  ------------------------------------  ------------------------------------  -------  -----------
EdgeOS_IoT_CBL-Mariner_DevTest         AzureCloud   0012ca50-c773-43b2-80e2-f24b6377145c  72f988bf-86f1-41af-91ab-2d7cd011db47  Enabled  False
Azure subscription 1                   AzureCloud   264a2087-c116-4fbf-a051-7209921e0896  cdd071d0-805d-4b10-bac9-ee8225b4cbdc  Enabled  True
Sandbox                                AzureCloud   e576cc29-a5c2-4571-9578-e353fffc910a  cdd071d0-805d-4b10-bac9-ee8225b4cbdc  Enabled  False
serverless pay as you go subscription  AzureCloud   8e2b185d-ce09-4c4b-a791-64c7bd2ef6ec  7568cf73-df78-4d0b-b960-324ecc60d97f  Enabled  False
azuredevopsserverless                  AzureCloud   ff4a6f94-0b93-4781-820b-67050b804221  7568cf73-df78-4d0b-b960-324ecc60d97f  Enabled  False

3 - set subscription

az account set --subscription <Azure-SubscriptionId>

4 - Create service principal

az ad sp create-for-rbac --role="Contributor" --scopes="/subscriptions/SUBSCRIPTION_ID"
or

az ad sp create-for-rbac --role="Contributor" --scopes="/subscriptions/SUBSCRIPTION_ID" --name="Azure-Terraform-DevOps"


C:\Users\silam>az ad sp create-for-rbac --role="Contributor" --scopes="/subscriptions/264a2087-c116-4fbf-a051-7209921e0896" --name="Azure-Terraform-DevOps"
Creating 'Contributor' role assignment under scope '/subscriptions/264a2087-c116-4fbf-a051-7209921e0896'
The output includes credentials that you must protect. Be sure that you do not include these credentials in your code or check the credentials into your source control. For more information, see https://aka.ms/azadsp-cli
{
  "appId": "6aa2de50-7d2a-4683-a0ab-9347c5c36a06",
  "displayName": "Azure-Terraform-DevOps",
  "password": "aVX8Q~gh~Qpe1C0vp2bt7XVsmENfxIQZfXOEvbc2",
  "tenant": "cdd071d0-805d-4b10-bac9-ee8225b4cbdc"
}


