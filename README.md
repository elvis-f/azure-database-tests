# Azure tests

![Azure logo](https://upload.wikimedia.org/wikipedia/commons/a/a8/Microsoft_Azure_Logo.svg)

Basic azure project to strengthen my understanding of Azure services and .NET functionality

# How to run

Clone this project into your repository:
`git clone https://github.com/elvis-f/azure-database-tests`

Navigate inside of the folder and then run it:
```
cd azure-database
dotnet run
```

# Azurite

This version uses Azurite local storage emulation, if there is a need to connect to a live Azure instance then edit `blobConnectionString` to have your Azure credidentials (USE A REFERENCE TO A SECRET STORAGE, DONT STORE YOUR CONNECTION STRING IN CODE!!!)
