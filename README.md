# AI Calorie Counter - Architecture Documentation

## Overview

### Objective:
AI Calorie Counter is a cross-platform mobile and desktop application built using **C#** and **.NET MAUI**. It targets **Android, iOS, and Windows** platforms to provide an AI-powered calorie and nutrition tracker similar to "FoodCalAI". The app leverages advanced AI/ML technologies, intuitive UI/UX design, and robust backend services to deliver seamless user experiences.

---

## High-Level Architecture

### Key Design Patterns:
- **MVVM** (Model-View-ViewModel) for separation of concerns and maintainable code.
- **Dependency Injection** for service provisioning.
- **Async/Await** for non-blocking operations.
- **Responsive Design** for mobile and desktop compatibility.

### Architecture Diagram:

```mermaid
flowchart LR
    %% Client Apps - .NET MAUI Cross-Platform
    subgraph Client_Apps
        AndroidApp[Android - .NET MAUI]
        iOSApp[iOS - .NET MAUI]
        WindowsApp[Windows - .NET MAUI]
    end

    %% BFF Layer
    subgraph BFF
        MauiAPI[MAUI BFF API]
    end

    %% Backend Services
    subgraph User_Service
        UserAPI[User API]
        UserDB[[PostgreSQL]]
        UserAPI --> UserDB
    end

    subgraph Nutrition_Service
        NutritionAPI[Nutrition API]
        NutritionDB[[PostgreSQL]]
        NutritionAPI --> NutritionDB
    end

    subgraph Image_Analysis_Service
        ImageAPI[Image Recognition API]
    end

    subgraph Calorie_Service
        CalorieAPI[Calorie Computation API]
    end

    subgraph AI_Service
        OpenAI[OpenAI / Azure OpenAI]
    end

    subgraph Notifications
        PushService[Push Notification Service]
    end


    %% Client to BFF
    AndroidApp --> MauiAPI
    iOSApp --> MauiAPI
    WindowsApp --> MauiAPI

    %% BFF to Backend APIs
    MauiAPI --> UserAPI
    MauiAPI --> NutritionAPI
    MauiAPI --> ImageAPI
    MauiAPI --> PushService

    %% ImageAPI flow
    ImageAPI --> AI_Service
    ImageAPI --> CalorieAPI

    %% CalorieAPI usage
    CalorieAPI --> AI_Service
```

---

## Folder Structure

Below is the recommended folder structure for the application to ensure clean maintainability:

```
/src
  /AI_Calorie_Counter
    /Views
      - HomePage.xaml
      - LogMealPage.xaml
      - ProfilePage.xaml
      - SettingsPage.xaml
    /ViewModels
      - HomeViewModel.cs
      - LogMealViewModel.cs
      - ProfileViewModel.cs
      - SettingsViewModel.cs
    /Services
      - IAiRecognitionService.cs
      - IDataSyncService.cs
      - BarcodeScanningService.cs
    /Models
      - UserProfile.cs
      - FoodLog.cs
      - Goal.cs
    /Database
      - AppDbContext.cs
      - Migrations/
    /Utils
      - Localization/
        - Resources.resx
        - Resources.fr.resx
      - Extensions/
      - Converters/
```

---
