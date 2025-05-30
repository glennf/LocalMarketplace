# LocalMarketplace

[![CI Build Status](https://github.com/glennf/LocalMarketplace/actions/workflows/ci.yml/badge.svg)](https://github.com/glennf/LocalMarketplace/actions/workflows/ci.yml)
[![Code Quality](https://github.com/glennf/LocalMarketplace/actions/workflows/code-quality.yml/badge.svg)](https://github.com/glennf/LocalMarketplace/actions/workflows/code-quality.yml)

A cross-platform mobile application built with .NET MAUI that facilitates buying and selling items locally with map-based discovery features.

## Project Overview

LocalMarketplace is designed to help users discover, buy, and sell items in their local area using modern mobile app technologies. The application features location-based item discovery, user authentication, messaging between buyers and sellers, and comprehensive item management.

## Architecture

This project follows the MVVM (Model-View-ViewModel) architecture pattern:

- **Models**: Plain data objects representing the core business entities
- **Views**: XAML-based UI with minimal code-behind
- **ViewModels**: Business logic and data binding components
- **Services**: Backend services for data access and business operations

## Key Features

- Map-based item discovery
- User authentication and profiles
- Real-time messaging between users
- Item listing management
- Search and filtering capabilities
- Offline data access

## Development Setup

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or Visual Studio Code
- MAUI workload installed

### Building the Project

To build for different platforms:

- **Windows**: `dotnet build -f:net9.0-windows10.0.19041.0`
- **macOS**: `dotnet build -f:net9.0-maccatalyst`
- **Android**: `dotnet build -f:net9.0-android`
- **iOS**: `dotnet build -f:net9.0-ios`

### Running Tests

Run the comprehensive test suite with:

```bash
dotnet test LocalMarketplace.Tests/LocalMarketplace.Tests.csproj
```

## Technology Stack

- .NET MAUI for cross-platform UI
- CommunityToolkit.Mvvm for MVVM implementation
- SQLite for local data storage
- Azure Identity for authentication
- xUnit and Moq for testing
