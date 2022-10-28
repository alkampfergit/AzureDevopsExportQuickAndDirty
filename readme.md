# Azure devops exporter

## Example commandline parameters

--address https://dev.azure.com/org-name --teamproject teamproject-name

You can also include a PAT in commandline, but for this version PAT is specified in 
clear text, so it is still better to avoid specifying a pat, the software will
open the standard interactive credentials to connet to the account and credentials
are cached by the operating system.

## Scope of the project

Just a simple way to connect to your Azure DevOps account and extract
some information and data with API that can be used to have some
statistics about usage and some data.

It is meant as an example on how to connect to a Team Project and 
start retrieving data that can be used to report or to extend the product.