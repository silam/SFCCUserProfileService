terraform {
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      # Root module should specify the maximum provider version
      # The ~> operator is a convenient shorthand for allowing only patch releases within a specific minor release.
      version = "~> 2.26"
    }

    random = {
      source  = "hashicorp/random"
      version = "~>3.0"
    }

  }
}

provider "azurerm" {
  features {}

  subscription_id = var.subscription_id
    client_id = var.client_id
    client_secret = var.client_secret
    tenant_id = var.tenant_id

}

resource "azurerm_resource_group" "resource_group" {
  name = var.rg_name //"${var.project}-${var.environment}-kvrg"
  location = var.rg_location
}

resource "azurerm_storage_account" "storage_account" {
  name = "${var.project}${var.environment}sakv"
  resource_group_name = azurerm_resource_group.resource_group.name
  location = var.location
  account_tier = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_application_insights" "application_insights" {
  name                = "${var.project}-${var.environment}-appinsight"
  location            = var.location
  resource_group_name = azurerm_resource_group.resource_group.name
  application_type    = "web"
}

resource "azurerm_app_service_plan" "app_service_plan" {
  name                = "${var.project}-${var.environment}-appserviceplan"
  resource_group_name = azurerm_resource_group.resource_group.name
  location            = var.location
  kind                = "FunctionApp"
  reserved = true
  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

data "azurerm_key_vault" "key-vault" {
  name                = "SLkvTerraformRTQwpi"
  resource_group_name = azurerm_resource_group.resource_group.name
}


data "azurerm_key_vault_secret" "key-vault-secret" {
  name         = "TerraformSecret"
  key_vault_id = data.azurerm_key_vault.key-vault.id
}
 
resource "azurerm_function_app" "function_app" {
  name                       = "${var.project}-${var.environment}-func-app"
  resource_group_name        = azurerm_resource_group.resource_group.name
  location                   = var.location
  app_service_plan_id        = azurerm_app_service_plan.app_service_plan.id
  app_settings = {
    "WEBSITE_RUN_FROM_PACKAGE"       = "",
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.application_insights.instrumentation_key,
    //"EventGridEndpoint" = var.eventgrid_endpoint
    "CosmosDBConnectionString"  = data.azurerm_key_vault_secret.key-vault-secret.value

  }


  os_type = "linux"
  storage_account_name       = azurerm_storage_account.storage_account.name
  storage_account_access_key = azurerm_storage_account.storage_account.primary_access_key
  version                    = "~3"

  lifecycle {
    ignore_changes = [
      app_settings["WEBSITE_RUN_FROM_PACKAGE"],
    ]
  }
}

/////////////////////////////////////////
// Key Vault
////////////////////////////////////////

/*
resource "random_pet" "rg_name" {
  prefix = var.resource_group_name_prefix
}

resource "azurerm_resource_group" "rg" {
  name     = random_pet.rg_name.id
  location = var.resource_group_location
}

data "azurerm_client_config" "current" {}

resource "random_string" "azurerm_key_vault_name" {
  length  = 13
  lower   = true
  numeric = false
  special = false
  upper   = false
}

locals {
  current_user_id = coalesce(var.msi_id, data.azurerm_client_config.current.object_id)
}

resource "azurerm_key_vault" "vault" {
  name                       = coalesce(var.vault_name, "vault-${random_string.azurerm_key_vault_name.result}")
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = var.sku_name
  soft_delete_retention_days = 7

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions    = var.key_permissions
    secret_permissions = var.secret_permissions
  }
}

resource "random_string" "azurerm_key_vault_key_name" {
  length  = 13
  lower   = true
  numeric = false
  special = false
  upper   = false
}

resource "azurerm_key_vault_key" "key" {
  name = coalesce(var.key_name, "key-${random_string.azurerm_key_vault_key_name.result}")

  key_vault_id = azurerm_key_vault.vault.id
  key_type     = var.key_type
  key_size     = var.key_size
  key_opts     = var.key_ops
*/

# rotation not expeted here
  # rotation_policy {
  #   automatic {
  #     time_before_expiry = "P30D"
  #   }

  #   expire_after         = "P90D"
  #   notify_before_expiry = "P29D"
  # }
#}
//////////////////////////////////////////////////


locals {
    subscription_id = "264a2087-c116-4fbf-a051-7209921e0896"
    client_id = "3261c2c7-db0a-447b-8459-208f00e66893"
    client_secret = "zsn8Q~uLSmU~w0RfySM2E9o6OPHwKH32KehJaakX"
    tenant_id = "cdd071d0-805d-4b10-bac9-ee8225b4cbdc"
}

# resource "azurerm_key_vault" "key-vault" {
#     name      = var.kv_name
#     location  = var.kv_location
#     resource_group_name     = var.rg_name
#     enabled_for_deployment  = var.kv_enabled_for_deployment
#     enabled_for_disk_encryption     = var.kv_enabled_for_disk_encryption
#     enabled_for_template_deployment = var.kv_enabled_for_template_deployment
#     tenant_id   = var.tenant_id
#     sku_name    = var.kv_sku_name
# }

resource "azurerm_key_vault_secret" "key-vault-secret" {
    name = var.kv_secret_name
    value = var.kv_secret_value
    key_vault_id = azurerm_key_vault.key-vault.id
 
    depends_on = [azurerm_resource_group.resource_group, azurerm_key_vault.key-vault]
}

data "azurerm_client_config" "current" {}


resource "azurerm_key_vault" "key-vault" {
    name = var.kv_name
    location = var.kv_location
    resource_group_name = var.rg_name
 
    enabled_for_deployment = var.kv_enabled_for_deployment
    enabled_for_disk_encryption = var.kv_enabled_for_disk_encryption
    enabled_for_template_deployment = var.kv_enabled_for_template_deployment
 
    tenant_id = var.tenant_id
    sku_name = var.kv_sku_name
 
    access_policy {
        tenant_id = data.azurerm_client_config.current.tenant_id
        object_id = data.azurerm_client_config.current.object_id
        key_permissions = ["Create", "Get", "List", "Purge", "Recover",]
        secret_permissions = ["Get", "List", "Purge", "Recover", "Set"]
        certificate_permissions = ["Create", "Get", "List", "Purge", "Recover", "Update"]
    }
 
    depends_on = [azurerm_resource_group.resource_group]
}
