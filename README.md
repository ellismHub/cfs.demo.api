# DEMO User database API

A User database repository that stores details about a user.
The api is implemented as a ASP dotnet core Web API in C#

It implements the following operations:
- GET /api/users → list all users
- GET /api/users/{id} → get single user
- POST /api/users → create user
- PUT /api/users/{id} → update user
- DELETE /api/users/{id} → delete user

The API is deployed to Azure as a Web API resource and is protected by a Service principle from MS Entra. 
The SPN has userdb.read userdb.write role permissions which validated by the api controllers.
It is publicly available.
Note that this a a rudamentary implementation and does does consider firewalls/VNETS/Private Eps/APIMS etc.

Github repo:
https://github.com/ellismHub/cfs.demo.api

The postman collection to excercise this API is located in the git:

[https://github.com/ellismHub/cfs.demo.api](https://github.com/ellismHub/cfs.demo.api/tree/main/Postman)

Note : copilot was used generate some code and tests
