# Odyssey API

This is a Demo project for the Odyssey API Integration Testing presentation. 

## Getting Started

1. Make sure you have .net 7 installed on your machine.
2. Clone the repo
3. Open a terminal and navigate to the root of the project
4. Run the follow command to create and update deploy the database

```
cd src/OdysseyAPI/
dotnet ef database update --context PhonebookDbContext
```

5. Run the following command to start the application

```
dotnet run
```

6. Open a browser and navigate to http://localhost:5197/swagger/index.html
