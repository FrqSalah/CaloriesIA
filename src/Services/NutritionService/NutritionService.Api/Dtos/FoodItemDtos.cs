namespace NutritionService.Api.Dtos;

public record CreateFoodItemDto(string Name, double Calories, double Protein, double Carbohydrates, double Fat);
