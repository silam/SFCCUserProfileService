variable "project" {
  type = string
  description = "Project name"
}

variable "environment" {
  type = string
  description = "Environment (dev / stage / prod)"
}

variable "location" {
  type = string
  description = "Azure region"
}

///////////////////////////////////////////////

/*
variable "resource_group_location" {
  type        = string
  description = "Location for all resources."
  default     = "eastus"
}

variable "resource_group_name_prefix" {
  type        = string
  description = "Prefix of the resource group name that's combined with a random ID so name is unique in your Azure subscription."
  default     = "rg"
}

variable "vault_name" {
  type        = string
  description = "The name of the key vault to be created. The value will be randomly generated if blank."
  default     = ""
}

variable "key_name" {
  type        = string
  description = "The name of the key to be created. The value will be randomly generated if blank."
  default     = ""
}

variable "sku_name" {
  type        = string
  description = "The SKU of the vault to be created."
  default     = "standard"
  validation {
    condition     = contains(["standard", "premium"], var.sku_name)
    error_message = "The sku_name must be one of the following: standard, premium."
  }
}

variable "key_permissions" {
  type        = list(string)
  description = "List of key permissions."
  default     = ["List", "Create", "Delete", "Get", "Purge", "Recover", "Update", "GetRotationPolicy", "SetRotationPolicy"]
}

variable "secret_permissions" {
  type        = list(string)
  description = "List of secret permissions."
  default     = ["Set"]
}

variable "key_type" {
  description = "The JsonWebKeyType of the key to be created."
  default     = "RSA"
  type        = string
  validation {
    condition     = contains(["EC", "EC-HSM", "RSA", "RSA-HSM"], var.key_type)
    error_message = "The key_type must be one of the following: EC, EC-HSM, RSA, RSA-HSM."
  }
}

variable "key_ops" {
  type        = list(string)
  description = "The permitted JSON web key operations of the key to be created."
  default     = ["decrypt", "encrypt", "sign", "unwrapKey", "verify", "wrapKey"]
}

variable "key_size" {
  type        = number
  description = "The size in bits of the key to be created."
  default     = 2048
}

variable "msi_id" {
  type        = string
  description = "The Managed Service Identity ID. If this value isn't null (the default), 'data.azurerm_client_config.current.object_id' will be set to this value."
  default     = "20230608"
}

*/

////////////////////////////////////////////

variable "subscription_id" {
    description = "Azure Tenant Subscription ID"
    type = string
    default = "264a2087-c116-4fbf-a051-7209921e0896"
}
variable "client_id" {
    description = "Service Principal App ID"
    type = string
    default = "3261c2c7-db0a-447b-8459-208f00e66893"
}
variable "client_secret" {
    description = "Service Principal Password"
    type = string
    default = "zsn8Q~uLSmU~w0RfySM2E9o6OPHwKH32KehJaakX"
}
variable "tenant_id" {
    description = "Azure Tenant ID"
    type = string
    default = "cdd071d0-805d-4b10-bac9-ee8225b4cbdc"
}


variable "rg_name" {
    description = "Resource Group Name"
    type = string
    default = "rgSLTerraformRTQwpi"
}
variable "rg_location" {
    description = "Resource Group Location"
    type = string
    default = "westus"
}
variable "kv_name" {
    description = "Azure Key Vault Name"
    type = string
    default = "SLkvTerraformRTQwpi"
}
variable "kv_location" {
    description = "Azure Key Vault Location"
    type = string
    default = "westus"
}
variable "kv_enabled_for_deployment" {
    description = "Azure Key Vault Enabled for Deployment"
    type = string
    default = "true"
}
variable "kv_enabled_for_disk_encryption" {
    description = "Azure Key Vault Enabled for Disk Encryption"
    type = string
    default = "true"
}
variable "kv_enabled_for_template_deployment" {
    description = "Azure Key Vault Enabled for Deployment"
    type = string
    default = "true"
}
variable "kv_sku_name" {
    description = "Azure Key Vault SKU (Standard or Premium)"
    type = string
    default = "standard"
}

variable "kv_secret_name" {
    description = "Azure Key Vault Secret Name"
    type = string
    default = "TerraformSecret"
}
variable "kv_secret_value" {
    description = "Azure Key Vault Secret Value"
    type = string
    default = "Terraform Secret"
}