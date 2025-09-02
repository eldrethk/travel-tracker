🌍 Travel Expense Tracker

A web application for managing travel expenses with receipt storage and expense categorization.
Built with ASP.NET Core 9.0 MVC and deployed on Azure App Service with automated CI/CD.

🎥 Demo


This demo shows creating a trip, adding expenses with receipt uploads, and viewing categorized summaries.

✨ Features

🗺️ Trip Management – Create and manage travel trips with date validation

💰 Expense Tracking – Add expenses with categories (Accommodation, Transportation, Meals, Entertainment, Other)

🧾 Receipt Storage – Upload and store receipt images using Azure Blob Storage

📊 Expense Summaries – View categorized expense breakdowns for each trip

☁️ Data Persistence – Uses Azure Cosmos DB for scalable data storage

🛠️ Technology Stack
Component	Technology
Backend	ASP.NET Core 9.0 MVC
Database	Azure Cosmos DB
File Storage	Azure Blob Storage
Hosting	Azure App Service
CI/CD	GitHub Actions
Authentication	Anonymous (extensible)
🏗️ Architecture
Services

ITripService / TripService – Handles trip CRUD operations

IExpenseService / ExpenseService – Manages expense data and categorization

IBlobService / BlobService – Handles receipt image upload and storage

IContainerNameResolver – Manages Azure Blob container naming

Data Models

Trip – Represents a travel trip with dates and expense summary

Expense – Individual expense items with categories and amounts

ExpenseSummary – Aggregated expense data by category

ExpenseCategory – Enum for expense categorization
