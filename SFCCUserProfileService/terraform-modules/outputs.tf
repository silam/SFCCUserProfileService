output "function_app_name" {
  value = azurerm_function_app.function_app.name
  description = "Deployed function app name"
}

output "function_app_default_hostname" {
  value = azurerm_function_app.function_app.default_hostname
  description = "Deployed function app hostname"
}


output "key_vault_endpoint" {
  value = azurerm_key_vault.key-vault.vault_uri
}

output "key_vault_id" {
  value = azurerm_key_vault.key-vault.id
}


////////////////////

/*
output "resource_group_name" {
  value = azurerm_resource_group.rg.name
}

output "azurerm_key_vault_name" {
  value = azurerm_key_vault.vault.name
}

output "azurerm_key_vault_id" {
  value = azurerm_key_vault.vault.id
}
*/

///////////////////